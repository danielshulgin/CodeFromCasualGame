using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class StoreInfo
{
    public StoreUIState storeUIState;
    
    public StoreTabUI storeTabUI;

    public StoreType storeType;
}

public enum StoreType
{
    IAP, Bullet, BulletSkin, Cannon, CannonBall, Obstacle
}
