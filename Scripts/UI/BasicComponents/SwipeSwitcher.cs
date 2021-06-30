using UnityEngine;

public abstract class SwipeSwitcher : MonoBehaviour
{
    [SerializeField] protected AnimationCurve xSpeedCurve;
    
    [SerializeField] protected float acceleration = 0.02f;
    
    [SerializeField] [Range(0f,1f)] protected float magnetZone = 0.25f;

    private float _speed;

    private bool _moving;

    private float _relativePosition;

    protected bool _rightDirection = true;

    protected abstract float Width { get; }
    
    protected abstract bool Press { get; }


    protected void HandleSwap(float deltaPosition, bool rightDirection)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        if (_moving)
        {
            _speed = deltaPosition / Width / Time.deltaTime;
            _relativePosition += _speed * Time.deltaTime;
            ClampRelativePosition();
            
            HandleUpdateRelativePosition(_relativePosition);
        }
        else
        {
            _rightDirection = rightDirection;
            HandleStartSwitch(rightDirection);
            _moving = true;
        }
    }

    protected void Update()
    {
        if (!_moving) return;
        
        if (!Press)
        {
            _speed += CalculateAcceleration();
            _relativePosition += _speed * CalculateSpeedSlowDawnK() * Time.deltaTime;
            ClampRelativePosition();
            HandleUpdateRelativePosition(_relativePosition);
        }
    }

    protected float CalculateAcceleration()
    {
        if (_relativePosition > magnetZone)
        {
            return acceleration;
        }
        if (_relativePosition > 0f)
        {
            return -acceleration;
        }
        
        if (_relativePosition < -magnetZone )
        {
            return -acceleration;
        }
        
        if (_relativePosition < 0f)
        {
            return acceleration;
        }
        
        return 0f;
    }
    
    protected float CalculateSpeedSlowDawnK()
    {
        return xSpeedCurve.Evaluate(Mathf.Abs(_relativePosition));
    }

    protected void ClampRelativePosition()
    {
        if (_rightDirection)
        {
            if (_relativePosition > 1f)
            {
                _relativePosition = 0f;
                HandleEndSwitch();
                _moving = false;
            }
            else if(_relativePosition < 0f)
            {
                HandleCancelSwitch();
                _moving = false;
            }
        }
        else
        {
            if (_relativePosition < -1f)
            {
                _relativePosition = 0f;
                HandleEndSwitch();
                _moving = false;
            }
            else if(_relativePosition > 0f)
            {
                HandleCancelSwitch();
                _moving = false;
            }
        }
    }

    protected abstract void HandleStartSwitch(bool rightDirection);
    
    protected abstract void HandleEndSwitch();
    
    protected abstract void HandleCancelSwitch();
    
    protected abstract void HandleUpdateRelativePosition(float position);
}
