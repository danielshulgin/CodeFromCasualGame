using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class StartGameTutorialPart : TutorialPart
{
    [Foldout("Dependencies", true)]
    [SerializeField] private ClickManager clickManager;

    [SerializeField] private TutorialPalmPointer tutorialPalmPointer;
    
    [SerializeField] private TutorialPopUp tutorialPopUp;

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

    [Foldout("Tutorial points", true)] 
    [SerializeField] private RectTransform tapToStartAdvisePoint;

    [Foldout("Settings", true)]

    [SerializeField] private Button skipButton;

    private string _prevVasylLayer;
    
    private string _prevCarouselLayer;


    [ButtonMethod]
    public override void Begin()
    {
        FadePanel.Instance.UnFade();
        
        _prevVasylLayer = vasylSpriteRenderer.sortingLayerName;
        _prevCarouselLayer = carouselSpriteRenderer.sortingLayerName;
        
        tutorialPalmPointer.Initialize();
        tutorialPalmPointer.Hide();
        
        skipButton.gameObject.SetActive(false);

        ShowStartButton();
    }
    
    private void ShowStartButton()
    {
        SetInteractableGameView(false);
        SetInteractableMainMenuUI(false);
        
        startButton.interactable = true;
        startButtonImage.color = Constants.NormalColor;
        
        tutorialPopUp.ShowAdviseOnUI("start_game_advise", tapToStartAdvisePoint);
        tutorialPalmPointer.PlayPalmAnimationOnUI(PalmPointerAnimationType.Tap, startButton.GetComponent<RectTransform>());
        startButton.onClick.AddListener(EndShowStartButton);
        clickManager.SetHandleInputTypes(InputTargetType.OnEndClick, true);
        clickManager.OnEndClick.AddListener(EndShowStartButton);
    }
    
    private void EndShowStartButton()
    {
        tutorialPopUp.Hide();
        tutorialPalmPointer.Hide();
        
        SetInteractableGameView(true);
        SetInteractableMainMenuUI(true);
        clickManager.OnEndClick.RemoveListener(EndShowStartButton);
        startButton.onClick.RemoveListener(EndShowStartButton);
        End();
    }

    private void End()
    {
        FadePanel.Instance.Stop();

        OnEnd.Invoke();
    }
    
    private void SkipTutorial()
    {
        FadePanel.Instance.Stop();
        
        tutorialPopUp.Hide();
        tutorialPalmPointer.Hide();
        
        SetInteractableGameView(true);
        SetInteractableMainMenuUI(true);
        startButton.onClick.RemoveListener(EndShowStartButton);
        clickManager.OnEndClick.RemoveListener(EndShowStartButton);
        OnEnd.Invoke();
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
