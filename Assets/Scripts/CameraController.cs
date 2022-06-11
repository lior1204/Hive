using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //zoom
    [SerializeField] [Range(3, 8)] private float camStartingZoom = 5;
    [SerializeField] [Range(1, 5)] private float maxZoomIn = 2;
    [SerializeField] [Range(8, 15)] private float maxZoomOut = 10;
    [SerializeField] [Range(0.01f, 1f)] private float zoomIncrement = 1f;
    private Camera cam;
    //pan
    private bool isPanPressed = false;
    private Vector2 mouseOrigin;

    //Auto Move
    [SerializeField] private float camSpeed = 0.02f;
    [SerializeField] [Range(0f, 1f)] private float autoCameraEdgeDistance = 0.2f;

    private void Start()
    {
        cam = Camera.main;
        cam.orthographicSize = camStartingZoom;
    }
    private void Update()
    {
        if (!GameManager.Instance.IsPaused)//check if game is playing
        {
            PanCamera();
            AutoMoveCamera();
        }
    }

    public void OnZoom(InputAction.CallbackContext context)//zoom in and out
    {
        if (context.ReadValue<float>() > 0)//zoom in
        {
            if (cam.orthographicSize > maxZoomIn)
                cam.orthographicSize -= zoomIncrement;
        }
        else if (context.ReadValue<float>() < 0)//zoom out
        {
            if (cam.orthographicSize < maxZoomOut)
                cam.orthographicSize += zoomIncrement;
        }
    }
    public void OnHoldPan(InputAction.CallbackContext context)//turn on and off paning
    {
        if (context.performed)//turn on pan when clicked
        {
            isPanPressed = true;
            mouseOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());//save current mouse position
        }
        if (context.canceled)//turn off pan when released
        {
            isPanPressed = false;
        }
    }

    private void PanCamera()//id panning move camera equal to delta from starting pan
    {
        if (isPanPressed)
        {
            Vector3 deltaMousePosition = mouseOrigin - (Vector2)cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());//caculate delta
            cam.transform.position += deltaMousePosition;
        }
    }
    private void AutoMoveCamera()//auto move the camera when the mouse is by the edge of the screen
    {
        if (!isPanPressed)
        {
            Vector2 mouseScreenPosition = cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());//mouse position on screen
            Vector3 camDelta = Vector3.zero;//delta movment based on what edge
            if (mouseScreenPosition.x <= autoCameraEdgeDistance)
                camDelta.x = -camSpeed;
            if (mouseScreenPosition.x >= 1 - autoCameraEdgeDistance)
                camDelta.x = camSpeed;
            if (mouseScreenPosition.y <= autoCameraEdgeDistance)
                camDelta.y = -camSpeed;
            if (mouseScreenPosition.y >= 1 - autoCameraEdgeDistance)
                camDelta.y = camSpeed;
            cam.transform.position += camDelta*Time.deltaTime;
        }
    }
}
