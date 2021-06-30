using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEditor;
using UnityEngine;

public class GroundCollision : MonoBehaviour
{
    [SerializeField] private Transform obstaclesParent;
    
    [SerializeField] private GroundObstacle defaultGroundObstacle;
    
    [SerializeField] private float y = -8;
    
    [SerializeField] private List<GroundObstacle> groundObstacles;
    
    
#if UNITY_EDITOR
    [ButtonMethod]
    public void AddNewObstacles()
    {
        groundObstacles = obstaclesParent.GetComponentsInChildren<GroundObstacle>().ToList();
        groundObstacles.Remove(defaultGroundObstacle);

        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }
#endif

    private void OnCollisionEnter2D(Collision2D other)
    {
        var bulletHp = other.gameObject.GetComponent<BulletHP>();
        if (bulletHp == null)
        {
            return;
        }

        var groundObstacle = GetCollisionObstacle(other.transform.position.x);
        var groundObstacleSettings = groundObstacle.GroundObstacleSettings;
        if (groundObstacle.Locked)
        {
            return;
        }
        
        var bodyPartImpactHandler = bulletHp.BodyPartImpactHandler;
        if (groundObstacle == defaultGroundObstacle)
        {
            bodyPartImpactHandler.SandGroundEnter(groundObstacleSettings);
        }
        else
        {
            bodyPartImpactHandler.SandGroundObstacleEnter(groundObstacleSettings);
        }

        ModifyBulletPhysicParams(other, groundObstacleSettings);
        groundObstacle.Lock();
    }

    private void ModifyBulletPhysicParams(Collision2D other, GroundObstacleSettings groundObstacleSettings)
    {
        var bulletRigidBody = other.gameObject.GetComponentInParent<Rigidbody2D>();
        var prevVelocity = bulletRigidBody.velocity;
        var newVelocity = new Vector2(groundObstacleSettings.velocityMultiplier.x * prevVelocity.x,
            groundObstacleSettings.velocityMultiplier.y * prevVelocity.y);

        if (groundObstacleSettings.enableMinVelocityConstrain)
        {
            newVelocity = new Vector2(
                Mathf.Sign(newVelocity.x) *
                Mathf.Clamp(newVelocity.x, groundObstacleSettings.minVelocity.x, float.MaxValue),
                Mathf.Sign(newVelocity.y) *
                Mathf.Clamp(newVelocity.y, groundObstacleSettings.minVelocity.y, float.MaxValue));
        }

        bulletRigidBody.velocity = newVelocity;
    }

    private GroundObstacle GetCollisionObstacle(float x)
    {
        foreach (var groundObstacle in groundObstacles)
        {
            if (groundObstacle.gameObject.activeSelf && groundObstacle.StartX < x && groundObstacle.EndX > x)
            {
                Debug.Log(groundObstacle.name);
                return groundObstacle;
            }
        }

        return defaultGroundObstacle;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (var groundObstacle in groundObstacles)
        {
            if(transform.parent != null)
                return;

            Handles.DrawBezier(new Vector2(groundObstacle.StartX, y), 
                new Vector2(groundObstacle.EndX, y),
                new Vector2(groundObstacle.StartX, y), 
                new Vector2(groundObstacle.EndX, y),
                groundObstacle.GroundObstacleSettings.debugLineColor,
                null, groundObstacle.GroundObstacleSettings.debugLineThickness);
        }
    }
#endif
}
