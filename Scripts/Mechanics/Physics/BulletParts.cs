using System.Collections.Generic;
using UnityEngine;


public class BulletParts : MonoBehaviour
{
    public Rigidbody2D pivotRigidbody2D;
    
    public BulletHP bulletHp;

    public PhysicVelocityConstrain pivotPhysicVelocityConstrain;
    
    public BodyPartImpactHandler pivotBodyPartImpactHandler;
}
