using System;
using DG.Tweening;
using Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class CannonUI : MonoBehaviour
{
    [SerializeField] private ClickManager clickManager;

    [SerializeField] private Cannon explosionSpawner;

    [SerializeField] private TextMeshProUGUI cannonballsNumber;

    [SerializeField] private Image cannonImage;
    
    [SerializeField] private Transform cannonPivot;
    
    [SerializeField] private RectTransform cannonMuzzle;
    
    [SerializeField] private Transform sliderTransform;
    
    [SerializeField] private CanvasGroup cannonCanvasGroup;
    
    [SerializeField] private GameObject cannonShotParticles;
    
    [SerializeField] private GameObject cannonballsNumberUI;

    [SerializeField] private GameObject swipeStartCircle;
    
    [SerializeField] private GameObject fireZone;
    
    [SerializeField] private Image fireZoneMaskImage;

    [SerializeField][Range(0f, 1f)] private float maxSwipeRelativeLength = 0.4f;

    [SerializeField] private float restoreRotationTime = 1f;
    
    [SerializeField][Range(0f, 1f)] private float recoilPower = 0.3f;
    
    [SerializeField] private float recoilTime = 0.5f;
    
    [SerializeField] private float cancelRadius = 250f;

    [Inject] private PlayerCannonballs _playerCannonballs;
    
    [Inject] private PlayerCannons _playerCannons;

    private CannonballScriptableObject _cannonballScriptableObject;
    
    private CannonScriptableObject _cannonScriptableObject;

    private float _maxSwipeLength = 250f;
    
    private Vector2 _swipeStartPosition;
    
    private Vector2 _lastInputVector;

    private bool _canFire;
    
    private bool _swipeStart;
    
    private float _startFireZoneSize;


    private void Awake()
    {
        _startFireZoneSize = fireZone.transform.localScale.x;
        _maxSwipeLength = Screen.height * maxSwipeRelativeLength;

        GameState.OnStartGame += HandleStartGame;
        GameState.OnStartFly += HandleStartFly;
        GameState.OnEndFly += HandleEndFly;

        PlayerCannonballs.OnChangeCannonball += HandleChangeCannonball;
        PlayerCannons.OnChangeCannon += HandleChangeCannon;
        HandleChangeCannonball(_playerCannonballs.CurrentCannonball);
        HandleChangeCannon(_playerCannons.CurrentCannon);
        
    }

    private void OnDestroy()
    {
        GameState.OnStartGame -= HandleStartGame;
        GameState.OnStartFly -= HandleStartFly;
        GameState.OnEndFly -= HandleEndFly;

        PlayerCannonballs.OnChangeCannonball -= HandleChangeCannonball;
        PlayerCannons.OnChangeCannon -= HandleChangeCannon;
    }

    private void HandleStartGame()
    {
        cannonballsNumber.text = _playerCannonballs.GetNumberInStack(_cannonballScriptableObject).ToString();
        sliderTransform.localScale = new Vector3(1f, 0.001f, 1f);
    }

    private void HandleStartFly()
    {
        cannonballsNumberUI.SetActive(true);
        _canFire = true;

        cannonCanvasGroup.alpha = 1f;
        
        clickManager.OnSwipe.AddListener(HandleSwipe);
        clickManager.OnSwipeBegin.AddListener(HandleSwipeBegin);
        clickManager.OnSwipeEnd.AddListener(HandleSwipeEnd);
    }

    private void HandleEndFly()
    {
        cannonballsNumberUI.SetActive(false);
        fireZone.SetActive(false);
        swipeStartCircle.SetActive(false);
        _canFire = false;
        _swipeStart = false;
        
        cannonCanvasGroup.alpha = 0f;
        
        clickManager.OnSwipe.RemoveListener(HandleSwipe);
        clickManager.OnSwipeBegin.RemoveListener(HandleSwipeBegin);
        clickManager.OnSwipeEnd.RemoveListener(HandleSwipeEnd);

        cannonPivot.rotation = Quaternion.identity;
        
        SetFireZoneSize(0f);
    }

    private void HandleSwipeBegin(Touch touch)
    {
        if (_playerCannonballs.GetNumberInStack(_cannonballScriptableObject) <= 0 || !_canFire)
        {
            return;
        }
        _swipeStartPosition = touch.position;
        swipeStartCircle.SetActive(true);
        swipeStartCircle.transform.position = _swipeStartPosition;
        _swipeStart = true;
        fireZone.SetActive(true);
    }
    
    private void HandleSwipe(Touch touch)
    {
        if (_playerCannonballs.GetNumberInStack(_cannonballScriptableObject) <= 0 ||
            !_canFire || !_swipeStart || CancelSwipe(touch.position))
        {
            return;
        }
        var inputVector = GetInputVector(touch.position);
        var power = GetShotPower(inputVector);
        RotateCannonImmediately(inputVector.normalized);
        sliderTransform.localScale = new Vector3(1f, power, 1f);
        SetFireZoneSize(power);
    }
    
    private void HandleSwipeEnd(Touch touch)
    {
        fireZone.SetActive(false);
        SetFireZoneSize(0f);
        sliderTransform.localScale = new Vector3(1f, 0.001f, 1f);
        swipeStartCircle.SetActive(false);
        
        if (_playerCannonballs.GetNumberInStack(_cannonballScriptableObject) <= 0 ||
            !_canFire || !_swipeStart || CancelSwipe(touch.position))
        {
            _swipeStart = false;
            RotateCannon(Vector3.zero);
            return;
        }
        _swipeStart = false;

        var inputVector = GetInputVector(touch.position);
        var power = GetShotPower(inputVector);
        var muzzleWordPosition = Camera.main.ScreenToWorldPoint(cannonMuzzle.transform.position); 
        LaunchBomb(new Vector2(muzzleWordPosition.x - Camera.main.transform.position.x, muzzleWordPosition.y), 
            inputVector.normalized, power);
        
        SimulateCannonRecoil();
        SpawnCannonShotParticles();
    }
    
    private void LaunchBomb(Vector3 startPosition, Vector2 direction, float power)
    {
        explosionSpawner.LaunchCannonball(startPosition, direction, power);
        _playerCannonballs.UseStackItems(_cannonballScriptableObject);
        cannonballsNumber.text = _playerCannonballs.GetNumberInStack(_cannonballScriptableObject).ToString();
    }

    private bool CancelSwipe(Vector2 currentFingerPosition)
    {
        var diff = currentFingerPosition - _swipeStartPosition;
        if (diff.magnitude  < cancelRadius || diff.y < 0)
        {
            return true;
        }

        return false;
    }

    private Vector2 GetInputVector(Vector2 currentFingerPosition)
    {
        var diff = currentFingerPosition - _swipeStartPosition;
        
        if (diff.y <= 0)
        {
            return _lastInputVector;
        }
        
        if (diff.magnitude > _maxSwipeLength + cancelRadius)
        {
            diff = (_maxSwipeLength + cancelRadius) * diff.normalized;
        }

        _lastInputVector = diff;
        return diff;
    }

    private float GetShotPower(Vector2 diff)
    {
        var shotPower = (diff.magnitude - cancelRadius) / _maxSwipeLength;
        if (shotPower < 0f)
        {
            shotPower = 0f;
        }
        return shotPower;
    }

    private void RotateCannonImmediately(Vector3 direction)
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cannonPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    private void SimulateCannonRecoil()
    {
        var diff = recoilPower * cannonPivot.GetComponent<RectTransform>().rect.height * transform.up;
        DOTween.Sequence()
            .Append(cannonPivot.DOMove(cannonPivot.position - diff, recoilTime * 0.33f))
            .Append(cannonPivot.DOMove(cannonPivot.transform.position, recoilTime * 0.66f))
            .AppendCallback(() => RotateCannon(Vector3.zero))
            .Play();
    }
    
    private void RotateCannon(Vector3 direction)
    {
         cannonPivot.DORotate(direction, restoreRotationTime);
    }

    private void SpawnCannonShotParticles()
    {
        var position = Camera.main.ScreenToWorldPoint(cannonMuzzle.position) - new Vector3(Camera.main.transform.position.x, 0f);
        var particles = Instantiate(cannonShotParticles);
        particles.transform.position = position;
    }

    private void HandleChangeCannonball(CannonballScriptableObject cannonballScriptableObject)
    {
        _cannonballScriptableObject = cannonballScriptableObject;
    }
    
    private void HandleChangeCannon(CannonScriptableObject cannonScriptableObject)
    {
        _cannonScriptableObject = cannonScriptableObject;
        cannonImage.sprite = cannonScriptableObject.cannonSprite;
        var maxDeflectionAngle = cannonScriptableObject.maxDeflectionAngle;
        fireZoneMaskImage.fillAmount = maxDeflectionAngle / 180f; //maxDeflectionAngle * 2f / 360f;
        fireZoneMaskImage.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f + maxDeflectionAngle);
    }

    private void SetFireZoneSize(float power)
    {
        if (!PlayerSettings.Instance.Sight)
        {
            power = 0f;
        }
        var size = _startFireZoneSize + power * (1f - _startFireZoneSize);
        fireZone.transform.localScale = size * Vector3.one;
    }
}
