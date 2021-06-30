using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "MyScriptableObjects/Bullet", order = 0)] 
public class BulletScriptableObject : ItemScriptableObject
{
    public BulletSkinScriptableObject firstSkin;

    public List<BulletSkinScriptableObject> bulletSkinScriptableObjects;

    public string carouselSpinAnimationName = "raccoon_default_spin";

    public int hp;

    public float offsetY;

    public float cannonBallCollisionResistance = 1f;
    
    public float cannonBallExplosionResistance = 1f;
}

