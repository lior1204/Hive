using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture : Link
{
    public float strengthCaptured;

    public static Capture NewLink(Planet origin, Planet target)//create new link from outside pools
    {
        GameObject obj = ObjectPooler.Instance.SpawnFromPool(ParamManager.Instance.CAPTUREPOOLTAG);//get from pool
        Capture link = obj.GetComponent<Capture>();
        if (link)
        {
            //set the members timestamp and not active
            link.Origin = origin;
            link.Target = target;
            link.isActive = false;
            link.TimeStemp = Time.time;
            link.strengthCaptured = 0;
        }
        return link;
    }
    public void ConvertToReinforcement()//remove this link from members and create new reinforcement link instead
    {
        Origin.AttemptReinforccing(Target);
        DestroyLink();
    }
    public override void DestroyLink()
    {
        base.DestroyLink();
        ObjectPooler.Instance.ReturnToPool(ParamManager.Instance.CAPTUREPOOLTAG, this.gameObject);
    }
}
