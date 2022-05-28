using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{

    private void OnMouseHover()
    {
        //if (EventSystem.current.IsPointerOverGameObject())
        //{

            //EventSystem.current.RaycastAll()
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag(ParamManager.Instance.PLANETTAG))
                {
                    Debug.Log("Planet");
                }
            }
        //}
    }

    public void OnMouseClicked()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
