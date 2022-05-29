using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;

public class PlayerInput : MonoBehaviour
{

    private void OnMouseHover()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = -5;
        Ray ray = new Ray(mousePosition, Vector3.forward);
        List<RaycastHit2D> hits = new List<RaycastHit2D>(Physics2D.GetRayIntersectionAll(ray));
        if(hits.Any(hit => hit.collider.CompareTag("FogMask"))){
            if(hits.FirstOrDefault(hit => hit.collider.CompareTag("Planet")))
            {
                Debug.Log("Planet");
            }
        }
    }

    public void OnMouseClicked()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OnMouseHover();
    }
}
