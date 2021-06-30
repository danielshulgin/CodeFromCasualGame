using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleSettings", menuName = "MyScriptableObjects/ObstacleSettings", order = 0)] 
public class ObstacleSettings : ScriptableObject, IMakeSound 
{
    public float hpDamageK = 1f;

    public bool enableEffects = true;
    
    public float lockTime = 0.7f;

    public float vibrationTimeMilliSeconds;
    
    public int coinsK = 1;

    public string soundName;
    
    public int soundNumber;
    
    public string SoundName => soundName;

    public int SoundsNumber => soundNumber;
}