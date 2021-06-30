using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class TriggerObstacle : MonoBehaviour, IMakeSound
{
    [SerializeField] private string soundName;
    
    [SerializeField] private int soundNumber;
    
    [SerializeField] private float entryDamageK = 0f;
    
    [SerializeField] private int tickDamage;
    
    [SerializeField] private int tickCoins;
    
    [SerializeField] private float entryCoinsK;
    
    [SerializeField] private float interval;
    
    [SerializeField] private float maxTicks = 10;

    [ReadOnly] [SerializeField] private int ticks = 0;
    
    [SerializeField] private TriggerObstacleType triggerObstacleType = TriggerObstacleType.Ground;

    private bool _startedDelay;
    
    private int _bulletPartsNumberInTrigger = 0;

    private int BulletPartsNumberInTrigger
    {
        get => _bulletPartsNumberInTrigger;
        set => _bulletPartsNumberInTrigger = value < 0 ? 0 : value;
    }
    
    private bool BulletInTrigger => _bulletPartsNumberInTrigger > 0;

    public string SoundName => soundName;
    
    public int SoundsNumber => soundNumber;
    
    public int TickDamage => tickDamage;
    
    public float EntryDamageK => entryDamageK;
    
    public float EntryCoinsK => entryCoinsK;
    
    public int TickCoins => tickCoins;
    
    public TriggerObstacleType Type => triggerObstacleType;

    
    private void Start()
    {
        GameState.OnEndGame += ResetObstacle;
    }

    private void OnDestroy()
    {
        GameState.OnEndGame -= ResetObstacle;
    }

    private void ResetObstacle()
    {
        StopAllCoroutines();
        _bulletPartsNumberInTrigger = 0;
        ticks = 0;
        _startedDelay = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var breakablePart = other.GetComponent<BreakablePart>();
        
        if (breakablePart != null && breakablePart.active)
        {
            ++BulletPartsNumberInTrigger;
            if (BulletPartsNumberInTrigger == 1)
            {
                breakablePart.bulletHp.BodyPartImpactHandler.OnTriggerObstacleEnter(this);
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        var breakablePart = other.GetComponent<BreakablePart>();
        
        if (breakablePart != null && breakablePart.active)
        {
            if (_startedDelay || ticks >= maxTicks)
            {
                return;
            }
            _startedDelay = true;
            StartCoroutine(DelayDamage(breakablePart.bulletHp));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var breakablePart = other.GetComponent<BreakablePart>();
        if (breakablePart != null && breakablePart.active)
        {
            --BulletPartsNumberInTrigger;
            if (!BulletInTrigger)
            {
                ticks = 0;
                StopAllCoroutines();
                _startedDelay = false;
            }
        }
    }

    IEnumerator DelayDamage(BulletHP bulletHp)
    {
        var time = 0f;
        while (time < interval)
        {
            time += Time.fixedDeltaTime;
            if (!BulletInTrigger)
            {
                ticks = 0;
                _startedDelay = false;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        bulletHp.BodyPartImpactHandler.OnTriggerObstacleTick(this);
        ++ticks;
        _startedDelay = false;
    }
}

public enum TriggerObstacleType
{
    Ground, Air
}
