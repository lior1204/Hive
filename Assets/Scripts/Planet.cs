using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    enum Controller
    {
        Neutral=0,
        Player=1,
        Enemy=2
    }

    //parameters
    [SerializeField] private bool isStartingPlanet = false;

    //state variables
    private int strength = 0;
    private Controller hiveController = Controller.Neutral;

    //references

    void Start()
    {
        if (isStartingPlanet)//if is players firs plamet make player control.
        {
            hiveController = Controller.Player;
        }
    }

    void Update()
    {
        
    }

}
