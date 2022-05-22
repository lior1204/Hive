using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;//singelton
    //references
    private List<Planet> playerPlanets = new List<Planet>();
    //public Planet PlayerPlanets { get { return playerPlanets; }  set { playerPlanets.Add(value); } }

    private void Awake()
    {
        if (Instance != null && Instance != this)// implement singelton
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

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
