using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private EdgeCollider2D _edgeCollider;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    public HiveController.Hive HiveType { get { return Origin ? Origin.HiveType : HiveController.Hive.Neutral; } }

    protected static void NewLink(Link link, Planet origin, Planet target)
    {
        if (link)
        {
            //set the members, activity, timestamp, parent, position and color
            link.Origin = origin;
            link.Target = target;
            link.isActive = false;
            link.TimeStemp = Time.time;
            link.transform.parent = origin.transform;
            link.transform.localPosition = Vector3.forward;
            link._spriteRenderer = link.GetComponent<SpriteRenderer>();
            link._edgeCollider = link.GetComponent<EdgeCollider2D>();
            link._boxCollider = link.GetComponent<BoxCollider2D>();
        }
    }

    void Update()
    {
        SetLineTransform();
        //SetEdgeCollider();
        SetBoxCollider();
    }
    private void SetLineTransform()//set the widht and angle of the link
    {
        float length = Vector2.Distance(Origin.transform.position, Target.transform.position);
        _spriteRenderer.size = new Vector2(length, _spriteRenderer.size.y);
        float angle = Mathf.Atan((Target.transform.position.y - Origin.transform.position.y)/(Target.transform.position.x - Origin.transform.position.x));
        angle *= Mathf.Rad2Deg;
        if (Target.transform.position.x < Origin.transform.position.x) angle += 180;
        transform.eulerAngles = Vector3.forward * angle;

    }

    private void SetBoxCollider()//set box collider points acording to line
    {
        //Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        //_boxCollider.size = S;
        //_boxCollider. .center = new Vector2((S.x / 2), 0);
    }
    private void SetEdgeCollider()//set edge collider points acording to line
    {
        Vector2[] edges = new Vector2[2];

        edges[0] = Vector2.zero;
        edges[0].x += _edgeCollider.edgeRadius;
        edges[1] = Vector2.Distance(Origin.transform.position, Target.transform.position) * Vector2.right;
        edges[1].x -= 2*_edgeCollider.edgeRadius;
        _edgeCollider.points = edges;
    }
    protected override void UpdateColorAndHighlight()//update color based on hive and highlight
    {
        //SetLineColor();
        SetLinkColor();
    }

    private void SetLinkColor()
    {
        if (isHovered || isClicked)//highlighted
        {
            _spriteRenderer.color = Origin.HiveRef.HiveHighlightColor;
        }
        else {
            _spriteRenderer.color = Origin.HiveRef.HiveColor;
        }
    }

    //private void SetLineColor()
    //{
    //    myLine.endColor = Target.GetComponent<SpriteRenderer>().color;
    //    if (isHovered || isClicked)//highlighted
    //    {
    //        //start color
    //        if (Origin.HiveRef)
    //            myLine.startColor = Origin.HiveRef.HiveHighlightColor;
    //        else
    //            myLine.startColor = ParamManager.Instance.NeutralHighlightColor;
    //        //end color
    //        if (Target.HiveRef)
    //            myLine.endColor = Target.HiveRef.HiveHighlightColor;
    //        else
    //            myLine.endColor = ParamManager.Instance.NeutralHighlightColor;
    //    }
    //    else//not highlithed
    //    {
    //        //start color
    //        if (Origin.HiveRef)
    //            myLine.startColor = Origin.HiveRef.HiveColor;
    //        else
    //            myLine.startColor = ParamManager.Instance.NeutralColor;
    //        //end color
    //        if (Target.HiveRef)
    //            myLine.endColor = Target.HiveRef.HiveColor;
    //        else
    //            myLine.endColor = ParamManager.Instance.NeutralColor;
    //    }
    //}

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
