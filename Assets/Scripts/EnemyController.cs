using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;//singelton reference
    //parameters
    [Header("Enemy")]
    [SerializeField] private float enemyDecisionRate = 1f;
    [SerializeField] private int decisionsPerCycle = 4;
    [SerializeField] private float enemyDelay = 5f;

    //references    
    private List<PlanetIntel> planets = new List<PlanetIntel>();
    private List<ActionProfile> actions = new List<ActionProfile>();
    Coroutine decisionsCoroutine;
    private void Awake()
    {
        SetSingelton();
    }
    private void SetSingelton()
    {
        if (Instance != null && Instance != this)// implement singelton
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        decisionsCoroutine = StartCoroutine(DecisionCycle());
    }
    void Update()
    {
        if (!GameManager.Instance.IsPaused)
        {
            foreach (PlanetIntel planet in planets)
            {
                planet.UpdateRelativesTimes();
            }
        }
    }

    IEnumerator DecisionCycle()
    {
        yield return new WaitForSeconds(enemyDelay);
        while (true)
        {
            if (GameManager.Instance.IsPaused)//pause game
                yield return new WaitUntil(() => !GameManager.Instance.IsPaused);
            yield return new WaitForSeconds(enemyDecisionRate);//delay between strength ticks
            MakeDecision();
        }
    }
    public void MakeDecision()
    {
        actions.Clear();
        foreach (PlanetIntel planet in planets)
        {
            actions.AddRange(planet.GetActions());
        }
        actions = actions.OrderBy(act => act.IsPriority).ThenBy(act => act.Score).ToList();
        for(int i = 0; i < Mathf.Min(decisionsPerCycle, actions.Count); i++)
        {
            ActivateAction(actions.ElementAt(i));
        }
    }
    private void ActivateAction(ActionProfile action)
    {
        switch (action.Action)
        {
            case ActionProfile.ActionType.Capture:
                HiveController.Enemy.CapturePlanet(action.origin, action.target);
                break;
            case ActionProfile.ActionType.Reinforce:
                HiveController.Enemy.ReinforcePlanet(action.origin, action.target);
                break;
            case ActionProfile.ActionType.DisconnectCapture:
            case ActionProfile.ActionType.DisconnectReinforcement:
                HiveController.Enemy.RemoveLink(action.origin, action.target);
                break;
        }
    }
    public void AddPlanet(Planet planet)
    {
        planets.Add(new PlanetIntel(planet));
    }
    public void Removelanet(Planet planet)
    {
        PlanetIntel p= planets.FirstOrDefault(intel => intel.origin == planet);
        if (p != null) planets.Remove(p);
    }

    private class PlanetIntel//contains a list of all relatives for a specific planet
    {
        public Planet origin { get; private set; }
        private List<ActionProfile> relatives = new List<ActionProfile>();
        public PlanetIntel(Planet planet)
        {
            this.origin = planet;
        }
        public void UpdateRelativesTimes()//update interaction timer with eeach relative
        {
            List<Collider2D> collided = Physics2D.OverlapCircleAll(origin.transform.position, origin.captureRange)
                .Where(col => col.CompareTag(ParamManager.Instance.PLANETTAG)&&col.gameObject!=origin.gameObject).ToList();//all planets in range exclute myself
            foreach(ActionProfile relative in relatives)//for all existing relatives
            {
                Collider2D collider = collided.FirstOrDefault(col => col.gameObject == relative.target.gameObject);
                if (collider)//if collided add time togather and
                {
                    relative.timeTogather += Time.deltaTime;
                    collided.Remove(collider);//remove from collided
                }
                else//if not add time apart
                {
                    relative.timeApart += Time.deltaTime;
                }
            }
            foreach(Collider2D collider in collided)//for all new colliders add them to relatives
            {
                Planet target = collider.gameObject.GetComponent<Planet>();
                if(target)
                    relatives.Add(new ActionProfile(origin, target));
            }
        }
        public List<ActionProfile> GetActions()//update relative scores and return the top actions
        {
            List<ActionProfile> actions;
            foreach(ActionProfile relative in relatives)
            {
                relative.CalculateScore();
                //Debug.Log("Target: " + relative.target.GetInstanceID()+",Actiom: "+ relative.Action + " ,Score:" + relative.Score) ;
            }
            //list of all actions above threshhold orderd by priority then threshold
            actions = relatives.Where(act => act.Score >= 1).OrderBy(act => act.IsPriority).ThenBy(act=>act.Score).ToList();
            return actions.Take(EnemyController.Instance.decisionsPerCycle).ToList();
        }
    
    }

    
    [Header("Capture")]
    [Header("Enemy Threshold Parameters")]
    [SerializeField] [Range(1f, 100)] private float planetSizeScore = 50;
    public float PlanetSizeScore { get { return planetSizeScore; } }
    //[SerializeField][Range(1f,100)] private int smallSizeScore = 0;
    //public float SmallSizeScore { get { return smallSizeScore; } }
    //[SerializeField] [Range(1f, 100)] private int mediumSizeScore = 20;
    //public float MediumSizeScore { get { return mediumSizeScore; } }
    //[SerializeField] [Range(1f, 100)] private int bigSizeScore = 50;
    //public float BigSizeScore { get { return bigSizeScore; } }
    [SerializeField] [Range(1f, 100)] private float strengthCaptureScore = 50;
    public float StrengthCaptureScore { get { return strengthCaptureScore; } }
    [SerializeField] [Range(0.5f, 5f)] private float strengthCaptureSkewing = 2;
    public float StrengthCaptureSkewing { get { return strengthCaptureSkewing; } }
    [SerializeField] [Range(1f, 100)] private float incomeCaptureScore = 50;
    public float IncomeCaptureScore { get { return incomeCaptureScore; } }
    [SerializeField] [Range(5f, 15f)] private float incomeDifferenceMax = 10;
    public float IncomeDifferenceMax { get { return incomeDifferenceMax; } }
    [SerializeField] [Range(0.5f, 5f)] private float incomeCaptureSkewing = 2;
    public float IncomeCaptureSkewing { get { return incomeCaptureSkewing; } }
    [SerializeField] [Range(2f, 5f)] private float incomeRelevenceBasedStrength = 3;
    public float IncomeRelevenceBasedStrength { get { return incomeRelevenceBasedStrength; } }
    [SerializeField] [Range(1f, 100)] private float neutralScore = 20;
    public float NeutralScore { get { return neutralScore; } }
    [SerializeField] [Range(1f, 100)] private float playerCaptureScore = 50;
    public float PlayerCaptureScore { get { return playerCaptureScore; } }
    [SerializeField] [Range(1f, 500)] private float randomCaptureScore = 100;
    public float RandomCaptureScore { get { return randomCaptureScore; } }

    [Header("Reinforce")]
    [SerializeField] [Range(1f, 100)] private int strengthReinforceScore = 50;
    public float StrengthReinforceScore { get { return strengthReinforceScore; } }
    [SerializeField] [Range(1f, 100)] private float strengthTargetReinforceScore = 80;
    public float StrengthTargetReinforceScore { get { return strengthTargetReinforceScore; } }
    [SerializeField] [Range(1f, 100)] private float incomeReinforceScore = 50;
    public float IncomeReinforceScore { get { return incomeReinforceScore; } }
    [SerializeField] [Range(1, 100f)] private float incomeTargetReinforceScore = 60;
    public float IncomeTargetReinforceScore { get { return incomeTargetReinforceScore; } }
    [SerializeField] [Range(5f, 15f)] private float incomeReinforceMax = 10;
    public float IncomeReinforceMax { get { return incomeReinforceMax; } }
    [SerializeField] [Range(1f, 500)] private float randomReinforceScore = 100;
    public float RandomReinforceScore { get { return randomReinforceScore; } }
    [SerializeField] [Range(1f, 100)] private float reinforceMinStrength = 20;
    public float ReinforceMinStrength { get { return reinforceMinStrength; } }
    [SerializeField] [Range(1f, 100)] private float getHelpMaxStrength = 40;
    public float GetHelpMaxStrength { get { return getHelpMaxStrength; } }


    [Header("Disconnect")]
    [SerializeField] [Range(1f, 100)] private int strengthDisconnectScore = 50;
    public float StrengthDisconnectScore { get { return strengthReinforceScore; } }
    [SerializeField] [Range(0.5f, 5f)] private float strengthDisconnectSkewing = 2;
    public float StrengthDisconnectSkewing { get { return strengthDisconnectSkewing; } }
    [SerializeField] [Range(1f, 100)] private float incomeDisconnectScore = 50;
    public float IncomeDisconnectScore { get { return incomeDisconnectScore; } }
    [SerializeField] [Range(5f, 15f)] private float incomeDisconnectMax = 10;
    public float IncomeDisconnectMax { get { return incomeReinforceMax; } }
    [SerializeField] [Range(0.5f, 5f)] private float incomeDisconnectSkewing = 2;
    public float IncomeDisconnectSkewing { get { return incomeDisconnectSkewing; } }
    [SerializeField] [Range(2f, 5f)] private float incomeRelevenceBasedStrengthDisconnect = 3;
    public float IncomeRelevenceBasedStrengthDisconnect { get { return incomeRelevenceBasedStrengthDisconnect; } }
    [SerializeField] [Range(1f, 500)] private float randomDisconnectScore = 100;
    public float RandomDisconnectScore { get { return randomDisconnectScore; } }


    [Header("General")]
    [SerializeField] [Range(1f, 2f)] private float relativityMinModifier = 1;
    public float RelativityMinModifier { get { return relativityMinModifier; } }
    [SerializeField] [Range(2f, 3f)] private float relativityMaxModifier = 2;
    public float RelativityMaxModifier { get { return relativityMaxModifier; } }
    [SerializeField] [Range(1f, 2.5f)] private float relativitySkewing = 1.5f;
    public float RelativitySkewing { get { return relativitySkewing; } }
    [SerializeField] [Range(1f, 5f)] private float minimumProfileTime = 1.5f;
    public float MinimumProfileTime { get { return minimumProfileTime; } }


    [Header("Thresholds")]
    [SerializeField] [Range(1f, 100)] private float lowStrengthPriorityThreshold = 10;
    public float LowStrengthPriorityThreshold { get { return lowStrengthPriorityThreshold; } }
    [SerializeField]  private float captureThreshold = 500;
    public float CaptureThreshold { get { return captureThreshold; } }
    [SerializeField] private float reinforceThreshold = 500;
    public float ReinforceThreshold { get { return reinforceThreshold; } }
    [SerializeField]  private float disconnectCaptureThreshold = 500;
    public float DisconnectCaptureThreshold { get { return disconnectCaptureThreshold; } }
    [SerializeField]  private float disconnectReinforceThreshold = 500;
    public float DisconnectReinforceThreshold { get { return disconnectReinforceThreshold; } }

}
