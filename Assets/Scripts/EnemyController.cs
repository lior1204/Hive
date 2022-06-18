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

            }
        }
    
    }


    [Header("Enemy Threshold Parameters")]
    [SerializeField] [Range(1f, 100)] private int planetSizeScore = 0;
    public float PlanetSizeScore { get { return planetSizeScore; } }
    //[SerializeField][Range(1f,100)] private int smallSizeScore = 0;
    //public float SmallSizeScore { get { return smallSizeScore; } }
    //[SerializeField] [Range(1f, 100)] private int mediumSizeScore = 20;
    //public float MediumSizeScore { get { return mediumSizeScore; } }
    //[SerializeField] [Range(1f, 100)] private int bigSizeScore = 50;
    //public float BigSizeScore { get { return bigSizeScore; } }
    [SerializeField] [Range(1f, 100)] private int strengthScore = 50;
    public float StrengthScore { get { return strengthScore; } }
    [SerializeField] [Range(0.1f, 5f)] private int strengthSkewing = 2;
    public float StrengthSkewing { get { return strengthSkewing; } }
    [SerializeField] [Range(1f, 100)] private int incomeScore = 50;
    public float IncomeScore { get { return incomeScore; } }
    [SerializeField] [Range(1f, 100)] private int neutralScore = 20;
    public float NeutralScore { get { return neutralScore; } }
    [SerializeField] [Range(1f, 100)] private int playerScore = 20;
    public float PlayerScore { get { return playerScore; } }
    [SerializeField] [Range(1f, 500)] private int randomScore = 100;
    public float RandomScore { get { return randomScore; } }
    [SerializeField] [Range(1f, 2f)] private Vector2 relativityScoreModifier = new Vector2(1, 2);
    public Vector2 RelativityScoreModifier { get { return relativityScoreModifier; } }


}
