using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ObstacleColliderModifier : MonoBehaviour
{
    public Vector2 velocityMultiplier = new Vector2(1f, 1f);

    public bool enableMinVelocityConstrain;    
    
    [MyBox.ConditionalField(nameof(enableMinVelocityConstrain))]
    public Vector2 minVelocity = Vector2.zero; 

    private void OnCollisionEnter2D(Collision2D other)
    {
        var bulletRigidBody = other.gameObject.GetComponentInParent<Rigidbody2D>();
        if (bulletRigidBody == null)
        {
            return;
        }
        
        var prevVelocity = bulletRigidBody.velocity;
        var newVelocity = new Vector2(velocityMultiplier.x * prevVelocity.x, velocityMultiplier.y * prevVelocity.y);
        
        if (enableMinVelocityConstrain)
        {
            newVelocity = new Vector2(Mathf.Sign(newVelocity.x) * Mathf.Clamp(newVelocity.x, minVelocity.x, float.MaxValue),
                                      Mathf.Sign(newVelocity.y) * Mathf.Clamp(newVelocity.y, minVelocity.y, float.MaxValue));
        }

        bulletRigidBody.velocity = newVelocity;
    }
}
       