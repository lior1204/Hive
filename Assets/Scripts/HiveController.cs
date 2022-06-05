using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HiveController : MonoBehaviour
{
    public static HiveController Player;//coupleton
    public static HiveController Enemy;//coupleton
    public enum Hive //hive type enum
    {
        Neutral = 0,
        Player = 1,
        Enemy = 2
    }
    //references
    private List<Planet> hivePlanets = new List<Planet>();
    private Planet queen = null;
    public Planet Queen { get { return queen; } set { if (Queen == null) queen = value; } }
    public Color HiveColor//public access dynamic color based on hive
    {
        get
        {
            return (CompareTag(ParamManager.Instance.PLAYERTAG)) ? ParamManager.Instance.PlayerColor :
                ParamManager.Instance.EnemyColor;
        }
    }
   public Color HiveHighlightColor//public access dynamic color based on hive
    {
        get
        {
            return (CompareTag(ParamManager.Instance.PLAYERTAG)) ? ParamManager.Instance.PlayerHighlightColor :
                ParamManager.Instance.EnemyHighlightColor;
        }
    }

    [SerializeField] private Hive hiveType=Hive.Player;
    private void Awake()
    {
        //Coupleton
        if (hiveType==Hive.Player)
            if (Player != null && Player != this)// implement singelton player
            {
                Destroy(this);
            }
            else
            {
                Player = this;
            }
        else if (hiveType == Hive.Enemy)
            if (Enemy != null && Enemy != this)// implement singelton enemy
            {
                Destroy(this);
            }
            else
            {
                Enemy = this;
            }
        else
            Destroy(this);
    }
    

    //actions
    public void CapturePlanet(Planet attacker, Planet captured)
    {
        if (hivePlanets.Contains(attacker))//check if attacker is in hive
        {
            if (attacker.IsCapturable(captured))//check if captured is capturable
            {
                attacker.AttemptCapture(captured);
            }
        }
    }

    //get and set
    public Planet GetPlanet(int id)//return planet if player controlls planet with id else null
    {
        return hivePlanets.Where(x => x.PlanetID == id).ElementAtOrDefault(0);
    }
    public Planet GetPlanet(Planet planet)//return planet if player controlls planet else null
    {
        return hivePlanets.Where(x => x == planet).ElementAtOrDefault(0);
    }
    public void AddPlanet(Planet planet)//add new planet to player
    {
        hivePlanets.Add(planet);
    }
    public void RemovePlanet(Planet planet)//add new planet to player
    {
        hivePlanets.Remove(planet);
    }
}
