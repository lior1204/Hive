using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public abstract class Link : MouseInteractable
{
    public bool isActive=false;
    public Planet Origin { get; protected set; }
    public Planet Target { get; protected set; }
    public float TimeStemp { get;  set ; }
    private EdgeCollider2D edgeCollider;
    private LineRenderer myLine;
    public HiveController.Hive HiveType { get { return Origin ? Origin.HiveType : HiveController.Hive.Neutral; } }
    

    void Start()
    {
        edgeCollider = this.GetComponent<EdgeCollider2D>();
        myLine = this.GetComponent<LineRenderer>();
    }
    void Update()
    {
        SetEdgeCollider(myLine);
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
    //private void DrawConnection(Planet captured)
    //{
    //    GameObject captureLineObj = new GameObject();
    //    captureLineObj.AddComponent(typeof(LineRenderer));
    //    GameObject.Instantiate(captureLineObj);
    //    LineRenderer line = captureLineObj.GetComponent<LineRenderer>();
    //    line.positionCount = 2;
    //    line.SetPosition(0, Origin.transform.position);
    //    line.SetPosition(1, captured.transform.position);
    //    line.SetWidth(0.25f, 0.1f);
    //    //line.material = Material.;
    //    //line.SetColors(_spriteRenderer.color, captured.GetComponent<SpriteRenderer>().color);
    //}
   
    public override void HoverObject()
    {
        //if (HiveRef)
        //    _spriteRenderer.color = HiveRef.HiveHighlightColor;
        //else
        //    _spriteRenderer.color = ParamManager.Instance.NeutralHighlightColor;
    }
    public override void UnHoverObject()
    {
        //if (HiveRef)
        //    _spriteRenderer.color = HiveRef.HiveColor;
        //else
        //    _spriteRenderer.color = ParamManager.Instance.NeutralColor;
    }
    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        edgeCollider.points = edges.ToArray();
    }
    public virtual void DestroyLink()//remove this link from both members
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
