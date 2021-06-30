using UnityEngine;

public class PhysicVelocityConstrain : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject;
    
    [SerializeField] private float maxXSpeed = 25f;
    
    [SerializeField] private float maxYSpeed = 18f;
    
    [SerializeField] private float maxAngularSpeed = 200f;

    private Rigidbody2D _rigidBody2D;
    
    private BulletCollision _bulletCollision;


    private void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ClampVelocity();
        ClampAngularVelocity();
    }

    private void ClampVelocity()
    {
        var clampedXSpeed = Mathf.Clamp(_rigidBody2D.velocity.x, -maxXSpeed, maxXSpeed);
        var clampedYSpeed = Mathf.Clamp(_rigidBody2D.velocity.y, -maxYSpeed, maxYSpeed);
        _rigidBody2D.velocity = new Vector2(clampedXSpeed, clampedYSpeed);
    }
    
    private void ClampAngularVelocity()
    {
        var clampedAngularVelocity = Mathf.Clamp(_rigidBody2D.angularVelocity, -maxAngularSpeed, maxAngularSpeed);
        _rigidBody2D.angularVelocity = clampedAngularVelocity;
    }
}
