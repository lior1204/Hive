using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reinforcement : Link
{
    public static Reinforcement NewLink(Planet origin, Planet target)//create new link from outside pools
    {
        GameObject obj = ObjectPooler.Instance.SpawnFromPool(ParamManager.Instance.REINFORCEMENTPOOLTAG);//get from pool
        Reinforcement link = obj.GetComponent<Reinforcement>();
        if (link)
        {
            //set the members timestamp and not active
            link.Origin = origin;
            link.Target = target;
            link.isActive = false;
            link.TimeStemp = Time.time;
        }
        return link;
    }
   
}
