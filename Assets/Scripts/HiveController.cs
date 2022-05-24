using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HiveController : MonoBehaviour
{
    public static HiveController Player;//coupleton
    public static HiveController Enemy;//coupleton
    public enum Controller
    {
        Neutral = 0,
        Player = 1,
        Enemy = 2
    }


    //references
    private List<Planet> playerPlanets = new List<Planet>();
    //public Planet PlayerPlanets { get { return playerPlanets; }  set { playerPlanets.Add(value); } }
    private Planet queen = null;
    public Planet Queen { get { return queen; } set { if (Queen == null) queen = value; } }
    private void Awake()
    {
        if (CompareTag(ReferenceManager.Instance.PLAYERTAG))
            if (Player != null && Player != this)// implement coupleton player
            {
                Destroy(this);
            }
            else
            {
                Player = this;
                //DontDestroyOnLoad(this.gameObject);
            }
        else if (CompareTag(ReferenceManager.Instance.ENEMYTAG))
            if (Enemy != null && Enemy != this)// implement coupleton enemy
            {
                Destroy(this);
            }
            else
            {
                Enemy = this;
                //DontDestroyOnLoad(this.gameObject);
            }
        else
            Destroy(this);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }



    //get and set
    public Planet GetPlanet(int id)//return planet if player controlls planet with id else null
    {
        return playerPlanets.Where(x => x.PlanetID == id).ElementAtOrDefault(0);
    }
    public Planet GetPlanet(Planet planet)//return planet if player controlls planet else null
    {
        return playerPlanets.Where(x => x == planet).ElementAtOrDefault(0);
    }
    public void CapturePlanet(Planet planet)//add new planet to player
    {
        playerPlanets.Add(planet);
    }
}
