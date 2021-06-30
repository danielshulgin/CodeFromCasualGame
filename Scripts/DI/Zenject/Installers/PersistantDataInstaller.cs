using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class PersistantDataInstaller: MonoInstaller
{
    [SerializeField] private PlayerData playerData;
    
    [SerializeField] private PlayerVasylSkins playerVasylSkins;
    
    [SerializeField] private PlayerLevels playerLevels;
    
    [SerializeField] private PlayerBulletSkins playerBulletSkins;
    
    [SerializeField] private PlayerCannonballs playerCannonBalls;
    
    [SerializeField] private PlayerCannons playerCannons;
    
    [SerializeField] private InAppPurchaseManager inAppPurchaseManager;
    
    [SerializeField] private ScriptableObjectDataBase scriptableObjectDataBase;
    
    
    public override void InstallBindings()
    {
        Container.Bind<PlayerData>().FromInstance(playerData);
        
        Container.Bind<PlayerVasylSkins>().FromInstance(playerVasylSkins);
        Container.Bind<PlayerLevels>().FromInstance(playerLevels);
        Container.Bind<PlayerBulletSkins>().FromInstance(playerBulletSkins);
        Container.Bind<PlayerCannonballs>().FromInstance(playerCannonBalls);
        Container.Bind<PlayerCannons>().FromInstance(playerCannons);
        
        Container.Bind<InAppPurchaseManager>().FromInstance(inAppPurchaseManager);
        
        Container.Bind<ScriptableObjectDataBase>().FromInstance(scriptableObjectDataBase);
    }
}