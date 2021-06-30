using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartImpactHandler : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject;

    private BulletCollision _bulletCollision;
    
    private Rigidbody2D _bulletRigidBody2D;

    public void Initialize()
    {
        _bulletCollision = rootGameObject.GetComponentInParent<BulletCollision>();
        _bulletRigidBody2D = GetComponent<Rigidbody2D>();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_bulletCollision != null)
        {
            _bulletCollision.SandCollisionEnter2D(other);
        }
    }

    public void SandGroundObstacleEnter(GroundObstacleSettings groundObstacleSettings)
    {
        _bulletCollision.SandGroundObstacleCollision(groundObstacleSettings, _bulletRigidBody2D.velocity.magnitude);
    }
    
    public void SandGroundEnter(GroundObstacleSettings groundObstacleSettings)
    {
        _bulletCollision.SandGroundCollision(groundObstacleSettings, _bulletRigidBody2D.velocity.magnitude);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (_bulletCollision != null)
        {
            _bulletCollision.SandTriggerEnter2D(other);
        }
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        if (_bulletCollision != null)
        {
            _bulletCollision.SandTriggerStay2D(other);
        }
    }

    public void OnTriggerObstacleEnter(TriggerObstacle triggerObstacle)
    {
        _bulletCollision.SandTriggerObstacleEnter(triggerObstacle, _bulletRigidBody2D.velocity.magnitude);
    }
    
    public void OnTriggerObstacleTick(TriggerObstacle triggerObstacle)
    {
        _bulletCollision.SandTriggerObstacleTick(triggerObstacle);
    }
}