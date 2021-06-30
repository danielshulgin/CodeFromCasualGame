using MyBox;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "CannonballScriptableObject", menuName = "MyScriptableObjects/CannonballScriptableObject", order = 5)]
public class CannonballScriptableObject : StackTypeScriptableObject
{
    public Grade grade;
    
    public GameObject cannonballPrefab;
    
    [FormerlySerializedAs("velocityMultiplier")] public float mass = 1f;

    public CannonballImpactType cannonballImpactType;

    [ConditionalField(nameof(cannonballImpactType), false, CannonballImpactType.Explosion)]
    public float explosionImpact = 1f;
    
    [ConditionalField(nameof(cannonballImpactType), false, CannonballImpactType.Collision)]
    public float collisionImpact = 1f;

    [ConditionalField(nameof(cannonballImpactType), false, CannonballImpactType.Collision)]
    public float maxBounceFlyTime = 0.5f;
}

public enum CannonballImpactType
{
    Explosion, Collision 
}
