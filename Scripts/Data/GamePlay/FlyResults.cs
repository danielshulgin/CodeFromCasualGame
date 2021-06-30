using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Zenject;

public class FlyResults : MonoBehaviour
{
     public static event Action<Currency> OnUpdateResult;
     
     public static event Action<Currency> OnFinalResult;
     
     public static event Action<Currency> OnAddDieReward;

     public static event Action OnChangeResults;

     [SerializeField] private BulletCollision bulletCollision;
     
     [SerializeField] private BulletDieHandler bulletDieHandler;
     
     [SerializeField] private InFlyMultiplier inFlyMultiplier;
     
     [SerializeField] private Transform bulletTransform;
     
     [SerializeField] private float pathCoinsMultiplier = 0.02f;

     [SerializeField] private float forceThresholdToCountHit = 5f;
     
     [SerializeField] private float maxVelocity = 30f;
     
     [SerializeField] private Currency maxDieReward;
     
     [SerializeField] private Currency minDieReward;

     private bool _multipliedCoins;

     private bool _addPath;

     private Vector3 _lastBulletPosition;

     public float PathLength { get; private set; }

     public Currency Result { get; private set; }

     public int Hits { get; private set; }


     private void Awake()
     {
          BulletSpawner.OnChangeBullet += HandleChangeBullet;
          bulletDieHandler.OnDieEvent.AddListener(HandleBulletDie);
     }

     private void Start()
     {
          bulletCollision.OnObstacleCollision += HandleObstacleCollision;
          bulletCollision.OnGroundCollision += HandleObstacleCollision;
          bulletCollision.OnGroundObstacleCollision += HandleObstacleCollision;
          bulletCollision.OnTriggerObstacleEnter += HandleTriggerObstacleEnter;
          bulletCollision.OnTriggerObstacleTick += HandleTriggerObstacleTick;
          
          RewardSender.OnReward += HandleReward;
          
          GameState.OnStartFly += HandleStartFly;
          GameState.OnEndFly += HandleEndFly;
          GameState.OnResetGame += HandleResetGame;
     }
    
     private void OnDestroy()
     {
          BulletSpawner.OnChangeBullet -= HandleChangeBullet;
          
          RewardSender.OnReward -= HandleReward;
          
          GameState.OnStartFly -= HandleStartFly;
          GameState.OnEndFly -= HandleEndFly;
          GameState.OnResetGame -= HandleResetGame;
     }

     private void HandleObstacleCollision(ObstacleSettings obstacleSettings, float velocity)
     {
          var forceMultiplier = Mathf.Clamp(velocity, 0f, maxVelocity);
          AddCoins((int)(obstacleSettings.coinsK * forceMultiplier));
          HandleHit(velocity);
     }

     private void HandleHit(float force)
     {
          if (force > forceThresholdToCountHit)
          {
               Hits += 1;
          }
     }
     
     private void HandleTriggerObstacleEnter(TriggerObstacle triggerObstacle, float velocity)
     {
          var velocityMultiplier = Mathf.Clamp(velocity, 0f, maxVelocity);
          AddCoins((int)(triggerObstacle.EntryCoinsK * velocityMultiplier));
     }

     private void HandleTriggerObstacleTick(TriggerObstacle triggerObstacle)
     {
          AddCoins(triggerObstacle.TickCoins);
     }

     private void HandleReward(Currency currency)
     {
          Result += currency;
     }
     
     private void AddCoins(int coins)
     {
          Result += new Currency((int)(coins* inFlyMultiplier.Value), 0, CurrencyType.CoinsCrystals);
          OnUpdateResult?.Invoke(Result);
     }

     public void MultiplyCoins(int multiplier)
     {
          if (_multipliedCoins) return;
          
          Result = new Currency(Result.coins * multiplier, Result.crystals, CurrencyType.CoinsCrystals); 
          
          _multipliedCoins = true;
          
          OnUpdateResult?.Invoke(Result);
          OnChangeResults?.Invoke();
     }

     private void HandleStartFly()
     {
          _addPath = true;
          _lastBulletPosition = bulletTransform.position;
     }

     private void HandleEndFly()
     {
          AddCoins((int)(PathLength * pathCoinsMultiplier));
          OnUpdateResult?.Invoke(Result);
          _addPath = false;
          GooglePlayServices.PostToLeaderBoard(Result.coins);
     }

     private void HandleChangeBullet(GameObject bullet)
     {
          bulletTransform = bullet.transform;
          _lastBulletPosition = bulletTransform.position;
     }

     private void FixedUpdate()
     {
          if (_addPath)
          {
               PathLength += (bulletTransform.transform.position - _lastBulletPosition).magnitude;
               _lastBulletPosition = bulletTransform.position;
          }
     }

     private void HandleResetGame()
     {
          OnFinalResult?.Invoke(Result);
          Result = new Currency(0, 0, CurrencyType.CoinsCrystals);
          PathLength = 0f;
          Hits = 0;
          OnUpdateResult?.Invoke(Result);
          _multipliedCoins = false;
     }

     private void HandleBulletDie()
     {
          var dieReward = new Currency(
               minDieReward.coins + (int)(UnityEngine.Random.Range(0f, 1f) * (maxDieReward.coins - minDieReward.coins)),
               minDieReward.crystals + (int)(UnityEngine.Random.Range(0f, 1f) * (maxDieReward.crystals - minDieReward.crystals)),
               minDieReward.currencyType);
          Result += dieReward;
          OnAddDieReward?.Invoke(dieReward);
     }
}
