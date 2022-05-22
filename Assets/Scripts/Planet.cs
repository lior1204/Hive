using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private static int IDCount = 0;
    enum Controller
    {
        Neutral=0,
        Player=1,
        Enemy=2
    }

    //parameters
    [SerializeField] private bool isStartingPlanet = false;
    public int PlanetID { get; private set; } = 0;

    //state variables
    private int strength = 0;
    private Controller hiveController = Controller.Neutral;

    //references

    void Start()
    {
        PlanetID = Planet.IDCount;
        PlanetID++;
        if (isStartingPlanet)//if is players firs plamet make player control.
        {
            hiveController = Controller.Player;
            Player.Instance.CapturePlanet(this);
        }
    }

    void Update()
    {
        
    }

}
