using System;
using Generic;
using UnityEngine;
using Zenject;
using Random = System.Random;

public class AudioEvents : MonoBehaviour
{
    [SerializeField] private BulletCollision bulletCollision;
    
    [SerializeField] private RotateCarousel rotateCarousel;

    [SerializeField] private MainMenuUIState mainMenuUIState;
    
    [SerializeField] private EndGameUIState endGameUIState;

    [SerializeField] private float minObstacleSoundVelocity = 2f;
    
    [SerializeField] private float maxCollisionVelocity = 26f;
    
    [SerializeField] private float secondEndMelodyDelay = 2f;
        
    [Inject] private PlayerBulletSkins _playerBulletSkins;

    private string _currentBulletCollisionSoundName;
    
    private string _currentBulletStartFlySoundName;
    
    private int _currentBulletCollisionSoundNumber;
    
    private int _currentBulletStartFlyNumber;

    
    private void Awake()
    {
        mainMenuUIState.OnEnter += HandleEnterMainMenu;
        BulletSpawner.OnChangeBullet += HandleChangeBullet;
        
        rotateCarousel.OnFullTurn += HandleCarouselTurn;
        rotateCarousel.OnStop += HandleCarouselStop;
        
        endGameUIState.OnEnter += HandleShowResult;
        endGameUIState.OnExit += HandleEndShowResult;
        
        bulletCollision.OnObstacleCollision += PlayObstacleCollisionSound;
        bulletCollision.OnGroundCollision += PlayObstacleCollisionSound;
        bulletCollision.OnGroundObstacleCollision += PlayObstacleCollisionSound;
        bulletCollision.OnTriggerObstacleEnter += PlayObstacleCollisionSound;
        
        GameState.OnStartGame += HandleStartGame;
        GameState.OnStartFly += HandleStartFly;
    }

    private void OnDestroy()
    {
        BulletSpawner.OnChangeBullet -= HandleChangeBullet;
        
        GameState.OnStartGame -= HandleStartGame;
        GameState.OnStartFly -= HandleStartFly;
    }

    private void PlayObstacleCollisionSound(IMakeSound obstacle, float collisionVelocity)
    {
        if (collisionVelocity < minObstacleSoundVelocity)
        {
            return;
        }
        collisionVelocity = Mathf.Clamp(collisionVelocity, 0f, maxCollisionVelocity);
        var volume = HelperFunctions.RangeToRange(collisionVelocity, 0f, maxCollisionVelocity, 0f, 1f);
        
        AudioManager.Instance.Play(obstacle.SoundName + UnityEngine.Random.Range(1, obstacle.SoundsNumber + 1), volume);
        
        AudioManager.Instance.Play(_currentBulletCollisionSoundName + UnityEngine.Random.Range(1, _currentBulletCollisionSoundNumber + 1), volume);
    }

    public void HandleStartGame()
    {
        AudioManager.Instance.Stop("theme_menu");
    }
    
    public void HandleStartFly()
    {
        AudioManager.Instance.PlayWithOverlay(_currentBulletStartFlySoundName + UnityEngine.Random.Range(1, _currentBulletStartFlyNumber + 1));
    }

    private void HandleEnterMainMenu()
    {
        AudioManager.Instance.Play("theme_menu");
    }
    
    private void HandleChangeBullet(GameObject bullet)
    {
        _currentBulletCollisionSoundName = _playerBulletSkins.CurrentBulletSkin.collisionSoundName;
        _currentBulletStartFlySoundName = _playerBulletSkins.CurrentBulletSkin.startFlySoundName;
        _currentBulletCollisionSoundNumber = _playerBulletSkins.CurrentBulletSkin.collisionSoundNumber;
        _currentBulletStartFlyNumber = _playerBulletSkins.CurrentBulletSkin.startFlySoundNumber;
    }

    private void HandleCarouselTurn()
    {
        AudioManager.Instance.PlayWithOverlay("carousel_rotate", 1f);
    }
    
    private void HandleCarouselStop()
    {
        AudioManager.Instance.Stop("carousel_rotate");
        AudioManager.Instance.Play("carousel_stop_" + UnityEngine.Random.Range(1, 4), 1f);
    }

    private void HandleShowResult()
    {
        AudioManager.Instance.Play("theme_fin1", 1f);
        Invoke(nameof(PlaySecondEndTheme), secondEndMelodyDelay);
    }

    private void PlaySecondEndTheme()
    {
        AudioManager.Instance.Stop("theme_fin1");
        AudioManager.Instance.Play("theme_fin2", 1f);
    }
    
    private void HandleEndShowResult()
    {
        AudioManager.Instance.Stop("theme_fin1");
        AudioManager.Instance.Stop("theme_fin2");
        CancelInvoke(nameof(PlaySecondEndTheme));
    }
}
