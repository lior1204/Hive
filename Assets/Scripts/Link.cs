using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Link 
{
    public bool isActive;
    public Planet origin;
    public Planet target;

    public Link(Planet origin, Planet target)
    {
        this.origin = origin;
        this.target = target;
        isActive = false;

    }
    public bool CompareTo(Link link)
    {
        bool members= origin == link.origin && target == link.target;
        bool type = this.GetType() == link.GetType();
        return members && type;
    }
    private void DrawConnection(Planet captured)
    {
        GameObject captureLineObj = new GameObject();
        captureLineObj.AddComponent(typeof(LineRenderer));
        GameObject.Instantiate(captureLineObj);
        LineRenderer line = captureLineObj.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, origin.transform.position);
        line.SetPosition(1, captured.transform.position);
        line.SetWidth(0.25f, 0.1f);
        //line.material = Material.;
        //line.SetColors(_spriteRenderer.color, captured.GetComponent<SpriteRenderer>().color);
    }
}
