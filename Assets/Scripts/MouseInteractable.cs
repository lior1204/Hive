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
    public abstract void HoverObject();
    public abstract void UnHoverObject();
    
}

