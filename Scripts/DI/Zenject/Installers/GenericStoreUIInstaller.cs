using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GenericStoreUIInstaller : MonoBehaviour
{    
    [SerializeField] private ObstaclesStoreUIState obstaclesStoreUiState;
    
    [SerializeField] private BulletSkinStoreUIState bulletSkinStoreUiState;
    
    [SerializeField] private BulletStoreUIState bulletStoreUiState;
    
    [SerializeField] private Button storeBuyButton;
    
    [SerializeField] private Button storeExitButton;
    
    [SerializeField] private Button storeEditButton;
    
    [SerializeField] private Image storeSelectedImage;
    
    [SerializeField] private Image storeObstacleCharacteristicsImage;
    
    [SerializeField] private RectTransform coinsAdvisePoint;
    
    [SerializeField] private RectTransform obstacleCharacteristicsAdvisePoint;
    
    [SerializeField] private RectTransform buyButtonPalmPointerPosition;
    
    [SerializeField] private RectTransform skipButtonInStorePosition;
    
    [SerializeField] private RectTransform tapToBuyAdvisePoint;
     
    [SerializeField] private RectTransform newSkinAdvisePoint;
     
    [SerializeField] private RectTransform newCharacterAdvisePoint;

    [Inject(Id = "MainSceneContainer")] private DiContainer _mainSceneContainer;

    
    private void Awake()
    {
        InstallBindings();
    }

    public void InstallBindings()
    {
        _mainSceneContainer.Unbind<ObstaclesStoreUIState>();
        _mainSceneContainer.Unbind<BulletSkinStoreUIState>();
        _mainSceneContainer.Unbind<BulletStoreUIState>();

        _mainSceneContainer.UnbindId<Button>("StoreBuyButton");
        _mainSceneContainer.UnbindId<Button>("StoreEditButton");
        _mainSceneContainer.UnbindId<Button>("StoreExitButton");

        _mainSceneContainer.UnbindId<Image>("StoreSelectedImage");
        _mainSceneContainer.UnbindId<Image>("StoreObstacleCharacteristicsImage");

        _mainSceneContainer.UnbindId<RectTransform>("CoinsAdvisePoint");
        _mainSceneContainer.UnbindId<RectTransform>("ObstacleCharacteristicsAdvisePoint");
        _mainSceneContainer.UnbindId<RectTransform>("BuyButtonPalmPointerPosition");
        _mainSceneContainer.UnbindId<RectTransform>("SkipButtonInStorePosition");

        _mainSceneContainer.UnbindId<RectTransform>("TapToBuyAdvisePoint");
        _mainSceneContainer.UnbindId<RectTransform>("NewSkinAdvisePoint");
        _mainSceneContainer.UnbindId<RectTransform>("NewCharacterAdvisePoint");
        
        
        
        _mainSceneContainer.Bind<ObstaclesStoreUIState>().FromInstance(obstaclesStoreUiState);
        _mainSceneContainer.Bind<BulletSkinStoreUIState>().FromInstance(bulletSkinStoreUiState);
        _mainSceneContainer.Bind<BulletStoreUIState>().FromInstance(bulletStoreUiState);
        
        _mainSceneContainer.Bind<Button>().WithId("StoreBuyButton").FromInstance(storeBuyButton);
        _mainSceneContainer.Bind<Button>().WithId("StoreEditButton").FromInstance(storeEditButton);
        _mainSceneContainer.Bind<Button>().WithId("StoreExitButton").FromInstance(storeExitButton);
        
        _mainSceneContainer.Bind<Image>().WithId("StoreSelectedImage").FromInstance(storeSelectedImage);
        _mainSceneContainer.Bind<Image>().WithId("StoreObstacleCharacteristicsImage")
            .FromInstance(storeObstacleCharacteristicsImage);
        
        _mainSceneContainer.Bind<RectTransform>().WithId("CoinsAdvisePoint").FromInstance(coinsAdvisePoint);
        _mainSceneContainer.Bind<RectTransform>().WithId("ObstacleCharacteristicsAdvisePoint").FromInstance(obstacleCharacteristicsAdvisePoint);
        _mainSceneContainer.Bind<RectTransform>().WithId("BuyButtonPalmPointerPosition").FromInstance(buyButtonPalmPointerPosition);
        _mainSceneContainer.Bind<RectTransform>().WithId("SkipButtonInStorePosition").FromInstance(skipButtonInStorePosition);
        
        _mainSceneContainer.Bind<RectTransform>().WithId("TapToBuyAdvisePoint").FromInstance(tapToBuyAdvisePoint);
        _mainSceneContainer.Bind<RectTransform>().WithId("NewSkinAdvisePoint").FromInstance(newSkinAdvisePoint);
        _mainSceneContainer.Bind<RectTransform>().WithId("NewCharacterAdvisePoint").FromInstance(newCharacterAdvisePoint);
    }
}