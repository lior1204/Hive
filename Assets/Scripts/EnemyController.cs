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
        
    }
    private class RelativityInformation
    {
        private Planet planet;
        private float timeTogather;
        private float timeApart;
        public RelativityInformation(Planet planet)
        {
            this.planet = planet;
            timeTogather = 0;
            timeApart = 0;
        }
        public float RelativityRatio { get { return timeTogather / (timeTogather + timeApart); } }
    }
    private class PlanetRelatives
    {
        private Planet planet;
        private List<RelativityInformation> relatives = new List<RelativityInformation>();
        public void LocateRelative()
        {
            Collider2D[] relatives=  Physics2D.OverlapCircleAll(planet.transform.position,planet.captureRange);
        }
    }
}
