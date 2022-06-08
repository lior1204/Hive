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
        Link.NewLink(link, origin, target);
        link.strengthCaptured = 0;//set strength
        return link;
    }
    public void ConvertToReinforcement()//remove this link from members and create new reinforcement link instead
    {
        Reinforcement newLink= Origin.AttemptReinforccing(Target);
        if (newLink)
        {
            newLink.TimeStemp = this.TimeStemp;//keep the old connection timestamp
        }
        DestroyLink();
    }
    public override void DestroyLink()
    {
        base.DestroyLink();
        ObjectPooler.Instance.ReturnToPool(ParamManager.Instance.CAPTUREPOOLTAG, this.gameObject);
    }
}
