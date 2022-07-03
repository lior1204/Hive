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
    public int PlanetCount { get { return hivePlanets.Count; } }
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

    PlayerController player=null;

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
    private void Start()
    {
        if (hiveType == Hive.Player)
            player = FindObjectOfType<PlayerController>();
    }
    //actions
    public void CapturePlanet(Planet attacker, Planet captured)//crate new capture link between 2 planets
    {
        if (hivePlanets.Contains(attacker))//check if attacker is in hive
        {
            if (attacker.HiveType!=captured.HiveType)//check if captured is not in hive
            {
                attacker.AttemptCapture(captured);
                if (attacker.HiveType == Hive.Player)
                    AudioManager.Instance.OnPlayerConnect();
                else;
                    //AudioManager.Instance.OnEnemyConnect();
            }
        }
    }
    public void ReinforcePlanet(Planet provider, Planet reinforced)//crate new reinforcement link between 2 planets
    {
        if (hivePlanets.Contains(provider)&&hivePlanets.Contains(reinforced))//check if both provider and reinforced in this hive
        {
            provider.AttemptReinforccing(reinforced);
            if (provider.HiveType == Hive.Player)
                AudioManager.Instance.OnPlayerConnect();
            else;
                //AudioManager.Instance.OnEnemyConnect();
        }
    }
    public void RemoveLink(Link link)//end a spesific link belonging to this hive
    {
        if (link&&hivePlanets.Contains(link.Origin))//check that the origin of the link is in the hive.
        {
            link.DestroyLink();
        }
    }
    public void RemoveLink(Planet origin,Planet target)//end a spesific link belonging to this hive
    {
        Link link = origin.GetLink(target);
        RemoveLink(link);
    }
    public void RemoveAllLinksOfPlanet(Planet origin)//destroy all links originating from a planet in hive
    {
        if (hivePlanets.Contains(origin))//check that the origin is in the hive.
        {
            origin.RemoveAllLinks();
        }
    }
    public void RemoveAllLinksToEnemy(Planet target)//destroy all links originating from a planet in hive to target
    {
        foreach(Planet planet in hivePlanets)
        {
            Debug.Log("Hive Remove to target");
            planet.RemoveLinkToTarget(target);
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
        if (hiveType == Hive.Enemy)//add planet to enemy
        {
            EnemyController.Instance.AddPlanet(planet);
        }
    }
    public void RemovePlanet(Planet planet)//add new planet to player
    {
        hivePlanets.Remove(planet);
        if (hiveType == Hive.Enemy)//add planet to enemy
        {
            EnemyController.Instance.Removelanet(planet);
        }if (player)//add planet to enemy
        {
            player.UnclickLostPlanet(planet);
        }

    }
}
