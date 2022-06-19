using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;//singelton reference
    private List<PlanetRelatives> planets = new List<PlanetRelatives>();
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPaused)
        {
            foreach (PlanetRelatives planet in planets)
            {
                planet.UpdateRelativesTimes();
            }
        }
    }
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

    public void MakeDesicion()
    {

    }


    private class PlanetRelatives//contains a list of all relatives for a specific planet
    {
        public Planet origin { get; private set; }
        private List<RelativeProfile> relatives = new List<RelativeProfile>();
        public PlanetRelatives(Planet planet)
        {
            this.origin = planet;
        }
        public void UpdateRelativesTimes()//update interaction timer with eeach relative
        {
            List<Collider2D> collided = Physics2D.OverlapCircleAll(origin.transform.position, origin.captureRange)
                .Where(col => col.CompareTag(ParamManager.Instance.PLANETTAG)&&col.gameObject!=origin.gameObject).ToList();//all planets in range exclute myself
            foreach(RelativeProfile relative in relatives)//for all existing relatives
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
                    relatives.Add(new RelativeProfile(origin, target));
            }
        }
        public void CheckForRelativeOverThreshold()//for every relative decide to connect or disconect
        {
            foreach(RelativeProfile relative in relatives)
            {
                relative.CalculateScore();
            }
        }
    
    }


    [Header("Enemy Threshold Parameters")]
    [Header("Capture")]
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
    [SerializeField] [Range(0.5f, 5f)] private float strengthReinforceSkewing = 2;
    public float StrengthReinforceSkewing { get { return strengthReinforceSkewing; } }
    [SerializeField] [Range(1f, 100)] private float incomeReinforceScore = 50;
    public float IncomeReinforceScore { get { return incomeReinforceScore; } }
    [SerializeField] [Range(5f, 15f)] private float incomeReinforceMax = 10;
    public float IncomeReinforceMax { get { return incomeReinforceMax; } }
    [SerializeField] [Range(0.5f, 5f)] private float incomeReinforceSkewing = 2;
    public float IncomeReinforceSkewing { get { return incomeReinforceSkewing; } }
    [SerializeField] [Range(1f, 500)] private float randomReinforceScore = 100;
    public float RandomReinforceScore { get { return randomReinforceScore; } }


    [Header("Disconnect")]
    [SerializeField] [Range(1f, 500)] private float randomDisconnectScore = 100;
    public float RandomDisconnectScore { get { return randomDisconnectScore; } }


    [Header("General")]
    [SerializeField] [Range(1f, 2f)] private float relativityMinModifier = 1;
    public float RelativityMinModifier { get { return relativityMinModifier; } }
    [SerializeField] [Range(2f, 3f)] private float relativityMaxModifier = 2;
    public float RelativityMaxModifier { get { return relativityMaxModifier; } }
    [SerializeField] [Range(1f, 2.5f)] private float relativitySkewing = 1.5f;
    public float RelativitySkewing { get { return relativitySkewing; } }


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
