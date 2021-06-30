using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class BuyTrapTutorialPart : TutorialPart
{
    [Foldout("Dependencies", true)]
    [SerializeField] private ClickManager clickManager;

    [SerializeField] private TutorialForceAdjusterRealization tutorialForceAdjusterRealization;
    
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
    
    [SerializeField] private SpriteRenderer vasylSpriteRenderer;

    [Foldout("CoinsCrystalsPanel", true)]
    [SerializeField] private Button coinsCrystalsButton;
    
    [SerializeField] private Button rewardedVideoButton;

    [Foldout("ConfirmPanel", true)]
    [SerializeField] private Button confirmButton;
    
    [SerializeField] private Button rejectButton;
    
    [SerializeField] private Button confirmPanelBackground;
    
    [Foldout("Tutorial points", true)]
    [SerializeField] private RectTransform exitButtonAdvisePoint;
    
    [Foldout("Settings", true)]
    [SerializeField] private Button skipButton;
    
    [SerializeField] private RectTransform trapStorePalmPointerPosition;

    [SerializeField] private ObstacleScriptableObject firstTrapScriptableObject;

#region StoreDependencies
    [Inject] private LazyInject<ObstaclesStoreUIState> _obstaclesStoreUiState;

    [Inject(Id = "StoreBuyButton")] private LazyInject<Button> _storeBuyButton;
    
    [Inject(Id = "StoreExitButton")] private LazyInject<Button> _storeExitButton;
    
    [Inject(Id = "StoreSelectedImage")] private LazyInject<Image> _storeSelectedImage;
    
    [Inject(Id = "StoreObstacleCharacteristicsImage")] private LazyInject<Image> _storeObstacleCharacteristicsImage;
    
    [Inject(Id = "CoinsAdvisePoint")] private LazyInject<RectTransform> _coinsAdvisePoint;
    
    [Inject(Id = "ObstacleCharacteristicsAdvisePoint")] private LazyInject<RectTransform> _obstacleCharacteristicsAdvisePoint;
    
    [Inject(Id = "BuyButtonPalmPointerPosition")] private LazyInject<RectTransform> _buyButtonPalmPointerPosition;
    
    [Inject(Id = "SkipButtonInStorePosition")] private LazyInject<RectTransform> _skipButtonInStorePosition;
#endregion
    
    [Inject] private PlayerData _playerData;


    [ButtonMethod]
    public override void Begin()
    {
        if (_playerData.ItemStateById[firstTrapScriptableObject.id] != ItemState.Available)
        {
            SkipTutorial();
            return;
        }
        
        AudioManager.Instance.Stop("theme_menu");
        FadePanel.Instance.UnFade();
        
        tutorialPalmPointer.Initialize();
        tutorialPalmPointer.Hide();
        
        skipButton.gameObject.SetActive(true);
        skipButton.onClick.AddListener(SkipTutorial);
        
        ShowTrapButton();
    }
    
    private void ShowTrapButton()
    {
        SetInteractableGameView(false);
        SetInteractableMainMenuUI(false);
        trapStoreButton.interactable = true;
        
        tutorialPalmPointer.PlayPalmAnimationOnUI(PalmPointerAnimationType.Tap, trapStorePalmPointerPosition);
        
        trapStoreButton.onClick.AddListener(EndShowTrapButton);
    }
    
    private void EndShowTrapButton()
    {
        SetInteractableGameView(true);
        SetInteractableMainMenuUI(true);
        
        trapStoreButton.onClick.RemoveListener(EndShowTrapButton);
        
        Invoke(nameof(ShowCoins), 0.1f);
    }
    
    private void ShowCoins()
    {
        skipButton.GetComponent<RectTransform>().anchoredPosition = _skipButtonInStorePosition.Value.anchoredPosition;
        
        tutorialPopUp.ShowAdviseOnUI("coins_crystals_advise", _coinsAdvisePoint.Value);
        
        SetInteractableStoreUI(false);
        UIHelperFunctions.ChangeDisabledButtonColor(coinsCrystalsButton, Constants.NormalColor);
        
        clickManager.OnEndClick.AddListener(EndShowCoins);
    }
    
    private void EndShowCoins()
    {
        tutorialPopUp.Hide();
        
        UIHelperFunctions.ChangeDisabledButtonColor(coinsCrystalsButton, Constants.DisabledColor);
        
        clickManager.OnEndClick.RemoveListener(EndShowCoins); 

        ShowBuy();
    }

    private void ShowBuy()
    {
        _storeBuyButton.Value.interactable = true;
        _storeSelectedImage.Value.color = Constants.NormalColor;
        UIHelperFunctions.ChangeDisabledButtonColor(_storeExitButton.Value, Constants.NormalColor);
        _obstaclesStoreUiState.Value.SetInteractableTab(0,true);
        
        _storeBuyButton.Value.onClick.AddListener(EndShowBuy);
        
        tutorialPalmPointer.PlayPalmAnimationOnUI(PalmPointerAnimationType.Tap, _buyButtonPalmPointerPosition.Value);
    }
    
    private void EndShowBuy()
    {
        SetInteractableStoreUI(false);
        UIHelperFunctions.ChangeDisabledButtonColor(_storeExitButton.Value, Constants.DisabledColor);
        _storeBuyButton.Value.onClick.RemoveListener(EndShowBuy);

        ShowConfirmDialog();
    }
    
    private void ShowConfirmDialog()
    {
        rejectButton.interactable = false;
        confirmPanelBackground.interactable = false;
        
        confirmButton.onClick.AddListener(EndConfirmDialog);
        
        tutorialPalmPointer.PlayPalmAnimationOnUI(PalmPointerAnimationType.Tap, confirmButton.GetComponent<RectTransform>());
    }
    
    private void EndConfirmDialog()
    {
        tutorialPalmPointer.Hide();
        rejectButton.interactable = true;
        confirmPanelBackground.interactable = true;

        confirmButton.onClick.RemoveListener(EndConfirmDialog);
        
        ShowObstacleCharacteristics();
    }
    
    private void ShowObstacleCharacteristics()
    {
        _obstaclesStoreUiState.Value.SetInteractableCharacteristicsPanel(true);
        tutorialPopUp.ShowAdviseOnUI("obstacle_characteristics_advise", _obstacleCharacteristicsAdvisePoint.Value);
        Invoke(nameof(OnEndClickAddListener), 0.1f);
    }

    private void OnEndClickAddListener()
    {
        clickManager.OnEndClick.AddListener(EndShowObstacleCharacteristics);
    }
    
    private void EndShowObstacleCharacteristics()
    {
        _obstaclesStoreUiState.Value.SetInteractableCharacteristicsPanel(false);
        tutorialPopUp.Hide();
        _storeObstacleCharacteristicsImage.Value.color = Constants.DisabledColor;
        clickManager.OnEndClick.RemoveListener(EndShowObstacleCharacteristics);

        ShowExitButton();
    }

    private void ShowExitButton()
    {
        clickManager.SetHandleInputTypes(InputTargetType.OnExitButtonClick, true);
        tutorialPopUp.ShowAdviseOnUI("exit_button_advise", exitButtonAdvisePoint);
        clickManager.OnExitButtonClick.AddListener(EndShowExitButton);
        clickManager.OnEndClick.AddListener(EndShowExitButton);
    }
    
    private void EndShowExitButton()
    {
        clickManager.OnEndClick.RemoveListener(EndShowExitButton);
        clickManager.SetHandleInputTypes(InputTargetType.OnEndClick, false);
        tutorialPopUp.Hide();
        clickManager.OnExitButtonClick.RemoveListener(EndShowExitButton);
        End();
    }

    private void End()
    {
        FadePanel.Instance.Stop();
        
        skipButton.onClick.RemoveListener(SkipTutorial);
        skipButton.gameObject.SetActive(false);
        
        SetInteractableStoreUI(true);
        
        uiStateMachine.ChangeState(maiMenuUiState);
        
        OnEnd?.Invoke();
    }
    
    private void SkipTutorial()
    {
        FadePanel.Instance.Stop();
        _playerData.BuyItem(firstTrapScriptableObject.id);
        OnEnd?.Invoke();
    }

    private void SetInteractableStoreUI(bool interactable)
    {
        clickManager.SetHandleInputTypes(InputTargetType.OnExitButtonClick, interactable);
        _obstaclesStoreUiState.Value.SetInteractable(interactable);
    }
    
    private void SetInteractableMainMenuUI(bool interactable)
    {
        leftButton.interactable = interactable;
        rightButton.interactable = interactable;
        startButton.interactable = interactable;
        coinsCrystalsButton.interactable = interactable;
        rewardedVideoButton.interactable = interactable;
        startButtonImage.color = interactable ? Constants.NormalColor : Constants.DisabledColor;
    }

    private void SetInteractableGameView(bool interactable)
    {
        clickManager.enabled = interactable;
        fadePlane.SetActive(!interactable);
    }
}
