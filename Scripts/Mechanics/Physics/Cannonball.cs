using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Cannonball : MonoBehaviour
{
    public Action<Vector2> OnCollisionWithBullet;
    
    public Action<Vector2> OnEndFly;

    private bool _handleCollision;

    private Coroutine _cannonBallMoveRoutine;

    private Vector2 _velocity;
    
    private float _gravity;
    
    private float _maxBounceFlyTime;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_handleCollision)
        {
            var collisionDirection = (transform.position - other.transform.position).normalized;
            OnCollisionWithBullet?.Invoke(collisionDirection);
            
            StopCoroutine(_cannonBallMoveRoutine);
            
            _velocity = _velocity.magnitude * collisionDirection; 
            
            _cannonBallMoveRoutine = StartCoroutine(MoveCannonBallRoutine(
                new Vector2(transform.position.x - Camera.main.transform.position.x, transform.position.y), _maxBounceFlyTime, true));
        }
    }

    public void StartMove(Vector2 startOffset,  Vector2 velocity, float gravity, float flyTime, bool handleCollision, float maxBounceFlyTime = 0.5f)
    {
        _velocity = velocity;
        _gravity = gravity;
        _handleCollision = handleCollision;
        _maxBounceFlyTime = maxBounceFlyTime;
        
        _cannonBallMoveRoutine = StartCoroutine(MoveCannonBallRoutine(startOffset, flyTime, true));
    }
    
    IEnumerator MoveCannonBallRoutine(Vector2 startOffset, float flyTime, bool destroyInTheEnd = false)
    {
        var translation = Vector2.zero;
        for (var time = 0f; time < flyTime; time += Time.fixedDeltaTime)
        {
            _velocity += new Vector2(0f, _gravity * Time.fixedDeltaTime);
            translation += _velocity * Time.fixedDeltaTime;
            
            var position = new Vector2(Camera.main.transform.position.x + startOffset.x + translation.x,
                startOffset.y + translation.y);
            
            transform.position = position;
            yield return new WaitForFixedUpdate();
        }

        if (destroyInTheEnd)
        {
            OnEndFly?.Invoke(transform.position);
            Destroy(gameObject);
        }
    }
}
