using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MouseInteractable:MonoBehaviour
{
    protected bool isHovered=false;
    protected bool isClicked=false;
    public virtual void HoverObject()//turn on hover
    {
        isHovered = true;
    }
    public virtual void UnHoverObject()//turn off hover
    {
        isHovered = false;
    }
    public void ClickObject()//turn on click
    {
        isHovered = true;
    }
    public virtual void UnClickObject()//turn off click
    {
        isHovered = false;
    }
    
}

