using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CannonScriptableObject", menuName = "MyScriptableObjects/CannonScriptableObject", order = 4)]
public class CannonScriptableObject : ItemScriptableObject
{
    public Grade grade;
    
    public Sprite cannonSprite;

    public float shotForce = 13f;

    public float maxDeflectionAngle = 20f;
}
