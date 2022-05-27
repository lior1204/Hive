using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;


public class Planet : MonoBehaviour
{
    private static int IDCount = 0;

    //parameters
    [SerializeField] private HiveController.Controller controllingHive = HiveController.Controller.Neutral;
    [SerializeField] private bool isQueen = false;
    [SerializeField] [Min(0.1f)] private float strengthGrowthRate = 0.7f;
    [SerializeField] [Range(20f,50f)] private float captureRange = 30;
    [SerializeField] [Range(100f,300f)] private float visibilityRange = 150;
    //[SerializeField] private float planetSize = 5f;

    public int PlanetID { get; private set; } = 0;

    //state variables
    private int strength = 0;
    //private List<Planet> planetsUnderCapture = new List<Planet>();
    private List<Capture> planetsInCaptureInteraction = new List<Capture>();
    private float captureImunity = ReferenceManager.Instance.CaptureImunityTime;
    public bool IsImune { get { return captureImunity <= 0; } }
    private HiveController HiveRef //dynamic HiveControllerReference based on controllingHive
    { 
        get
        {
            if (controllingHive == HiveController.Controller.Player) return HiveController.Player;
            else if (controllingHive == HiveController.Controller.Enemy) return HiveController.Enemy;
            else return null;
        }
    }

    //references
    private TextMeshPro _strengthDisplay=null;
    private SpriteRenderer _spriteRenderer=null;
    private Coroutine strenghtGrowthCoroutine;
    private Coroutine captureCoroutine;


    void Start()
    {
        PlanetID = Planet.IDCount;//set id
        PlanetID++;//increase id count
        _strengthDisplay = Instantiate(ReferenceManager.Instance._StrengthDisplayPrefab);//create strength display
        _spriteRenderer = GetComponent<SpriteRenderer>();
        strenghtGrowthCoroutine=StartCoroutine(GenerateStrength());//start strength and save reference
        SetStartInHive();
    }
    void Update()
    {
        UpdateStrengthDisplay();
    }
    private void SetStartInHive()//add to hive and set color
    {
        if (HiveRef)//if is players first planet make player control.
        {
            HiveRef.CapturePlanet(this);
            _spriteRenderer.color = HiveRef.HiveColor;
            if(isQueen)
                HiveRef.Queen=this;
        }
        else
        {
            StopCoroutine(strenghtGrowthCoroutine);//if neutral stop generating strength
            _spriteRenderer.color = ReferenceManager.Instance.NeutralColor;
        }
    }

    private void UpdateStrengthDisplay()// update text and pin to planet
    {
        _strengthDisplay.text = strength+"";
        _strengthDisplay.transform.position = new Vector3( transform.position.x, transform.position.y,-1);
    }

    IEnumerator GenerateStrength()//increase strength
    {
        while (true)
        {
            if (GameManager.Instance.IsPaused)//pause game
                yield return new WaitUntil(() =>!GameManager.Instance.IsPaused);
            yield return new WaitForSeconds(CalculateStrengthGainRate());
            strength++;
        }
    }
    public void AttemptCapture(Planet other)//start capture of other planet
    {
        //check if this planet in hive and if other planet is not in this hive
        if (controllingHive != HiveController.Controller.Neutral && controllingHive != other.controllingHive)
        {
            if (planetsInCaptureInteraction.Where(p => p.planet == other).Count() == 0) //if not already in capture
            {
                planetsInCaptureInteraction.Add(new Capture(other));// add captured planet to list
                if (captureCoroutine != null)//start capturing if not started
                    StartCoroutine(CaptureInteraction());
                other.UnderCapture(this);//set captured planet under capture
            }
        }
    }
    private void UnderCapture(Planet other)//become under capture by another planet
    {
        if (planetsInCaptureInteraction.Where(p => p.planet == other).Count() == 0) //if not already in capture
        {
            planetsInCaptureInteraction.Add(new Capture(other));
            if (captureCoroutine != null)//start capturing if not started
                StartCoroutine(CaptureInteraction());
        }
    }
    IEnumerator CaptureInteraction()//lose strength until you reach zero
    {
        StopCoroutine(strenghtGrowthCoroutine);//stop generating strength while capturing
        while (strength>0)//continue if both planets above zero strength
        {
            if (GameManager.Instance.IsPaused)//pause game
                yield return new WaitUntil(() => !GameManager.Instance.IsPaused);
            yield return new WaitForSeconds(CalculateStrengthLossRate());
            strength--;
            foreach(Capture c in planetsInCaptureInteraction)//for each captured planet count duration of capture
            {
                c.strengthCaptured++;
            }
        }
        GetCaptured();
    }

    private void GetCaptured()//get captured by the hive that captured for the most time.
    {
        int playerCaptureDuration = 0;
        int enemyCaptureDuration=0;
        foreach(Capture c in planetsInCaptureInteraction)//determine hive that captured this planet for the longest
        {
            if (c.planet.controllingHive == HiveController.Controller.Player)
                playerCaptureDuration += c.strengthCaptured;
            else if (c.planet.controllingHive == HiveController.Controller.Enemy)
                enemyCaptureDuration += c.strengthCaptured;
        }
        if (playerCaptureDuration > enemyCaptureDuration)
            controllingHive = HiveController.Controller.Player;
        else
            controllingHive = HiveController.Controller.Enemy;
        _spriteRenderer.color = HiveRef.HiveColor;
    }
    private float CalculateStrengthGainRate()
    {
        return strengthGrowthRate;
    }
    private float CalculateStrengthLossRate()
    {
        return ReferenceManager.Instance.CaptureLoseStrengthRate;
    }


    private class Capture
    {
        public Planet planet;
        public int strengthCaptured = 0;
        public Capture(Planet planet)
        {
            this.planet = planet;
        }
    }
}
