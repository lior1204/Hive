using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer))]
public class OrbitalMovement : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] [Range(12,36)]private int resolution = 24;
    [SerializeField] private Elipse elipse;
    [SerializeField] private bool isActive = true;

    private float cycleTime;
    private Transform orbitingTransform;
    private Planet orbitingObject;
    private float cycleProgress = 0;
    private float cycleFrequency;
    private void Start()
    {
        transform.localPosition = Vector3.zero;
        orbitingTransform = GetComponentInChildren<Transform>();
        orbitingObject = orbitingTransform.GetComponent<Planet>();
        Debug.Log(orbitingObject != null);
        if (orbitingObject != null)
        {
            if (Mathf.Abs(orbitingObject.GetOrbitCycleTime()) > 0.5)
                cycleFrequency = 1 / orbitingObject.GetOrbitCycleTime();
            else
                cycleFrequency = 1 / 0.5f;
        }
        else cycleFrequency = 0.2f;
        Debug.Log(cycleFrequency);
        SetIntoOrbitPosition();
    }
    private void Update()
    {
        if (isActive && !GameManager.Instance.IsPaused)
        {
            cycleProgress += Time.deltaTime*cycleFrequency;
            cycleProgress %= 1;
            SetIntoOrbitPosition();
        }
    }
    private void SetIntoOrbitPosition()
    {
        orbitingTransform.localPosition = elipse.GetPositionOnElipse(cycleProgress);
    }
    
    
    //draw
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }
    public void DrawElipse()
    {
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.loop = true;
        Vector3[] points = new Vector3[resolution];
        for(int i=0; i < resolution; i++)
        {
            Vector2 point = elipse.GetPositionOnElipse((float)i / resolution);
            points[i] = new Vector3( point.x,point.y,0)+transform.position;
        }
        _lineRenderer.positionCount = resolution;
        _lineRenderer.SetPositions(points);
    }
    //private void OnValidate()
    //{
    //    if (_lineRenderer)
    //        DrawElipse();
    //}

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
