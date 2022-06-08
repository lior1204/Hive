using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reinforcement : Link
{
    public static Reinforcement NewLink(Planet origin, Planet target)//create new link from outside pools
    {
        GameObject obj = ObjectPooler.Instance.SpawnFromPool(ParamManager.Instance.REINFORCEMENTPOOLTAG);//get from pool
        Reinforcement link = obj.GetComponent<Reinforcement>();
        Link.NewLink(link, origin, target);
        return link;
    }
    public override void DestroyLink()
    {
        base.DestroyLink();
        ObjectPooler.Instance.ReturnToPool(ParamManager.Instance.REINFORCEMENTPOOLTAG, this.gameObject);
    }
}
