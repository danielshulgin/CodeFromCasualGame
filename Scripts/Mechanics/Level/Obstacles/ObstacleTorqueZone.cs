using UnityEngine;


public class ObstacleTorqueZone : MonoBehaviour
{
    [SerializeField] private float torque = 0;

    private bool _calculatedInCurrentFrame;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_calculatedInCurrentFrame)
            return;

        var bulletRigidBody = other.GetComponentInParent<Rigidbody2D>();
        if (bulletRigidBody == null)
        {
            return;
        }

        var flyingToRight = bulletRigidBody.angularVelocity > 0 ? true : false;
        
        bulletRigidBody.AddTorque(flyingToRight ? torque : (torque * -1));

        _calculatedInCurrentFrame = true;
    }

    private void FixedUpdate()
    {
        _calculatedInCurrentFrame = false;
    }
}
