using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CollidersDebugView : MonoBehaviour
{
#if UNITY_EDITOR
	public bool enable = true;
	
	private Vector3[][] polygonPointList;
	
	private Vector3[][] capsulePointList; 
	
	private Vector3[][] circlePointList;
	
	private Vector3[][] edgePointList;
	
	private Vector3[][] boxPointList;
	
	private PolygonCollider2D[] polygonColliders2D;
	
	private CapsuleCollider2D[] capsuleColliders2D;
	
	private CircleCollider2D[] circleColliders2D;
	
	private EdgeCollider2D[] edgeColliders2D;

	private BoxCollider2D[] boxColliders2D;

	public Color polygonColour = Color.white;
	
	public Color capsuleColour = Color.red;
	
	public Color circleColour = Color.green;
	
	public Color edgeColour = Color.yellow;
	
	public Color boxColour = Color.blue;

	public float lineWidth = 4f;
	
	public int arcSegmentCount = 15;

	public bool liveUpdate = true;

	private void Awake()
	{
		if (!Application.isEditor && !liveUpdate)
		{
			polygonColliders2D = GetComponentsInChildren<PolygonCollider2D>();
			polygonPointList = new Vector3[polygonColliders2D.Length][];
			
			capsuleColliders2D = GetComponentsInChildren<CapsuleCollider2D>();
			capsulePointList = new Vector3[capsuleColliders2D.Length][];
			
			circleColliders2D = GetComponentsInChildren<CircleCollider2D>();
			circlePointList = new Vector3[circleColliders2D.Length][];
			
			edgeColliders2D = GetComponentsInChildren<EdgeCollider2D>();
			edgePointList = new Vector3[edgeColliders2D.Length][];
		
			boxColliders2D = GetComponentsInChildren<BoxCollider2D>();
			boxPointList = new Vector3[boxColliders2D.Length][];
		}
	}

	void OnDrawGizmos() 
	{
		if (!enable)
			return;
		if (Application.isEditor || liveUpdate)
		{
			polygonColliders2D = GetComponentsInChildren<PolygonCollider2D>();
			polygonPointList = new Vector3[polygonColliders2D.Length][];
			
			capsuleColliders2D = GetComponentsInChildren<CapsuleCollider2D>();
			capsulePointList = new Vector3[capsuleColliders2D.Length][];
			
			circleColliders2D = GetComponentsInChildren<CircleCollider2D>();
			circlePointList = new Vector3[circleColliders2D.Length][];
			
			edgeColliders2D = GetComponentsInChildren<EdgeCollider2D>();
			edgePointList = new Vector3[edgeColliders2D.Length][];
		
			boxColliders2D = GetComponentsInChildren<BoxCollider2D>();
			boxPointList = new Vector3[boxColliders2D.Length][];
		}

		for (int i = 0; i< polygonColliders2D.Length; i++) {
			PolygonCollider2D collider = polygonColliders2D [i];
			polygonPointList[i] = GetPolygonPoints (collider);
		}
			
		for (int i = 0; i< capsuleColliders2D.Length; i++) {
			CapsuleCollider2D collider = capsuleColliders2D [i];
			capsulePointList[i] = GetCapsuleCollider2DPoints(collider);
		}
			
		for (int i = 0; i< circleColliders2D.Length; i++) {
			CircleCollider2D collider = circleColliders2D [i];
			circlePointList[i] = GetCircleCollider2DPoints(collider, arcSegmentCount);
		}
		
		for (int i = 0; i< edgeColliders2D.Length; i++) {
			EdgeCollider2D collider = edgeColliders2D [i];
			edgePointList[i] = GetEdgeCollider2DPoints(collider);
		}

		for (int i = 0; i< boxColliders2D.Length; i++) {
			BoxCollider2D collider = boxColliders2D [i];
			boxPointList[i] = GetBoxCollider2DPoints(collider);
		}
		
		DrawPolygonGizmo (polygonPointList, polygonColour);
		DrawPolygonGizmo (capsulePointList, capsuleColour);
		DrawPolygonGizmo (circlePointList, circleColour);
		DrawPolygonGizmo (edgePointList, edgeColour);
		DrawPolygonGizmo (boxPointList, boxColour);
	}
    
    
    private void DrawPolygonGizmo(Vector3[][] colliderPoints, Color color)
    {
        for(var i = 0; i < colliderPoints.Length;i++)
        {
	        var points = colliderPoints[i];
			
            for(var k = 1; k < points.Length;k++)
            {
                var p1 = points[k-1];
                var p2 = points[k];
                Handles.DrawBezier(p1,p2,p1,p2, color,null, lineWidth);
            }
        }
    }
    
    private Vector3[] GetPolygonPoints (PolygonCollider2D collider)
    {
	    var points = new Vector3[collider.points.Length + 1];
	    for(int i = 0; i< collider.points.Length;i++)
	    {
		    var p = collider.points[i];
		    var point = collider.transform.TransformPoint(p.x+collider.offset.x,p.y+collider.offset.y,0);
		    points[i] = point;
	    }

	    points [collider.points.Length] = points [0];

	    return points;
    }
    
    Vector3[] GetBoxCollider2DPoints(BoxCollider2D collider)
    {
	    var scale = collider.size * 0.5f;
	    var points = new Vector3[5];

	    points[0] = collider.transform.TransformPoint(new Vector3(-scale.x+collider.offset.x,scale.y+collider.offset.y,0));
	    points[1] = collider.transform.TransformPoint(new Vector3(scale.x+collider.offset.x,scale.y+collider.offset.y,0));
	    points[2] = collider.transform.TransformPoint(new Vector3(scale.x+collider.offset.x,-scale.y+collider.offset.y,0));
	    points[3] = collider.transform.TransformPoint(new Vector3(-scale.x+collider.offset.x,-scale.y+collider.offset.y,0));
	    points[4] = points[0];

	    return points;
    }
    
    private Vector3[] GetEdgeCollider2DPoints(EdgeCollider2D collider)
    {
	    return collider.points.Select(v2 => collider.transform.TransformPoint(v2 + collider.offset)).ToArray();
    }
    
    Vector3[] GetCircleCollider2DPoints(CircleCollider2D collider, int segments)
    {
	    var radius = collider.radius;
	    var angle = collider.transform.rotation.z;
	    var segmentSize = 360f / segments;
	    var circlePoints = new Vector3[segments+1];
	    
	    for(var i = 0; i< segments;i++)
	    {
		    var p = collider.transform.TransformPoint(new Vector3(Mathf.Cos(Mathf.Deg2Rad*(i*segmentSize+angle))*radius+collider.offset.x,Mathf.Sin(Mathf.Deg2Rad*(i*segmentSize+angle))*radius+collider.offset.y));
		    circlePoints[i] = p;
	    }

	    circlePoints[segments] = circlePoints[1];
	    
	    return circlePoints;
    }
    
    private Vector3[] GetCapsuleCollider2DPoints(CapsuleCollider2D collider)
    {
	    var circlePoints = new List<Vector3>();

	    if (collider.direction == CapsuleDirection2D.Horizontal )
	    {
		    var x = Mathf.Clamp(collider.size.x,collider.size.y, collider.size.x);
		    circlePoints.AddRange(GetCapsule2DArcPoints(collider, collider.size.y/2f, arcSegmentCount, new Vector2(-x / 2f + collider.size.y / 2f, 0f), 180f, 90));
		    circlePoints.AddRange(GetCapsule2DArcPoints(collider, collider.size.y/2f, arcSegmentCount, new Vector2(x / 2f - collider.size.y / 2f, 0f), 180f, -90));
		    circlePoints.Add(circlePoints[0]);
	    }
	    else
	    {
		    var y = Mathf.Clamp(collider.size.y,collider.size.x, collider.size.y);
		    circlePoints.AddRange(GetCapsule2DArcPoints(collider, collider.size.x/2f, arcSegmentCount, new Vector2(0f, y / 2f - collider.size.x / 2f), 180f, 0));
		    circlePoints.AddRange(GetCapsule2DArcPoints(collider, collider.size.x/2f, arcSegmentCount, new Vector2(0f, -y / 2f + collider.size.x / 2f), 180f, 180));
		    circlePoints.Add(circlePoints[0]);
	    }
	    
	    return circlePoints.ToArray();
    }
    
    private List<Vector3> GetCapsule2DArcPoints(CapsuleCollider2D collider, float radius, int segments, Vector2 offset, float arcAngle, float startAngle)
    {
	    var angle = collider.transform.rotation.z + startAngle;
	    var segmentSize = arcAngle / segments;
	    var circlePoints = new List<Vector3>();
	    
	    for(int i = 0; i <= segments;i++)
	    {
		    var p = collider.transform.TransformPoint(new Vector3(Mathf.Cos(Mathf.Deg2Rad*(i*segmentSize+angle))*radius+collider.offset.x + offset.x,Mathf.Sin(Mathf.Deg2Rad*(i*segmentSize+angle))*radius+collider.offset.y + offset.y));
		    circlePoints.Add(p);
	    }

	    return circlePoints;
    }
#endif
}
