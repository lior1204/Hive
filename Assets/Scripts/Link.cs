using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public abstract class Link : MouseInteractable
{
    [SerializeField] private float startWidth = 0.25f;
    [SerializeField] private float endWidth = 0.1f;
    //parameters
    public bool isActive = false;
    public Planet Origin { get; protected set; }
    public Planet Target { get; protected set; }
    public float TimeStemp { get; set; }
    //references
    private EdgeCollider2D edgeCollider;
    private LineRenderer myLine;
    public HiveController.Hive HiveType { get { return Origin ? Origin.HiveType : HiveController.Hive.Neutral; } }
    void Update()
    {
        SetLinePosition();
        SetEdgeCollider();
    }
    protected static void NewLink(Link link,Planet origin, Planet target)
    {
        if (link)
        {
            //set the members, activity, timestamp, parent, position and color
            link.Origin = origin;
            link.Target = target;
            link.isActive = false;
            link.TimeStemp = Time.time;
            link.transform.parent = origin.transform.parent;
            link.transform.position = Vector3.zero;
            link.SetLine();
        }
    }
    private void SetEdgeCollider()
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < myLine.positionCount; point++)
        {
            Vector3 lineRendererPoint = myLine.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        edgeCollider.points = edges.ToArray();
    }
    private void SetLinePosition()
    {
        Vector3[] pos = { Origin.transform.position, Target.transform.position };//set to origin and target positions
        //draw line behind planet
        pos[0]-= new Vector3(0f, 0f, 0.01f);
        pos[1]-= new Vector3(0f, 0f, 0.01f);
        myLine.SetPositions(pos);
    }

    private void SetLine()
    {
        edgeCollider = this.GetComponent<EdgeCollider2D>();
        myLine = this.GetComponent<LineRenderer>();
        myLine.startWidth = startWidth;
        myLine.endWidth = endWidth;
        UpdateColorAndHighlight();
    }
    protected override void UpdateColorAndHighlight()//update color based on hive and highlight
    {
        
        myLine.endColor = Target.GetComponent<SpriteRenderer>().color;
        if (isHovered || isClicked)//highlighted
        {
            //start color
            if (Origin.HiveRef)
                myLine.startColor = Origin.HiveRef.HiveHighlightColor;
            else
                myLine.startColor = ParamManager.Instance.NeutralHighlightColor;
            //end color
            if (Target.HiveRef)
                myLine.endColor = Target.HiveRef.HiveHighlightColor;
            else
                myLine.endColor = ParamManager.Instance.NeutralHighlightColor;
        }
        else//not highlithed
        {
            //start color
            if (Origin.HiveRef)
                myLine.startColor = Origin.HiveRef.HiveColor;
            else
                myLine.startColor = ParamManager.Instance.NeutralColor;
            //end color
            if (Target.HiveRef)
                myLine.endColor = Target.HiveRef.HiveColor;
            else
                myLine.endColor = ParamManager.Instance.NeutralColor;
        }
    }

    public bool CompareExactTo(Link link)//is the same as another link
    {
        bool members = Origin == link.Origin && Target == link.Target;//check origin and target exactly the same
        bool type = this.GetType() == link.GetType();//check type
        return members && type;
    }
    public bool IsReverse(Link link)//is the same as another link
    {
        bool members = (Origin == link.Target && Target == link.Origin);//check if links have the same member in reversed roles
        bool type = this.GetType() == link.GetType();//check type
        return members && type;
    }
    public Planet GetOther(Planet planet)//return the other planet in the link
    {
        if (planet == Origin)
            return Target;
        if (planet == Target)
            return Origin;
        return null;
    }
    
    
    public virtual void DestroyLink()//remove this link from both members
    {
        Origin.RemoveLink(this);
        Target.RemoveLink(this);
    }
    
}
