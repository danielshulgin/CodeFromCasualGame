using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Zenject;


public class BulletDieHandler : MonoBehaviour
{
    public UnityEvent OnDieEvent;
    
    [SerializeField] private GameState gameState;
    
    [SerializeField] private CompleteDieUIState dieUIState;
    
    [SerializeField] private BulletCollision bulletCollision;

    [SerializeField] private GameObject dieParticlesPrefab;
    
    [SerializeField] private ParticleSystem dieScreenParticles;
    
    [SerializeField] private float standardDieEffectsDuration = 3f;
    
    [SerializeField] private float shredderDieResultsDelay = 0f;
    
    [Inject] private PlayerBulletSkins _playerBulletSkins;
    
    [Inject] private UIStateMachine _uiStateMachine;

    private GameObject _bullet;

    
    private void Awake()
    {
        BulletSpawner.OnChangeBullet += HandleChangeBullet;
        MagnetDieZone.OnEndPull += HandleShredderDie;
    }

    private void OnDestroy()
    { 
        BulletSpawner.OnChangeBullet -= HandleChangeBullet;
        MagnetDieZone.OnEndPull -= HandleShredderDie;
    }
    
    public void HandleChangeBullet(GameObject bullet)
    {
        _bullet = bullet;
        
        bullet.GetComponent<BulletHP>().OnZeroHP += HandleStandardDie;
    }

    private void HandleStandardDie()
    {
        if (PlayerSettings.Instance.Blood)
        {
            Instantiate(dieParticlesPrefab, _bullet.transform.position, quaternion.identity);
            dieScreenParticles.Play();
            _bullet.SetActive(false);
            gameState.SendEndFly();
            gameState.SendEndGame();
            OnDieEvent?.Invoke();
            StartCoroutine(DelayShowResults(standardDieEffectsDuration));
        }
        else
        {
            _bullet.SetActive(false);
            gameState.SendEndFly();
            gameState.SendEndGame();
            gameState.SendShowResults();
        }
    }

    private void HandleShredderDie()
    {
        gameState.SendEndFly();
        gameState.SendEndGame();
        OnDieEvent?.Invoke();
        StartCoroutine(DelayShowResults(shredderDieResultsDelay));
    }

    IEnumerator DelayShowResults(float time)
    {
        _uiStateMachine.ChangeState(dieUIState);
        yield return new WaitForSeconds(time);
        dieUIState.Exit();
        gameState.SendShowResults();
    }
}
