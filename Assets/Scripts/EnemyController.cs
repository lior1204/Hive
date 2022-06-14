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



    private class RelativityInformation//information on planet and the time of interaction with it
    {
        public Planet planet { get; private set; }
        public float timeTogather { get { return timeTogather; } set { timeTogather += value>0?value:0; } }
        public float timeApart { get { return timeApart; } set { timeApart += value > 0 ? value : 0; } }
        public RelativityInformation(Planet planet)
        {
            this.planet = planet;
            timeTogather = 0;
            timeApart = 0;
        }
        public float RelativityRatio { get { return timeTogather / (timeTogather + timeApart); } }
    }
    private class PlanetRelatives//contains a list of all relatives for a specific planet
    {
        public Planet planet { get; private set; }
        private List<RelativityInformation> relatives = new List<RelativityInformation>();
        public PlanetRelatives(Planet planet)
        {
            this.planet = planet;
        }
        public void UpdateRelativesTimes()
        {
            List<Collider2D> collided = Physics2D.OverlapCircleAll(planet.transform.position, planet.captureRange)
                .Where(col => col.CompareTag(ParamManager.Instance.PLANETTAG)&&col.gameObject!=planet.gameObject).ToList();//all planets in range exclute myself
            foreach(RelativityInformation relative in relatives)//for all existing relatives
            {
                Collider2D collider = collided.FirstOrDefault(col => col.gameObject == relative.planet.gameObject);
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
                Planet p = collider.gameObject.GetComponent<Planet>();
                if(p)
                    relatives.Add(new RelativityInformation(p));
            }
        }
    }
}
