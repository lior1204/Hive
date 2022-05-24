using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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

    //references
    private TextMeshPro _strengthDisplay=null;
    private SpriteRenderer _spriteRenderer=null;
    void Start()
    {
        PlanetID = Planet.IDCount;//set id
        PlanetID++;//increase id count
        _strengthDisplay = Instantiate(ReferenceManager.Instance._StrengthDisplayPrefab);//create strength display
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetStartInHive();
        StartCoroutine(GenerateStrength());
    }
    void Update()
    {
        UpdateStrengthDisplay();
    }
    private void SetStartInHive()//add to hive and set color
    {
        if (controllingHive == HiveController.Controller.Player)//if is players first planet make player control.
        {
            HiveController.Player.CapturePlanet(this);
            _spriteRenderer.color = ReferenceManager.Instance.PlayerColor;
            if(isQueen)
                HiveController.Player.Queen=this;
        }
        else if (controllingHive == HiveController.Controller.Enemy)//if is players first planet make player control.
        {
            HiveController.Enemy.CapturePlanet(this);
            _spriteRenderer.color = ReferenceManager.Instance.EnemyColor;
            if (isQueen)
                HiveController.Player.Queen = this;
        }
        else
        {
            _spriteRenderer.color = ReferenceManager.Instance.NeutralColor;
        }
    }

    public void AttemptCapture(Planet planet)
    {

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
            yield return new WaitForSeconds(strengthGrowthRate);
            strength++;
        }
        
    }
}
