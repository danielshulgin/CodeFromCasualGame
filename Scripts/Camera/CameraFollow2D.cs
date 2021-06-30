using System.Collections;
using Cinemachine;
using UnityEngine;
using Zenject;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera staticCineMachine;

    [SerializeField] private CinemachineVirtualCamera followCineMachine;

    [SerializeField] private BulletCollision bulletCollision;
    
    [SerializeField] private float shakeDuration = 0.5f;
    
    [SerializeField] private float shakeAmplitude = 1.2f;
    
    [SerializeField] private float shakeFrequency = 2.0f;
        
    [Inject] private PlayerBulletSkins _playerBulletSkins;

    private CinemachineBasicMultiChannelPerlin _virtualCameraNoise;

    private Vector3 _startPosition;

    private bool _shaking = false;


    private void Awake()
    {
        BulletSpawner.OnChangeBullet += HandleChangeBullet;
    }

    private void Start()
    {
        _startPosition = transform.position;
        _virtualCameraNoise = followCineMachine.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        
        bulletCollision.OnObstacleCollision += HandleObstacleCollision;
        bulletCollision.OnGroundCollision += HandleObstacleCollision;
        bulletCollision.OnGroundObstacleCollision += HandleObstacleCollision;
        
        GameState.OnStartFly += HandleStartFly;
        GameState.OnResetGame += HandleResetGame;
    }
    
    private void OnDestroy()
    {
        BulletSpawner.OnChangeBullet -= HandleChangeBullet;
        
        GameState.OnStartFly -= HandleStartFly;
        GameState.OnResetGame -= HandleResetGame;
    }

    private void HandleObstacleCollision(ObstacleSettings obstacleSettings, float velocity)
    {
        if (obstacleSettings.enableEffects)
        {
            Shake(velocity);
        }
    }

    public void Shake(float force)
    {
        if (!_shaking)
        {
            StartCoroutine(StartShake(shakeDuration, force));
        }
    }

    public void HandleStartFly()
    {
        staticCineMachine.enabled = false;
        followCineMachine.OnTransitionFromCamera(staticCineMachine, Vector3.up, Time.deltaTime);
        followCineMachine.enabled = true;
    }

    public void HandleResetGame()
    {
        _shaking = false;
        StopAllCoroutines();
        _virtualCameraNoise. m_AmplitudeGain = 0f;
        _virtualCameraNoise.m_FrequencyGain = 0f;
        followCineMachine.enabled = false;
        Camera.main.transform.rotation = Quaternion.identity;
        staticCineMachine.enabled = true;
        transform.position = _startPosition;
    }

    private void HandleChangeBullet(GameObject bullet)
    {
        followCineMachine.Follow = bullet.transform;
        followCineMachine.LookAt = bullet.transform;
    }

    private IEnumerator StartShake(float duration, float force)
    {
        _shaking = true;
        _virtualCameraNoise. m_AmplitudeGain = shakeAmplitude * force;
        _virtualCameraNoise.m_FrequencyGain = shakeFrequency;
        yield return new WaitForSeconds(duration);
        _virtualCameraNoise.m_AmplitudeGain = 0f;
        _virtualCameraNoise.m_FrequencyGain = 0f;
        _shaking = false;
    }
}
