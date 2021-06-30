using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet skin", menuName = "MyScriptableObjects/Bullet skin", order = 5)] 
public class BulletSkinScriptableObject : ItemScriptableObject
{
    public GameObject bulletPrefab;
    
    public BulletScriptableObject bullet;

    public string collisionSoundName = "char_racoon_";
    
    public string startFlySoundName = "charfly_racoon_";
    
    public int collisionSoundNumber = 5;
    
    public int startFlySoundNumber = 5;
}
