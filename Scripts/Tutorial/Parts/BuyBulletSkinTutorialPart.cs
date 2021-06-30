using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class BuyBulletSkinTutorialPart : TutorialPart
{
    [Foldout("Dependencies", true)]
    [SerializeField] private ClickManager clickManager;

    [SerializeField] private TutorialPalmPointer tutorialPalmPointer;
    
    [SerializeField] private TutorialPopUp tutorialPopUp;

    [SerializeField] private MainMenuUIState maiMenuUiState;
    
    [SerializeField] private UIStateMachine uiStateMachine;
    
    [Foldout("Main menu", true)]
    [SerializeField] private Button leftButton;
    
    [SerializeField] private Button rightButton;
    
    [SerializeField] private Button startButton;
    
    [SerializeField] private Button trapStoreButton;

    [SerializeField] private Image startButtonImage;
    
    [Foldout("Scene objects", true)]
    [SerializeField] private GameObject fadePlane;
    
    [SerializeField] private SpriteRenderer carouselSpriteRenderer;

    [Foldout("CoinsCrystalsPanel", true)]
    [SerializeField] private Button coinsCrystalsButton;
    
    [SerializeField] private Button rewardedVideoButton;
    
    [Foldout("Settings", true)]
    [SerializeField] private Button skipButton;
    
    [SerializeField] private string ignoreFadeLayer = "ignore_fade";
    
    [SerializeField] private BulletScriptableObject secondBulletScriptableObject;
    
    [SerializeField] private BulletSkinScriptableObject secondBulletSkinScriptableObject;

    [SerializeField] private Transform storeButtonPoint;

    [Inject] private PlayerData _playerData;

    #region StoreDependencies
    [Inject] private LazyInject<BulletStoreUIState> _bulletStoreUI;
    
    [Inject] private LazyInject<BulletSkinStoreUIState> _bulletSkinStoreUIState;
    
    [Inject(Id = "StoreBuyButton")] private LazyInject<Button> _storeBuyButton;
    
    [Inject(Id = "StoreEditButton")] private LazyInject<Button> _storeEditButton;
    
    [Inject(Id = "TapToBuyAdvisePoint")] private LazyInject<RectTransform> _tapToBuyAdvisePoint;
    
    [Inject(Id = "NewSkinAdvisePoint")] private LazyInject<RectTransform> _newSkinAdvisePoint;
     
    [Inject(Id = "NewCharacterAdvisePoint")] private LazyInject<RectTransform> _newCharacterAdvisePoint;
    
    [Inject(Id = "SkipButtonInStorePosition")] private LazyInject<RectTransform> _skipButtonInStorePosition;
    #endregion

    private string _prevCarouselLayer;


    [ButtonMethod]
    public override void Begin()
    {
        /*if (playerData.ItemStateById[secondBulletScriptableObject.id] != ItemState.Available 
            && playerData.ItemStateById[secondBulletSkinScriptableObject.id] != ItemState.Available )
        {
            SkipTutorial();
        }*/
        
        FadePanel.Instance.UnFade();
        AudioManager.Instance.Stop("theme_menu");
        
        _prevCarouselLayer = carouselSpriteRenderer.sortingLayerName;
        
        tutorialPalmPointer.Initialize();
        tutorialPalmPointer.Hide();
        
        skipButton.gameObject.SetActive(true);
        skipButton.onClick.AddListener(SkipTutorial);

        ShowBulletButton();
    }
    
    private void ShowBulletButton()
    {
        carouselSpriteRenderer.sortingLayerName = ignoreFadeLayer;
        SetInteractableGameView(false);
        SetInteractableMainMenuUI(false);
        clickManager.SetHandleInputTypes(InputTargetType.OnBulletTouch, true);
        
        tutorialPalmPointer.PlayPalmAnimationOnWordPoint(PalmPointerAnimationType.Tap, storeButtonPoint);
        
        //clickManager.OnBulletTouch.AddListener(EndShowBulletButton);
    }
    
    private void EndShowBulletButton()
    {
        carouselSpriteRenderer.sortingLayerName = _prevCarouselLayer;
        tutorialPalmPointer.Hide();
        
        SetInteractableGameView(true);
        SetInteractableMainMenuUI(true);

        //clickManager.OnBulletTouch.RemoveListener(EndShowBulletButton);
        Invoke(nameof(ShowBulletInStoreUI), 0.1f);
    }

    private void ShowBulletInStoreUI()
    {
        skipButton.GetComponent<RectTransform>().anchoredPosition = _skipButtonInStorePosition.Value.anchoredPosition;
        tutorialPopUp.ShowAdviseOnUI("new_character_advise", _newCharacterAdvisePoint.Value);
        
        _bulletStoreUI.Value.SelectTab(1);
        SetInteractableBulletStoreUI(false);
        _bulletStoreUI.Value.SetInteractableTab(1, true);
        
        clickManager.OnEndClick.AddListener(EndShowCharacterInStoreUI);
    }
    
    private void EndShowCharacterInStoreUI()
    {
        tutorialPopUp.Hide();
        _bulletStoreUI.Value.SetInteractableTab(1, false);
        
        clickManager.OnEndClick.RemoveListener(EndShowCharacterInStoreUI);
        
        ShowEditButton();
    }
    
    //TODO
    private void ShowCharacterBuyButton()
    {
        tutorialPopUp.ShowAdviseOnUI("tap_to_buy_advise", _tapToBuyAdvisePoint.Value);
        
        _bulletStoreUI.Value.SelectTab(1);
        SetInteractableBulletStoreUI(false);
        _bulletStoreUI.Value.SetInteractableTab(1, true);
        _storeBuyButton.Value.interactable = true;
        
        clickManager.OnEndClick.AddListener(EndShowCharacterBuyButton);
    }
    
    private void EndShowCharacterBuyButton()
    {
        tutorialPopUp.Hide();
        
        _bulletStoreUI.Value.SetInteractableTab(1, false);
        _storeBuyButton.Value.interactable = false;
        
        clickManager.OnEndClick.RemoveListener(EndShowCharacterBuyButton);
        
        ShowEditButton();
    }

    private void ShowEditButton()
    {
        _bulletStoreUI.Value.SelectTab(0);
        _bulletStoreUI.Value.SetInteractableTab(0, true);
        _storeEditButton.Value.interactable = true;
        
        tutorialPalmPointer.PlayPalmAnimationOnUI(PalmPointerAnimationType.Tap, _storeEditButton.Value.gameObject.GetComponent<RectTransform>());
        
        _storeEditButton.Value.onClick.AddListener(EndShowEditButton);
    }
    
    private void EndShowEditButton()
    {
        _storeEditButton.Value.onClick.RemoveListener(EndShowEditButton);
        _storeEditButton.Value.interactable = false;
        
        tutorialPalmPointer.Hide();
        
        Invoke(nameof(ShowCharacterSkin), 0.1f);
    }

    private void ShowCharacterSkin()
    {
        Debug.Log(_tapToBuyAdvisePoint.Value == null);
        tutorialPopUp.ShowAdviseOnUI("new_skin_advise", _newSkinAdvisePoint.Value);
        SetInteractableBulletSkinStoreUI(false);
        _bulletSkinStoreUIState.Value.SetInteractableTab(0, true);

        UIHelperFunctions.ChangeDisabledButtonColor(_storeBuyButton.Value, Constants.NormalColor);
        
        clickManager.OnEndClick.AddListener(EndShowCharacterSkin);
    }
    
    private void EndShowCharacterSkin()
    {
        tutorialPopUp.Hide();

        _bulletSkinStoreUIState.Value.SetInteractableTab(1, false);
        
        clickManager.OnEndClick.RemoveListener(EndShowCharacterSkin);
        
        ShowCharacterSkinBuyButton();
        End();
    }
    
    private void ShowCharacterSkinBuyButton()
    {
        tutorialPopUp.ShowAdviseOnUI("tap_to_buy_advise", _tapToBuyAdvisePoint.Value);
        
        _bulletSkinStoreUIState.Value.SetInteractableTab(1, true);
        _bulletSkinStoreUIState.Value.SelectTab(1);
        
        UIHelperFunctions.ChangeDisabledButtonColor(_storeBuyButton.Value, Constants.NormalColor);
        
        clickManager.OnEndClick.AddListener(EndShowCharacterSkinBuyButton);
    }
    
    private void EndShowCharacterSkinBuyButton()
    {
        tutorialPopUp.Hide();
        UIHelperFunctions.ChangeDisabledButtonColor(_storeBuyButton.Value, Constants.DisabledColor);
        
        clickManager.OnEndClick.RemoveListener(EndShowCharacterSkinBuyButton);
        
        End();
    }
    //
    
    private void End()
    {
        FadePanel.Instance.Stop();

        skipButton.onClick.RemoveListener(SkipTutorial);
        skipButton.gameObject.SetActive(false);
        
        uiStateMachine.ChangeState(maiMenuUiState);
        OnEnd.Invoke();
    }
    
    private void SkipTutorial()
    {
        FadePanel.Instance.Stop();
        OnEnd?.Invoke();
    }
    
    private void SetInteractableBulletStoreUI(bool interactable)
    {
        clickManager.SetHandleInputTypes(InputTargetType.OnExitButtonClick, interactable);
        _bulletStoreUI.Value.SetInteractable(interactable);
    }
    
    private void SetInteractableBulletSkinStoreUI(bool interactable)
    {
        clickManager.SetHandleInputTypes(InputTargetType.OnExitButtonClick, interactable);
        _bulletSkinStoreUIState.Value.SetInteractable(interactable);
    }
    
    private void SetInteractableMainMenuUI(bool interactable)
    {
        leftButton.interactable = interactable;
        rightButton.interactable = interactable;
        startButton.interactable = interactable;
        trapStoreButton.interactable = interactable;
        coinsCrystalsButton.interactable = interactable;
        rewardedVideoButton.interactable = interactable;
        
        startButtonImage.color = interactable ? Constants.NormalColor : Constants.DisabledColor;
    }

    private void SetInteractableGameView(bool interactable)
    {
        var handleInputTypes = new Dictionary<InputTargetType, bool>
        {
            [InputTargetType.OnCarouselTouch] = interactable,
            [InputTargetType.OnVasylTouch] = interactable,
            [InputTargetType.OnCarouselEndTouch] = interactable,
            [InputTargetType.OnBulletTouch] = interactable,
            [InputTargetType.OnElkTouch] = interactable,
            [InputTargetType.OnDuckTouch] = interactable,
            [InputTargetType.OnSwipeRight] = interactable,
            [InputTargetType.OnSwipeLeft] = interactable,
            [InputTargetType.OnSwipe] = interactable,
            [InputTargetType.OnSwipeBegin] = interactable,
            [InputTargetType.OnSwipeEnd] = interactable,
            [InputTargetType.OnExitButtonClick] = interactable,
            [InputTargetType.OnEndClick] = interactable,
        };
        
        clickManager.SetHandleInputTypes(handleInputTypes);
        
        fadePlane.SetActive(!interactable);
    }
}
