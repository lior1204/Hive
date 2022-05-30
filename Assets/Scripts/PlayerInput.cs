using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;

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
        if (hits.Any(hit => hit.collider.CompareTag(ParamManager.Instance.PLANETTAG)))//check if planet is inside visability fog mask
        {
            RaycastHit2D h;
            if ( h= (hits.FirstOrDefault(hit => hit.collider.CompareTag(ParamManager.Instance.FOGMASKTAG))))//check if hit planet
            {
                OnHoverPlanet(h.collider.GetComponent<Planet>());
            }
        }
    }
    private void OnHoverPlanet(Planet planet)
    {
        if (planet != currentHoverPlanet)
        {

        }
    }
    public void OnMouseClicked()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckforMouseHover();
    }
}
