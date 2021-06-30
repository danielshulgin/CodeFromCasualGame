using UnityEngine;
using UnityEngine.Events;

public class TutorialForceAdjusterRealization : BasicForceAdjusterRealization
{
    public UnityEvent OnMaximumPointReachEvent;
    
    public UnityEvent OnCarouselStartTouchEvent;
    
    public UnityEvent OnCarouselEndTouchEvent;
    
    [SerializeField] private float forceReduceSpeed = .2f;
    
    [SerializeField] private float forceAddSpeed = .2f;
    
    [SerializeField] [Range(0f, 1f)] private float stopPoint = .6f;
    
    [SerializeField] [Range(0f, 1f)] private float force;
    
    private bool _press = false;
    
    private bool _frozenForce;

    public float StopPoint => stopPoint;

    public override float Force
    {
        get => force;

        protected set
        {
            force = Mathf.Clamp(value, 0f, stopPoint); 
            if (force >= (stopPoint - float.Epsilon))
            {
                OnMaximumPointReachEvent.Invoke();
                _frozenForce = true;
            }            
        }
    }
    
    
    public override void UpdateRealization()
    {
        if (!_frozenForce)
        {
            if (_press)
            {
                Force += Time.deltaTime * forceAddSpeed;
            }
            else
            {
                Force -= Time.deltaTime * forceReduceSpeed;
            }
        }
    }

    public override void CarouselStartTouch()
    {
        OnCarouselStartTouchEvent.Invoke();
        _press = true;
    }
    
    public override void CarouselEndTouch()
    {
        _press = false;
        if (!_frozenForce)
        {
            OnCarouselEndTouchEvent.Invoke();
        }
    }
    
    public override void ResetGame()
    {
        force = 0f;
        _frozenForce = false;
    }
}
