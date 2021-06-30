using System;
using Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class RotateCarousel : MonoBehaviour
{
    public Action OnFullTurn;
    
    public Action OnStop;
    
    [SerializeField] private ForceAdjuster forceAdjuster;

    [SerializeField] private float offset = 2.8f;
    
    [SerializeField] private float maxRotationSpeed = 10f;
    
    [SerializeField] private float startSpeed = 0.1f;
    
    [SerializeField] private StartFlyDirection startDirectionRandomType = StartFlyDirection.Random;

    [SerializeField] [Range(-1f, 1f)] private float animationRelativePosition;

    [SerializeField] private Animator carouselAnimator;
    
    [SerializeField] private Animator bulletRotationPositionAnimator;
    
    [SerializeField] private GameObject bulletStoreButton;
    
    [SerializeField] private Transform bulletRotationAnimationPoint;
    
    [SerializeField] private string bulletRotationPositionAnimationName;
    
    [SerializeField] private float r = 2.5f;
    
    [SerializeField] private int discretizationLevels = 10;
    
    [Inject] private PlayerData _playerData;
    
    [Inject] private PlayerBulletSkins _playerBulletSkins;

    private bool _rotate;
    
    private bool _started;
    
    private GameObject _bulletGameObject;

    private bool _currentStartDirectionClockwise = true;
    
    private string _currentBulletSpinAnimationName;

    public bool RightFlyDirection { get; private set; } = true;
    
    
    public bool СurrentStartDirectionClockwise => _currentStartDirectionClockwise;

    public float Offset => offset;
    
    public float RelativePosition => animationRelativePosition;
    

    private void Awake()
    {
        BulletSpawner.OnChangeBullet += HandleChangeBullet;
    }

    private void Start()
    {
        carouselAnimator.Play(_currentBulletSpinAnimationName, 0, 0f);
        carouselAnimator.speed = 0f;
        
        GameState.OnStartGame += HandleStartGame;
        GameState.OnStartFly += HandleStartFly;
        GameState.OnResetGame += HandleResetGame;
    }
    
    private void OnDestroy()
    {
        BulletSpawner.OnChangeBullet -= HandleChangeBullet;
        
        GameState.OnStartGame -= HandleStartGame;
        GameState.OnStartFly -= HandleStartFly;
        GameState.OnResetGame -= HandleResetGame;
    }

    private void Update()
    {
        if (_rotate)
        {
            if (forceAdjuster.Force < float.Epsilon)
            {
                if (_started)
                {
                    _started = false;
                    OnStop?.Invoke();
                }
            }
            else
            {
                _started = true;
            }

            var force = forceAdjuster.Force;
            if (force > float.Epsilon)
            {
                force = startSpeed + HelperFunctions.RangeToRange(force, 0f, 1f, startSpeed, 1f);
            }
            
            animationRelativePosition += force * Time.deltaTime * maxRotationSpeed;
            UpdatePositions();
        }
    }

    public void UpdatePositions()
    {
        if (animationRelativePosition > 1f)
        {
            OnFullTurn?.Invoke();
            animationRelativePosition = 0f;
        }
        carouselAnimator.Play(_currentBulletSpinAnimationName, 0, animationRelativePosition);
        bulletRotationPositionAnimator.Play(bulletRotationPositionAnimationName, 0, animationRelativePosition);
        UpdateBulletPosition();
        UpdateBulletScale();

        if (_currentStartDirectionClockwise)
        {
            RightFlyDirection = bulletRotationAnimationPoint.localScale.x < 0;
        }
        else
        {
            RightFlyDirection = bulletRotationAnimationPoint.localScale.x > 0;
        }
    }

    public void HandleStartFly()
    {
        _started = false;
        _rotate = false;
        carouselAnimator.Play("carousel_idle");
    }
    
    public void Freeze()
    {
        _rotate = false;
        
        carouselAnimator.speed = 0f;
        bulletRotationPositionAnimator.speed = 0f;
    }

    public void HandleStartGame()
    {
        _rotate = true;

        bulletStoreButton.SetActive(false);
    }

    public void HandleResetGame()
    {
        bulletStoreButton.SetActive(true);
    }
    
    public void SetDebugValues(float animationRelativePosition, bool clockWise)
    {
        startDirectionRandomType = clockWise ? StartFlyDirection.Clockwise : StartFlyDirection.CounterClockwise;
        this.animationRelativePosition = animationRelativePosition;
        
        UpdateStartBulletDirection();
        UpdateCarouselScale();
        UpdateAnimationBindings();
        
        carouselAnimator.Play(_currentBulletSpinAnimationName, 0, animationRelativePosition);
        bulletRotationPositionAnimator.Play(bulletRotationPositionAnimationName, 0, animationRelativePosition);
    }

    private void HandleChangeBullet(GameObject bullet)
    {
        _bulletGameObject = bullet;
        UpdateStartBulletDirection();
        UpdateCarouselScale();
        UpdateAnimationBindings();

        SetAnimationToIdle();
    }

    private void SetAnimationToIdle()
    {
        animationRelativePosition = 0f;
        
        carouselAnimator.Play(_currentBulletSpinAnimationName, 0, 0);
        bulletRotationPositionAnimator.Play(bulletRotationPositionAnimationName, 0, 0);
        carouselAnimator.speed = 0f;
        bulletRotationPositionAnimator.speed = 0f;
    }

    private void UpdateStartBulletDirection()
    {
        RightFlyDirection = true;
        switch (startDirectionRandomType)
        {
            case StartFlyDirection.Clockwise:
                RightFlyDirection = false;
                break;
            case StartFlyDirection.CounterClockwise:
                RightFlyDirection = true;
                break;
            case StartFlyDirection.Random:
                if (UnityEngine.Random.value > 0.5f)
                {
                    RightFlyDirection = !RightFlyDirection;
                }

                break;
        }
        _currentStartDirectionClockwise = !RightFlyDirection;
    }

    private void UpdateAnimationBindings()
    {
        var currentBulletScriptableObject = _playerData.ScriptableObjectDataBase
            .BulletScriptableObjectsById[_playerData.LastSelectedBulletId];

        _currentBulletSpinAnimationName = currentBulletScriptableObject.carouselSpinAnimationName;
    }

    private void UpdateCarouselScale()
    {
        var carouselScale = carouselAnimator.gameObject.transform.localScale;
        if (_currentStartDirectionClockwise)
        {
            carouselAnimator.gameObject.transform.localScale =
                new Vector3(-Mathf.Abs(carouselScale.x), carouselScale.y, carouselScale.z);
        }
        else
        {
            carouselAnimator.gameObject.transform.localScale =
                new Vector3(Mathf.Abs(carouselScale.x), carouselScale.y, carouselScale.z);
        }
    }

    public void UpdateBulletPosition()
    {
        if (_bulletGameObject!= null)
        {
            _bulletGameObject.transform.position = new Vector3(
                r * DiscretizeXAnimationPosition(bulletRotationAnimationPoint.position.x),
                bulletRotationAnimationPoint.position.y + _playerBulletSkins.CurrentBullet.offsetY, 
                bulletRotationAnimationPoint.position.z);
        }
    }

    private float DiscretizeXAnimationPosition(float value)
    {
        return Mathf.RoundToInt(((value + 1f) / 2f) * discretizationLevels) / (float)discretizationLevels * 2f - 1f; 
    }
    
    public void UpdateBulletScale()
    {
        var invertScaleK = _currentStartDirectionClockwise ? -1 : 1;
        _bulletGameObject.transform.localScale = new Vector3(bulletRotationAnimationPoint.localScale.x * invertScaleK,
                                                              bulletRotationAnimationPoint.localScale.y,
                                                              bulletRotationAnimationPoint.localScale.z);
    } 
}
