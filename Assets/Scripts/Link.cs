using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Link : MouseInteractable
{
    //parameters
    private bool isActive = false;
    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; SetActiveMat(); }
    }
    public Planet Origin { get; protected set; }
    public Planet Target { get; protected set; }
    public float TimeStamp { get; set; }
    //references
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
            link.TimeStamp = Time.time;
            link.transform.parent = origin.transform;
            link.transform.localPosition = Vector3.forward;
            link._spriteRenderer = link.GetComponent<SpriteRenderer>();
            link._boxCollider = link.GetComponent<BoxCollider2D>();
            link.UpdateColorAndHighlight();
        }
    }

    void Update()
    {
        SetLineTransform();
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
    public override void UpdateColorAndHighlight()//update color based on hive and highlight
    {
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
        SetActiveMat();
    }
    private void SetActiveMat()
    {
        if(TimeStamp>=Time.time)
        Debug.Log("Link: "+GetInstanceID()+" Set Mat, active: " + IsActive + " Hive: " + HiveType);
        if (isActive)
        {
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
            _spriteRenderer.material = ParamManager.Instance.LinkActiveMaterial;
        }
        else
        {
            if (HiveType == HiveController.Hive.Enemy)
            {
                _spriteRenderer.enabled = false;
                _boxCollider.enabled = false;
            }
            else
            {
                _spriteRenderer.material = ParamManager.Instance.LinkInactiveMaterial;
            }
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
