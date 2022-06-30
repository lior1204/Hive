using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MouseInteractable:MonoBehaviour
{
    protected bool isHovered=false;
    public bool isClicked { get; protected set; } = false;
    public virtual void HoverObject()//turn on hover
    {
        isHovered = true;
        UpdateColorAndHighlight();
    }
    public virtual void UnHoverObject()//turn off hover
    {
        isHovered = false;
        UpdateColorAndHighlight();
    }
    public virtual void ClickObject()//turn on click
    {
        isClicked = true;
        UpdateColorAndHighlight();
    }
    public virtual void UnClickObject()//turn off click
    {
        isClicked = false;
        UpdateColorAndHighlight();
    }
    public abstract void UpdateColorAndHighlight();//update planet collor based on hive and hishlight
    
}

