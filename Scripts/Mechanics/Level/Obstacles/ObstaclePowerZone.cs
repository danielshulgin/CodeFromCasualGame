using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;


public class ObstaclePowerZone : MonoBehaviour
{
    [SerializeField][Range(-100, 100)] private float xVelocityK;
    
    [SerializeField][Range(-100, 100)] private float yVelocityK;

    [SerializeField] private Vector2 constantForce = Vector2.zero;
    
    private bool _calculatedInCurrentFrame;

    
    private void OnTriggerStay2D(Collider2D other)
    {
        if(_calculatedInCurrentFrame)
            return;

        var bulletRigidBody = other.GetComponentInParent<Rigidbody2D>();
        if (bulletRigidBody == null)
        {
            return;
        }
        var prevVelocity = bulletRigidBody.velocity;
        
        var velocityMultiplier = new Vector2(1f + 0.001f * xVelocityK, 1f + 0.001f * yVelocityK);
        
        bulletRigidBody.velocity = new Vector2(velocityMultiplier.x * prevVelocity.x,  velocityMultiplier.y * prevVelocity.y)
            + new Vector2(Mathf.Sign(prevVelocity.x) * constantForce.x,  constantForce.y);
        
        _calculatedInCurrentFrame = true;
    }

    private void FixedUpdate()
    {
        _calculatedInCurrentFrame = false;
    }
}
