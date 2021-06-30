using System;
using UnityEditor;
using UnityEngine;

public class GroundObstacle : Obstacle
{
    [SerializeField] private GroundObstacleSettings groundObstacleSettings;
    
    public GroundObstacleSettings GroundObstacleSettings => groundObstacleSettings;

    public override ObstacleSettings Settings => groundObstacleSettings;

    public float StartX => transform.position.x - groundObstacleSettings.width / 2f + groundObstacleSettings.offset;
    
    public float EndX => transform.position.x + groundObstacleSettings.width / 2f + groundObstacleSettings.offset;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(transform.parent != null || groundObstacleSettings == null)
            return;

        Handles.DrawBezier(new Vector2(StartX, 0f), new Vector2(EndX, 0f),
            new Vector2(StartX, 0f), new Vector2(EndX, 0f),
            groundObstacleSettings.debugLineColor,null, groundObstacleSettings.debugLineThickness);
    }
#endif
}
