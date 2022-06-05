using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Link 
{
    public bool isActive;
    public Planet Origin { get; private set; }
    public Planet Target { get; private set; }
    public float TimeStemp { get; private set; }
    private LinkVisual visualLink;

    public Link(Planet origin, Planet target)
    {
        this.Origin = origin;
        this.Target = target;
        isActive = false;
        TimeStemp = Time.time;
        //visualLink = ObjectPooler.Instance.SpawnFromPool(ParamManager.Instance.LinkPoolTag).GetComponent<LinkVisual>();
        //visualLink.transform.parent = origin.transform;
        //visualLink.transform.position = origin.transform.position;
    }
    public bool CompareExactTo(Link link)//is the same as another link
    {
        bool members= Origin == link.Origin && Target == link.Target;//check origin and target exactly the same
        bool type = this.GetType() == link.GetType();//check type
        return members && type;
    } 
    public bool IsReverse(Link link)//is the same as another link
    {
        bool members= (Origin == link.Target && Target == link.Origin);//check if links have the same member in reversed roles
        bool type = this.GetType() == link.GetType();//check type
        return members && type;
    }
    private void DrawConnection(Planet captured)
    {
        GameObject captureLineObj = new GameObject();
        captureLineObj.AddComponent(typeof(LineRenderer));
        GameObject.Instantiate(captureLineObj);
        LineRenderer line = captureLineObj.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, Origin.transform.position);
        line.SetPosition(1, captured.transform.position);
        line.SetWidth(0.25f, 0.1f);
        //line.material = Material.;
        //line.SetColors(_spriteRenderer.color, captured.GetComponent<SpriteRenderer>().color);
    }

    public void DestroyLink()//remove this link from both members
    {
        Origin.RemoveLink(this);
        Target.RemoveLink(this);
    }
    public Planet GetOther(Planet planet)//return the other planet in the link
    {
        if (planet == Origin)
            return Target;
        if (planet == Target)
            return Origin;
        return null;
    }
}
