using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyController : MonoBehaviour
{
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
    public void MakeDesicion()
    {

    }



    private class RelativeProfile//information on planet and the time of interaction with it
    {
        private Planet origin;
        public Planet target { get; private set; }
        public float timeTogather { get { return timeTogather; } set { timeTogather += value>0?value:0; } }
        public float timeApart { get { return timeApart; } set { timeApart += value > 0 ? value : 0; } }
        public RelativeProfile(Planet origin,Planet target)
        {
            this.origin = origin;
            this.target = target;
            timeTogather = 0;
            timeApart = 0;
        }
        public float RelativityRatio { get { return timeTogather / (timeTogather + timeApart); } }//how much to prioritize this planet
        public float StrengthDifference { get { return origin.strength - target.strength; } }//the stength difference between the planets
        public HiveController.Hive TargetHive { get { return target.HiveType; } }//hive of target
        public Planet.PlanetSize TargetSize { get { return target.Size; } }//size of target
        public float IncomeDifference { get { return origin.CalculateDeltaStrength() - target.CalculateDeltaStrength(); } }//the income difference between the planets
        public float CalculateInteraction()//calculate score for relative
        {
            float score = 0;
            switch(TargetSize)
            {
                case Planet.PlanetSize.Small:
                    {
                        score += ParamManager.Instance.SmallSizeScore;
                        break;
                    }
                case Planet.PlanetSize.Medium:
                    {
                        score += ParamManager.Instance.MediumSizeScore;
                        break;
                    }
                case Planet.PlanetSize.Big:
                    {
                        score += ParamManager.Instance.BigSizeScore;
                        break;
                    }

            }//score for size
            score += StrengthDifference * ParamManager.Instance.StrengthScore;//score for strength difference
            score += IncomeDifference * ParamManager.Instance.IncomeScore;//score for income difference
            if (TargetHive == HiveController.Hive.Neutral) score += ParamManager.Instance.NeutralScore;//score for neutral target
            else if (TargetHive == HiveController.Hive.Player) score += ParamManager.Instance.PlayerScore;//score for player target
            score += Random.Range(-ParamManager.Instance.RandomScore, ParamManager.Instance.RandomScore);//add random noise
            score *= Mathf.Lerp(ParamManager.Instance.RelativityScoreModifier.x,
                ParamManager.Instance.RelativityScoreModifier.y, Mathf.Pow(RelativityRatio, 1.5f));//modifier based on relativity ratio
            return score;
        }
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
}
