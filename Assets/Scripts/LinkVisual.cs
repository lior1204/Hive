using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class LinkVisual : MouseInteractable
{
    private EdgeCollider2D edgeCollider;
    private LineRenderer myLine;
    public Link ParetntLink { get; private set; }
    public HiveController.Hive HiveType { get { return ParetntLink.Origin? ParetntLink.Origin.HiveType: HiveController.Hive.Neutral; } }
    void Start()
    {
        edgeCollider = this.GetComponent<EdgeCollider2D>();
        myLine = this.GetComponent<LineRenderer>();
    }
    void Update()
    {
        SetEdgeCollider(myLine);
    }

    public override void HoverObject()
    {
        //if (HiveRef)
        //    _spriteRenderer.color = HiveRef.HiveHighlightColor;
        //else
        //    _spriteRenderer.color = ParamManager.Instance.NeutralHighlightColor;
    }
    public override void UnHoverObject()
    {
        //if (HiveRef)
        //    _spriteRenderer.color = HiveRef.HiveColor;
        //else
        //    _spriteRenderer.color = ParamManager.Instance.NeutralColor;
    }
    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        edgeCollider.points=edges.ToArray();
    }
}