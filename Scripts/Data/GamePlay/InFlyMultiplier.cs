using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class InFlyMultiplier : MonoBehaviour
{
    public Action<float> OnChangeInFlyMultiplier;
    
    public Action<float> OnChangeBaseInFlyMultiplier;
    
    public Action<float> OnResetInFlyMultiplier;

    [SerializeField] private BulletCollision bulletCollision;

    [SerializeField] private float inFlyMultiplierResetTime = 1.5f;

    public float Value => ComboNumber * BaseValue;
    
    public float BaseValue { get; private set; } = 1f;
    
    public int ComboNumber { get; private set; } = 1;

    private Coroutine _resetInFlyMultiplierRoutine;
    
    public float InFlyMultiplierResetTime => inFlyMultiplierResetTime;

    
    private void Awake()
    {
        BulletSpawner.OnChangeBullet += HandleChangeBullet;
    }

    private void Start()
    {
        bulletCollision.OnGroundObstacleCollision += HandleGroundObstacleCollision;
        bulletCollision.OnTriggerObstacleEnter += HandleTriggerObstacleEnter;
        bulletCollision.OnObstacleCollision += HandleObstacleCollision;
        
        GameState.OnEndFly += HandleEndFly;
    }

    private void HandleEndFly()
    {
        ComboNumber = 1;
        BaseValue = 1f;
    }

    private void HandleObstacleCollision(ObstacleSettings obstacleSettings, float velocity)
    {
        SetComboNumber(ComboNumber + 1);
    }
    
    private void HandleGroundObstacleCollision(ObstacleSettings obstacleSettings, float velocity)
    {
        SetComboNumber(ComboNumber + 1);
    }

    private void HandleTriggerObstacleEnter(TriggerObstacle triggerObstacle, float velocity)
    {
        if (triggerObstacle.Type == TriggerObstacleType.Ground)
        {
            return;
        }
        SetComboNumber(ComboNumber + 1);
    }
    
    private void SetComboNumber(int newValue)
    {
        ComboNumber = newValue;
        if (_resetInFlyMultiplierRoutine != null)
        {
            StopCoroutine(_resetInFlyMultiplierRoutine);
        }

        _resetInFlyMultiplierRoutine = StartCoroutine(ResetComboNumber());
        OnChangeInFlyMultiplier?.Invoke(Value);
    }
    
    private void SetBaseValue(float newValue)
    {
        BaseValue = newValue;
        if (_resetInFlyMultiplierRoutine != null)
        {
            StopCoroutine(_resetInFlyMultiplierRoutine);
        }

        _resetInFlyMultiplierRoutine = StartCoroutine(ResetComboNumber());
        OnChangeBaseInFlyMultiplier?.Invoke(Value);
    }
    
    IEnumerator ResetComboNumber()
    {
        yield return new WaitForSeconds(inFlyMultiplierResetTime);
        ComboNumber = 1;
        OnResetInFlyMultiplier?.Invoke(Value);
    }

    private void HandleChangeBullet(GameObject bullet)
    {
        var bulletHp = bullet.GetComponent<BulletHP>();
        bulletHp.OnBreakPart += HandleBreakBulletPart;
    }

    private void HandleBreakBulletPart(BreakablePart breakablePart)
    {
        SetBaseValue(breakablePart.baseInFlyMultiplier);
    }
}
