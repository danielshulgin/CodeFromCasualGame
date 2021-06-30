using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class BreakablePart : MonoBehaviour
{
    public int hpToDestroy = 3;
    
    public float baseInFlyMultiplier = 1f;
        
    public List<ParticleSystem> breakParticles;
        
    public List<GameObject> gameObjectsToActivatePhysics;
    
    public List<Collider2D> activeColliders2D;
    
    public List<Collider2D> brokenColliders2D;
        
    public Sprite brokenSprite;
    
    public SpriteRenderer spriteRenderer;

    public Transform centerMassOffsetTransform;
    
    public bool active = true;

    [HideInInspector] public BulletHP bulletHp;

    private Vector3 _centerMassOffset;
    
    public Vector3 CenterMassOffset => centerMassOffsetTransform.localPosition;

    public BodyPartImpactHandler BodyPartImpactHandler => bulletHp.BodyPartImpactHandler;
    
    
    private void Start()
    {
        bulletHp = GetComponentInParent<BulletHP>();
    }
}
