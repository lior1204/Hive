using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class PlayerInput : MonoBehaviour
{
    private Planet currentHoverPlanet;
    private Planet currentClickedPlanet;
    private void CheckforMouseHover()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());//get mouse position
        mousePosition.z = -5;
        Ray ray = new Ray(mousePosition, Vector3.forward);
        List<RaycastHit2D> hits = new List<RaycastHit2D>(Physics2D.GetRayIntersectionAll(ray));//raycast to hit planets
        if (hits.Any(hit => hit.collider.CompareTag(ParamManager.Instance.FOGMASKTAG)))//check if planet is inside visability fog mask
        {
            RaycastHit2D h;
            if (h = (hits.FirstOrDefault(hit => hit.collider.CompareTag(ParamManager.Instance.PLANETTAG))))//check if hit planet
            {
                OnHoverPlanet(h.collider.GetComponent<Planet>());
            }
            else
            {
                OnHoverPlanet(null);//if not hover planet remove last hovered
            }
        }
        else
        {
            OnHoverPlanet(null);//if not inside mask remove last hover
        }
    }
    private void OnHoverPlanet(Planet planet)
    {
        if (planet != currentHoverPlanet)//if hover something other then current or hover null
        {
            if (currentHoverPlanet&&currentHoverPlanet!=currentClickedPlanet)//if previously hover something unhover
                currentHoverPlanet.RemoveHighlightPlanet();
            if (planet && IsValidHover(planet)) //if planet isnt null and is valid
            {
                currentHoverPlanet = planet;//set current to new hover or null
                currentHoverPlanet.HighlightPlanet();
            }
            else
                currentHoverPlanet = null;
        }
    }

    public void OnMouseClicked()//
    {
        if (currentClickedPlanet)//if already have clicked planet
        {
            if (!currentHoverPlanet)//if not hovering something unClick planet
            {
                currentClickedPlanet.RemoveHighlightPlanet();
                currentClickedPlanet = null;
            }
            else//if hovering something do function
            {
                if (currentHoverPlanet == currentClickedPlanet)
                {
                    currentClickedPlanet.RemoveHighlightPlanet();
                    currentClickedPlanet = null;
                }
                else if (currentHoverPlanet.HiveType != HiveController.Hive.Player)// if hovering non-player planet start capture
                    HiveController.Player.CapturePlanet(currentClickedPlanet, currentHoverPlanet);
                else
                { //TODO link - in future
                }
            }
        }
        else //if not already clicked make hovered clicked
        {
            currentClickedPlanet = currentHoverPlanet;
            currentHoverPlanet = null;
        }
    }

    private bool IsValidHover(Planet planet)//check if hovering over something that is a valid hover based on current clicked
    {
        if (planet != currentHoverPlanet)
        {
            //if (currentClickedPlanet)
            //{
            //    if (planet.HiveType)
            //}
            //else if (planet.HiveType == HiveController.Hive.Player)
            //{
            //    return true;
            //}
            return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        CheckforMouseHover();
    }
}
