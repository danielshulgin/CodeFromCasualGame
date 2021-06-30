using System;
using Generic;
using UnityEngine;

public class BulletEffects : MonoBehaviour
{
    [SerializeField] private BulletCollision bulletCollision;

    private AndroidVibrator _androidVibrator;
    
    
    private void Start()
    {
        bulletCollision.OnObstacleCollision += HandleObstacleCollision;
        //TODO
        /*bulletCollision.OnGroundCollision += PlayObstacleCollisionSound;
        bulletCollision.OnGroundObstacleCollision += PlayObstacleCollisionSound;*/
        bulletCollision.OnFullBodyCollisionEnter2D += HandleCollision;
        
        GameState.OnEndGame += HandleEndGame;

#if !UNITY_EDITOR && UNITY_ANDROID
        _androidVibrator = new AndroidVibrator();
#endif
    }

    private void OnDestroy()
    {
        GameState.OnEndGame -= HandleEndGame;
    }

    private void HandleObstacleCollision(ObstacleSettings obstacleSettings, float velocity)
    {
#if UNITY_EDITOR && UNITY_ANDROID
        if(PlayerSettings.Instance.PhysicVibration){
            _androidVibrator.Vibrate((int)obstacleSettings.vibrationTimeMilliSeconds);
        }
#endif
    }

    public void HandleEndGame()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        if(PlayerSettings.Instance.PhysicVibration){
            _androidVibrator.Cancel();
        }
#endif
    }
    
    private void HandleCollision(Collision2D other)
    {
        BloodSpawner.instance.HandleBulletCollision(other);
    }
}
