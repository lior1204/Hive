using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class PlayerInput : MonoBehaviour
{
    private MouseInteractable currentHover;
    private Planet currentClickedPlanet;
    private void CheckforMouseHover()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());//get mouse position
        mousePosition.z = -5;
        Ray ray = new Ray(mousePosition, Vector3.forward);
        List<RaycastHit2D> hits = new List<RaycastHit2D>(Physics2D.GetRayIntersectionAll(ray));//raycast to hit planets
        if (hits.Any(hit => hit.collider.CompareTag(ParamManager.Instance.FOGMASKTAG)))//check if planet is inside visability fog mask
        {
            RaycastHit2D hover = (hits.FirstOrDefault(hit => hit.collider.GetComponent<MouseInteractable>() != null));//check if hit IMouseHover
            if (hover)//if not null trigger mouseHover
            {
                OnMouseHover(hover.collider.GetComponent<MouseInteractable>());
                return;
            }
        }
        OnMouseHover(null);//if not inside mask or not on IMouseHover remove last hover
    }
    private void OnMouseHover(MouseInteractable hover)
    {
        if (hover != currentHover)//if hover something other then current or hover null
        {
            if (currentHover&&currentHover!=currentClickedPlanet)//if previously hover something unhover
                currentHover.UnHoverObject();
            if (hover && IsValidHover(hover)) //if planet isnt null and is valid
            {
                currentHover = hover;//set current to new hover or null
                currentHover.HoverObject();
            }
            else
                currentHover = null;
        }
    }

    public void OnMouseClicked()//
    {
        if (currentClickedPlanet)//if already have clicked planet
        {
            if (!currentHover)//if not hovering something unClick planet
            {
                currentClickedPlanet.RemoveHighlightPlanet();
                currentClickedPlanet = null;
            }
            else//if hovering something do function
            {
                if (currentHover == currentClickedPlanet)
                {
                    currentClickedPlanet.RemoveHighlightPlanet();
                    currentClickedPlanet = null;
                }
                else if (currentHover.HiveType != HiveController.Hive.Player)// if hovering non-player planet start capture
                    HiveController.Player.CapturePlanet(currentClickedPlanet, currentHover);
                else
                { //TODO link - in future
                }
            }
        }
        else //if not already clicked make hovered clicked
        {
            currentClickedPlanet = currentHover;
            currentHover = null;
        }
    }

    private bool IsValidHover(MouseInteractable hover)//check if hovering over something that is a valid hover based on current clicked
    {
        if (hover != currentHover)//if different from current hover
        {
            if (currentClickedPlanet)//if there is clicked planet
            {
                if (currentClickedPlanet.IsCapturable(hover))//allow hover planets inside range
                {
                    return true;
                }
            }
            else if (hover.HiveType == HiveController.Hive.Player)//if there is no clicked then allow hover on player planets
            {
                return true;
            }
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        CheckforMouseHover();
    }
}
