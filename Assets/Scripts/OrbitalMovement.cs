using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class OrbitalMovement : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Planet orbitingObject;
    [SerializeField] private Elipse elipse;
    [SerializeField] private bool isActive = true;
    [SerializeField][Range(0,1f)] private float startingPosition = 0;
    [SerializeField] [Range(12, 36)] private int resolution = 24;

    private float cycleProgress = 0;
    private float cycleFrequency;
    private void Start()
    {
        if (Application.isPlaying)
        {
            orbitingObject = GetComponentInChildren<Planet>();
            if (transform.parent)//if parent then set the orbit position to the parent
                transform.localPosition = Vector3.zero;
            if (orbitingObject != null)//set cycle frequency for the planet
            {
                if (Mathf.Abs(orbitingObject.GetOrbitCycleTime()) > 0.5)
                    cycleFrequency = 1 / orbitingObject.GetOrbitCycleTime();
                else
                    cycleFrequency = 1 / 0.5f;
            }
            else cycleFrequency = 0.2f;
            cycleProgress = startingPosition;//set starting position
            SetIntoOrbitPosition();
        }
    }
    private void Update()
    {
        if (Application.isPlaying)
        {
            if (isActive && !GameManager.Instance.IsPaused)//planet is active and game isnt paused
            {
                cycleProgress += Time.deltaTime * cycleFrequency;//increase time
                cycleProgress %= 1;
                SetIntoOrbitPosition();
            }
        }
        else
        {
            UpdateVisualInEditor();
        }
    }
    private void SetIntoOrbitPosition()
    {
        if(orbitingObject)
        orbitingObject.transform.localPosition = elipse.GetPositionOnElipse(cycleProgress);
    }


    //draw elipse
    private void Awake()
    {
        if (Application.isPlaying)
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
        }
    }
    public void DrawElipse()
    {
        if (_lineRenderer)
        {
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.loop = true;
            Vector3[] points = new Vector3[resolution];
            for (int i = 0; i < resolution; i++)
            {
                Vector2 point = elipse.GetPositionOnElipse((float)i / resolution);
                points[i] = new Vector3(point.x, point.y, 0) + transform.position;
            }
            _lineRenderer.positionCount = resolution;
            _lineRenderer.SetPositions(points);
        }
    }
    private void UpdateVisualInEditor()
    {
        if (transform.parent)//if parent then set the orbit position to the parent
            transform.localPosition = Vector3.zero;
        DrawElipse();
        cycleProgress = startingPosition;
        SetIntoOrbitPosition();
    }

    [Serializable]
    public class Elipse
    {
        public float xAxis = 3;
        public float yAxis = 3;
        public Elipse (float xAxis,float yAxis)
        {
            this.xAxis = xAxis;
            this.yAxis = yAxis;
        }
        public Vector2 GetPositionOnElipse(float t)//get position on the elipse with t between 0 and 1
        {
            float angle = Mathf.Deg2Rad * 360f * t;
            float x = Mathf.Sin(angle) * xAxis;
            float y = Mathf.Cos(angle) * yAxis;
            return new Vector2(x, y);
        }
    }
}
