using System;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public event Action<Collision2D> OnFullBodyCollisionEnter2D = (c) => { };
    
    public event Action<ObstacleSettings, float> OnObstacleCollision = (obstacle, force) => { };
    
    public event Action<ObstacleSettings, float> OnGroundObstacleCollision = (obstacle, force) => { };
    
    public event Action<ObstacleSettings, float> OnGroundCollision = (obstacle, force) => { };
    
    public event Action<TriggerObstacle, float> OnTriggerObstacleEnter = (obstacle, force) => { };
    
    public event Action<TriggerObstacle> OnTriggerObstacleTick = obstacle => { };

    private bool _canHandle;

    private TriggerObstacle _lastTriggerObstacle;

    private bool _triggerEnterInFrame;
    
    private bool _triggerStayInFrame;

    
    private void Start()
    {
        GameState.OnStartFly += HandleStartFly;
        GameState.OnEndFly += HandleEndFly;
    }

    private void OnDestroy()
    {
        GameState.OnStartFly -= HandleStartFly;
        GameState.OnEndFly -= HandleEndFly;
    }

    private void FixedUpdate()
    {
        if (_triggerEnterInFrame && !_triggerStayInFrame)
        {
            OnTriggerObstacleEnter(_lastTriggerObstacle, GetComponentInChildren<Rigidbody2D>().velocity.magnitude);
        }
        
        _triggerEnterInFrame = false;
        _triggerStayInFrame = false;
    }

    public void SandCollisionEnter2D(Collision2D other)
    {
        if(!_canHandle)
            return;
        
        if (other == null)
        {
            return;
        }

        var obstacle = other.gameObject.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            if (!obstacle.Locked)
            {
                OnObstacleCollision?.Invoke(obstacle.Settings, other.relativeVelocity.magnitude);
                obstacle.Lock();
            }
        }
        
        OnFullBodyCollisionEnter2D(other);
    }

    public void SandGroundObstacleCollision(ObstacleSettings obstacleSettings, float velocity)
    {
        OnGroundObstacleCollision?.Invoke(obstacleSettings, velocity);
    }
    
    public void SandGroundCollision(ObstacleSettings obstacleSettings, float velocity)
    {
        OnGroundCollision?.Invoke(obstacleSettings, velocity);
    }

    public void SandTriggerEnter2D(Collider2D other)
    {
        if(!_canHandle)
            return;

        if (other == null)
        {
            return;
        }
        var triggerObstacle = other.GetComponent<TriggerObstacle>();
        if (triggerObstacle != null)
        {
            _lastTriggerObstacle = triggerObstacle;
            _triggerEnterInFrame = true;
        }
    }
    
    public void SandTriggerStay2D(Collider2D other)
    {
        if(!_canHandle)
            return;

        if (other == null)
        {
            return;
        }
        _triggerStayInFrame = true;
    }

    public void SandTriggerObstacleEnter(TriggerObstacle triggerObstacle, float velocity)
    {
        OnTriggerObstacleEnter(triggerObstacle, velocity);
    }
    
    public void SandTriggerObstacleTick(TriggerObstacle triggerObstacle)
    {
        OnTriggerObstacleTick.Invoke(triggerObstacle);
    }

    public void HandleEndFly()
    {
        _canHandle = false;
    }
    
    public void HandleStartFly()
    {
        _canHandle = true;
    }
}
