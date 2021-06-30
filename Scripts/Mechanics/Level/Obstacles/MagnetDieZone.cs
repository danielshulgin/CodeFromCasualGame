using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class MagnetDieZone : MonoBehaviour
{
    public static event Action OnStartPull;
    
    public UnityEvent StartPull;
    
    public static event Action OnEndPull;
    
    public UnityEvent EndPull;
    
    [SerializeField] private float moveTimeIn = 1.5f;
    
    [SerializeField] private float moveTimeDie = 5f;

    [SerializeField] private Transform targetPointIn;
    
    [SerializeField] private Transform targetPointDie;
    
    [SerializeField] private Transform magnetPoint;

    [SerializeField] private Collider2D shredderCollider2D;
    
    [SerializeField] private ParticleSystem startParticles;
    
    [SerializeField] private ParticleSystem endParticles;
    
    [SerializeField] private float endParticlesDelay = 1f;

    private bool inAction;

    private Rigidbody2D bulletRigidbody2D;

    private Sequence _dieSequence;
    

    private void Start()
    {
        GameState.OnEndGame += HandleEndGame;
    }

    private void OnDestroy()
    {
        GameState.OnEndGame -= HandleEndGame;
    }

    //TODO handle start game and unlock mechanic
    private void HandleEndGame()
    {
        StopAllCoroutines();
        if (inAction)
        {
            _dieSequence?.Kill();
            shredderCollider2D.isTrigger = false;
            inAction = false;
            AudioManager.Instance.Stop("obst_shredder");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            var breakablePart = other.GetComponent<BreakablePart>();
            if (!breakablePart.active)
            {
                return;
            }
            inAction = true;
            
            bulletRigidbody2D = other.GetComponentInParent<Rigidbody2D>();
            magnetPoint.transform.position = other.transform.position;
            
            var bulletHp = breakablePart.bulletHp;
            var bulletParts = bulletHp.breakableParts.Where(bp => bp.active)
                .Select(bp => bp.gameObject);

            other.gameObject.layer = 9;
            foreach (var bulletPart in bulletParts)
            {
                bulletPart.layer = 9;
            }
            
            if (startParticles != null && PlayerSettings.Instance.Blood)
            {
                startParticles.Play();
            }
            
            Invoke(nameof(StartEndParticles), endParticlesDelay);
            shredderCollider2D.isTrigger = true;
            
            StartPull.Invoke();
            OnStartPull?.Invoke();
            AudioManager.Instance.Play("obst_shredder");
            
            _dieSequence = DOTween.Sequence().Append(magnetPoint
                    .DOMove(targetPointIn.transform.position, moveTimeIn))
                .Append(magnetPoint
                    .DOMove(targetPointDie.transform.position, moveTimeDie))
                .AppendCallback(() =>
                    {
                        
                        shredderCollider2D.isTrigger = false;
                        inAction = false;
                        other.transform.parent.gameObject.SetActive(false);
                        AudioManager.Instance.Stop("obst_shredder");
                        EndPull.Invoke();
                        OnEndPull?.Invoke();
                    }
                );
        }
    }

    private void FixedUpdate()
    {
        if (inAction)
        {
            bulletRigidbody2D.MovePosition(magnetPoint.position);
            bulletRigidbody2D.MoveRotation(0f);
        }
    }

    private void StartEndParticles()
    {
        if (endParticles != null && PlayerSettings.Instance.Blood)
        {
            endParticles.Play();
        }
    }
}
