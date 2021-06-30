using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletHP : MonoBehaviour
{
    public event Action<BreakablePart> OnBreakPart;
    
    public event Action OnZeroHP;
    
    [SerializeField] private int hp = 5000;
    
    [SerializeField] public List<BreakablePart> breakableParts;
    
    private float brokenPartGravityScale = 2;
    
    private float brokenPartMass = 10;
    
    private BulletCollision _bulletCollision;

    private bool _locked;
    
    private int _activePartIndex;

    public int Hp
    {
        get => hp;
        private set => hp = value < 0 ? 0 : value;
    }
    
    public BodyPartImpactHandler BodyPartImpactHandler { get; private set; }


    private void Start()
    {
        GameState.OnEndGame += HandleEndGame;
    }

    public void Initialize()
    {
        _bulletCollision = GetComponentInParent<BulletCollision>();
        BodyPartImpactHandler = GetComponent<BodyPartImpactHandler>();
        
        _bulletCollision.OnObstacleCollision += HandleObstacleCollision;
        _bulletCollision.OnGroundCollision += HandleObstacleCollision;
        _bulletCollision.OnGroundObstacleCollision += HandleObstacleCollision;
        _bulletCollision.OnTriggerObstacleEnter += HandleTriggerObstacleEnter;
        _bulletCollision.OnTriggerObstacleTick += HandleTriggerObstacleTick;
        
        _locked = false;
        _activePartIndex = 0;
    }

    private void OnDestroy()
    {
        if (_bulletCollision == null)
        {
            //TODO
            return;
        }
        _bulletCollision.OnObstacleCollision -= HandleObstacleCollision;
        _bulletCollision.OnGroundCollision -= HandleObstacleCollision;
        _bulletCollision.OnGroundObstacleCollision -= HandleObstacleCollision;
        _bulletCollision.OnTriggerObstacleEnter -= HandleTriggerObstacleEnter;
        _bulletCollision.OnTriggerObstacleTick -= HandleTriggerObstacleTick;
        
        GameState.OnEndGame -= HandleEndGame;
    }
    
    private void HandleEndGame()
    {
        _locked = true;
    }

    public void HandleDamage(int damage)
    {
        if(_locked)
            return;
        
        Hp -= damage;
        
        CheckBreakableParts();

        if (hp == 0)
        {
            OnZeroHP?.Invoke();
        }
    }

    private void HandleTriggerObstacleEnter(TriggerObstacle triggerObstacle, float velocity)
    {
        HandleDamage((int)(triggerObstacle.EntryDamageK * velocity));
    }

    private void HandleTriggerObstacleTick(TriggerObstacle triggerObstacle)
    {
        HandleDamage(triggerObstacle.TickDamage);
    }

    private void HandleObstacleCollision(ObstacleSettings obstacleSettings, float velocity)
    {
        Debug.Log("Collision force: " + velocity + "\nObstacle name: " + obstacleSettings.name);
        HandleDamage((int) (velocity * obstacleSettings.hpDamageK));
    }

    private void CheckBreakableParts()
    {
        foreach (var breakablePart in breakableParts)
        {
            if (breakablePart.active && breakablePart.hpToDestroy >= hp)
            {
                BreakPart(breakablePart);
            }
        }
    }

    private void BreakPart(BreakablePart breakablePart)
    {
        breakablePart.active = false;
        ActivateParticles(breakablePart.breakParticles);
        ActivatePhysic(breakablePart.gameObjectsToActivatePhysics);
        SetActivateColliders2D(breakablePart.activeColliders2D, false);
        SetActivateColliders2D(breakablePart.brokenColliders2D, true);

        if (breakablePart.spriteRenderer != null && breakablePart.brokenSprite != null)
        {
            breakablePart.spriteRenderer.sprite = breakablePart.brokenSprite;
        }

        if (breakablePart.centerMassOffsetTransform != null)
        {
            ChangeCenterMass(breakablePart.CenterMassOffset);
        }

        ++_activePartIndex;
        if (_activePartIndex < breakableParts.Count)
        {
            SetActivateColliders2D(breakableParts[_activePartIndex].activeColliders2D, true);
        }
        OnBreakPart?.Invoke(breakablePart);
    }

    private void ActivateParticles(List<ParticleSystem> particleSystems)
    {
        if(!PlayerSettings.Instance.Blood)
            return;
        
        foreach (var breakParticleSystem in particleSystems)
        {
            breakParticleSystem.Play();
        }
    }
    
    private void ActivatePhysic(List<GameObject> gameObjectsToActivate)
    {
        foreach (var gameObjectToActivatePhysic in gameObjectsToActivate)
        {
            var rb2D = gameObjectToActivatePhysic.AddComponent<Rigidbody2D>();
            rb2D.gravityScale = brokenPartGravityScale;
            rb2D.mass = brokenPartMass;
        }
    }
    
    private void SetActivateColliders2D(List<Collider2D> collider2Ds, bool active)
    {
        foreach (var partCollider2D in collider2Ds)
        {
            partCollider2D.enabled = active;
        }
    }

    private void ChangeCenterMass(Vector3 offset)
    {
        transform.position = transform.TransformPoint(offset);
        foreach (Transform child in transform)
        {
            child.transform.localPosition -= offset;
        }
    }
}
