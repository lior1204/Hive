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
    [SerializeField] private HiveController.Hive hiveType = HiveController.Hive.Neutral;
    public HiveController.Hive HiveType { get { return hiveType; } }
    [SerializeField] private bool isQueen = false;
    [SerializeField] private int startingStrength = 6;
    [SerializeField] private int strengthIncome = 2;
    [SerializeField] [Range(20f, 50f)] private float captureRange = 30;
    [SerializeField] [Range(100f, 300f)] private float visibilityRange = 150;
    //[SerializeField] private float planetSize = 5f;

    public int PlanetID { get; private set; } = 0;

    //state variables
    private int strength = 0;
    private List<Capture> planetsInCaptureInteraction = new List<Capture>();
    private float captureImunity;
    public bool IsImune { get { return captureImunity <= 0; } }
    

    //references
    private TextMeshPro _strengthDisplay = null;
    private SpriteRenderer _spriteRenderer = null;
    private Coroutine strenghtGrowthCoroutine;
    private HiveController HiveRef //dynamic HiveControllerReference based on controllingHive
    {
        get
        {
            if (hiveType == HiveController.Hive.Player) return HiveController.Player;
            else if (hiveType == HiveController.Hive.Enemy) return HiveController.Enemy;
            else return null;
        }
    }

    void Start()
    {
        PlanetID = Planet.IDCount;//set id
        PlanetID++;//increase id count
        _strengthDisplay = Instantiate(ParamManager.Instance._StrengthDisplayPrefab);//create strength display
        _spriteRenderer = GetComponent<SpriteRenderer>();//sprite renderer reference
        strength = startingStrength;//st starting strength
        strenghtGrowthCoroutine = StartCoroutine(GenerateStrength());//start strength and save reference
        SetStartInHive();
        captureImunity = ParamManager.Instance.CaptureImunityTime;
    }
    void Update()
    {
        UpdateStrengthDisplay();
    }
    private void SetStartInHive()//add to hive and set color
    {
        if (HiveRef)//if is players first planet make player control.
        {
            HiveRef.AddPlanet(this);
            _spriteRenderer.color = HiveRef.HiveColor;
            if (isQueen)
                HiveRef.Queen = this;
        }
        else
        {
            _spriteRenderer.color = ParamManager.Instance.NeutralColor;
        }
    }

    private void UpdateStrengthDisplay()// update text and pin to planet
    {
        _strengthDisplay.text = strength + "";
        _strengthDisplay.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }
    IEnumerator GenerateStrength()//increase strength
    {
        while (true)
        {
            if (GameManager.Instance.IsPaused)//pause game
                yield return new WaitUntil(() => !GameManager.Instance.IsPaused);
            yield return new WaitForSeconds(ParamManager.Instance.StrengthUpdateRate);//delay between strength ticks
            strength += CalculateDeltaStrength();
            foreach (Capture c in planetsInCaptureInteraction)//for each capture interaction planet count contribution of capture
            {
                c.strengthCaptured += c.planet.strength > 0 ? ParamManager.Instance.CaptureStrengthOutcome
                    : ParamManager.Instance.ZeroStrengthReducedOutcome;//count less contribution if other has 0 strength
            }
            if (strength <= 0)//if reach zero or below get captured
            {
                strength = 0;
                GetCaptured();
            }
        }
    }
    private int CalculateDeltaStrength()// determines strengrh change per strengt tick
    {
        int deltaStrengt = CalculateStrengthIncome() - CalculateStrengthOutcome();
        return deltaStrengt;
    }
    private int CalculateStrengthIncome()//planet's passive income
    {
        if(HiveRef)
            return strengthIncome;
        return 0;
    }
    private int CalculateStrengthOutcome()//capture cost for each capture interaction
    {
        int outcome = 0;
        int zeroPlanets = planetsInCaptureInteraction.Where(capture => capture.planet.strength <= 0).Count();//count planets with 0 strength
        outcome += zeroPlanets * ParamManager.Instance.ZeroStrengthReducedOutcome;//planets with 0 strength contribute less to outcome
        outcome += ParamManager.Instance.CaptureStrengthOutcome * (planetsInCaptureInteraction.Count() - zeroPlanets);//planets with strength contribute to outcopme
        return outcome;
    }
   
    public void AttemptCapture(Planet other)//start capture of other planet
    {
        //check if this planet in hive and if other planet is not in this hive
        if (hiveType != HiveController.Hive.Neutral && hiveType != other.hiveType)
        {
            if (planetsInCaptureInteraction.Where(p => p.planet == other).Count() == 0) //if not already in capture interaction with other
            {
                planetsInCaptureInteraction.Add(new Capture(other));// add captured planet to list
                //if (captureCoroutine != null)//start capturing if not started
                //    captureCoroutine = StartCoroutine(CaptureInteraction());
                other.UnderCapture(this);//tell other planet they are under capture
            }
        }
    }
    private void UnderCapture(Planet other)//become under capture by another planet
    {
        if (planetsInCaptureInteraction.Where(p => p.planet == other).Count() == 0) //if not already in capture interaction with other
        {
            planetsInCaptureInteraction.Add(new Capture(other));
            //if (captureCoroutine != null)//start capturing if not started
            //    captureCoroutine = StartCoroutine(CaptureInteraction());
        }
    }
    private void GetCaptured()//get captured by the hive that captured for the most time.
    {
        if (planetsInCaptureInteraction.Where(c => c.planet.hiveType != HiveController.Hive.Neutral).Count() > 0)//if in capture with non neutral planet
        {
            //determine hive that captured this planet for the longest
            int playerCaptureDuration = 0;
            int enemyCaptureDuration = 0;
            foreach (Capture c in planetsInCaptureInteraction)
            {
                if (c.planet.hiveType == HiveController.Hive.Player)//count strength taken by player
                {
                    playerCaptureDuration += c.strengthCaptured;
                }
                else if (c.planet.hiveType == HiveController.Hive.Enemy)//count strength taken by enemy
                {
                    enemyCaptureDuration += c.strengthCaptured;
                }
            }
            foreach (Capture c in planetsInCaptureInteraction)
            {
                c.planet.FinishCaptureInteraction(this);//tell other to remove yourself
            }
            planetsInCaptureInteraction.Clear();//remove all interections
            if (playerCaptureDuration > 0 || enemyCaptureDuration > 0)// change hive if at least one capture is with hive
            {
                    if (HiveRef)
                    HiveRef.RemovePlanet(this);
                if (playerCaptureDuration > enemyCaptureDuration)
                    hiveType = HiveController.Hive.Player;
                else
                    hiveType = HiveController.Hive.Enemy;
                HiveRef.AddPlanet(this);
                _spriteRenderer.color = HiveRef.HiveColor;
            }
        }
    }
    private void FinishCaptureInteraction(Planet other)//remove other from interactions list
    {
        Capture c = planetsInCaptureInteraction.Where(capture => capture.planet == other).ElementAt(0);//find Capture
        planetsInCaptureInteraction.Remove(c);//remove from interaction list
    }
    public bool IsInCaptureRange(Planet captured)
    {
        return (Vector2.Distance(transform.position, captured.transform.position) <= captureRange);
    }

    public void HighlightPlanet()
    {
        if (HiveRef)
            _spriteRenderer.color = HiveRef.HiveHighlightColor;
        else
            _spriteRenderer.color = ParamManager.Instance.NeutralHighlightColor;
    }
    public void RemoveHighlightPlanet()
    {
        if (HiveRef)
            _spriteRenderer.color = HiveRef.HiveColor;
        else
            _spriteRenderer.color = ParamManager.Instance.NeutralColor;
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
