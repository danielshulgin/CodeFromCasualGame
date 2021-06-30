using System;
using System.Collections;
using Generic;
using UnityEngine;
using Zenject;

public class BulletFly : MonoBehaviour
{
    [SerializeField] private RotateCarousel rotateCarousel;

    [SerializeField] private GameState gameState;

    [SerializeField] private ForceAdjuster forceAdjuster;
        
    [SerializeField] private float force = 10f;
    
    [SerializeField] private float torque = 0;
    
    [SerializeField] private AnimationCurve realForceRelativeForceCurve;
    
    [SerializeField] private AnimationCurve torqueRelativeForceCurve;

    [SerializeField] private AnimationCurve angleCurve;
    
    [SerializeField] private float minStartAngle = 30f;
    
    [SerializeField] private float maxStartAngle = 60f;
    
    [SerializeField] private float endOfFlyDelay = 2.5f;
    
    [SerializeField] private float endOfFlyCheckVelocity = 1f;

    [SerializeField] private float endOfFlyConditionPositionDelta = 1f;
    
    [SerializeField] private int discretizationLevels = 10;
        
    [Inject] private PlayerBulletSkins _playerBulletSkins;

    private Rigidbody2D _bulletRigidbody2D;
    
    private bool _endOfFlyDelayRoutineStarted;
   
    public bool Fly { get; private set; }

    private void Awake()
    {
        BulletSpawner.OnChangeBullet += HandleChangeBullet;
    }

    private void Start()
    {
        GameState.OnStartFly += HandleStartFly;
        GameState.OnEndFly += HandleEndFly;
        GameState.OnResetGame += HandleResetGame;
        _bulletRigidbody2D = GetComponentInChildren<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        BulletSpawner.OnChangeBullet -= HandleChangeBullet;
        
        GameState.OnStartFly -= HandleStartFly;
        GameState.OnEndFly -= HandleEndFly;
        GameState.OnResetGame -= HandleResetGame;
    }


    private void Update()
    {
        if (IsEndOfFlyCheckCondition() && !_endOfFlyDelayRoutineStarted)
        {
            StartCoroutine(EndOfFlyDelayRoutine());
        }
    }

    public void HandleStartFly()
    {
        //TODO 
        transform.GetChild(0).gameObject.SetActive(true);
        _bulletRigidbody2D = GetComponentInChildren<BulletParts>().pivotRigidbody2D;
        var direction = CalculateDirection();
        _bulletRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _bulletRigidbody2D.AddForce(force * realForceRelativeForceCurve.Evaluate(forceAdjuster.Force) * direction);
        _bulletRigidbody2D.AddTorque(torque * torqueRelativeForceCurve.Evaluate(forceAdjuster.Force) 
                                      * (rotateCarousel.RightFlyDirection ? 1 : -1));
        Fly = true;
        _endOfFlyDelayRoutineStarted = false;
    }
    
    
    public Vector2 CalculateDirection()
    {
        float angleCurveX;
        if (rotateCarousel.RelativePosition < 0.25f)
        {
            angleCurveX = 0.5f + 2f * rotateCarousel.RelativePosition;
        } else if (rotateCarousel.RelativePosition < 0.75f)
        {
            angleCurveX = 2f * (rotateCarousel.RelativePosition - 0.25f);
        }
        else
        {
            angleCurveX = 2f * (rotateCarousel.RelativePosition - 0.75f);
        }
        
        var angle = minStartAngle +
                    angleCurve.Evaluate(DiscretizeAngleCurveX(angleCurveX)) * (maxStartAngle - minStartAngle);
        
        if (!rotateCarousel.RightFlyDirection)
        {
            angle = 180 - angle;
        }
        Debug.Log("start angle: " + angle);
        return HelperFunctions.DegreeToVector2(angle);
    }
    
    private float DiscretizeAngleCurveX(float value)
    {
        return Mathf.RoundToInt(value * discretizationLevels) / (float)discretizationLevels; 
    }

    public void HandleResetGame()
    {
        Fly = false;
    }
    
    public void HandleChangeBullet(GameObject bullet)
    {
        Fly = false;
    }
    
    private IEnumerator EndOfFlyDelayRoutine()
    {
        var startPosition = _bulletRigidbody2D.transform.position;
        
        _endOfFlyDelayRoutineStarted = true;
        yield return new WaitForSeconds(endOfFlyDelay);
        _endOfFlyDelayRoutineStarted = false;
        
        var endPosition = _bulletRigidbody2D.transform.position;
        if (Vector3.Distance(startPosition, endPosition) < endOfFlyConditionPositionDelta)
        {
            gameState.SendEndFly();
            gameState.SendEndGame();
            gameState.SendShowResults();
        }
    }

    private bool IsEndOfFlyCheckCondition()
    {
        return Fly && (_bulletRigidbody2D.velocity.magnitude < endOfFlyCheckVelocity);
    }

    public void HandleEndFly()
    {
        Fly = false;
        StopAllCoroutines();
        if (_bulletRigidbody2D != null)//TODO
        {
            _bulletRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _bulletRigidbody2D.velocity = Vector2.zero;
            _bulletRigidbody2D.angularVelocity = 0f;
        }
    }
}
