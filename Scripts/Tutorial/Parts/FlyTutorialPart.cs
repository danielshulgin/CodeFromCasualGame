using System;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class FlyTutorialPart : TutorialPart
{
    [Foldout("Dependencies", true)]
    [SerializeField] private GameState gameState;
    
    [SerializeField] private ClickManager clickManager;

    [SerializeField] private ForceAdjuster forceAdjuster;
    
    [SerializeField] private RotateCarousel rotateCarousel;
        
    [SerializeField] private GameForceAdjusterRealization gameForceAdjusterRealization;
        
    [SerializeField] private TutorialForceAdjusterRealization tutorialForceAdjusterRealization;
    
    [SerializeField] private TutorialPalmPointer tutorialPalmPointer;
    
    [SerializeField] private TutorialPopUp tutorialPopUp;
 
    [Foldout("Game menu", true)]
    [SerializeField] private Button pauseButton;
    
    [SerializeField] private RectTransform desiredForcePointer;
    
    [SerializeField] private RectTransform forceBackgroundRect;
    
    [Foldout("End game menu", true)]
    [SerializeField] private Button retryButton;
    
    [SerializeField] private Button mainMenuButton;
    
    [SerializeField] private Button doubleResultButton;
    
    [SerializeField] private Button endGameMenuPanel;

    [Foldout("Scene objects", true)]
    [SerializeField] private Transform carouselTouchPoint;
    
    [SerializeField] private Transform vasylTouchPoint;
    
    [SerializeField] private GameObject fadePlane;
    
    [SerializeField] private SpriteRenderer carouselSpriteRenderer;
    
    [SerializeField] private SpriteRenderer vasylSpriteRenderer;

    [Foldout("Tutorial points", true)] 
    [SerializeField] private Transform startPopUpPoint;
    
    [SerializeField] private Transform holdPopUpPoint;
     
    
    [Foldout("Settings", true)]
    [SerializeField] private Button skipButton;

    [SerializeField] private RectTransform skipButtonNormalPosition;
    
    [SerializeField] private RectTransform skipButtonPauseButtonPosition;
    
    [SerializeField] private string ignoreFadeLayer = "ignore_fade";
    
    private string prevVasylLayer;
    
    private string prevCarouselLayer;
    
    
    [ButtonMethod]
    public override void Begin()
    {
        FadePanel.Instance.UnFade();
        
        prevVasylLayer = vasylSpriteRenderer.sortingLayerName;
        prevCarouselLayer = carouselSpriteRenderer.sortingLayerName;
        
        GameState.OnStartFly += EndShowVasylTouch;
        GameState.OnShowResults += ShowEndGameMenuExitButton;
        
        skipButton.gameObject.SetActive(true);
        skipButton.onClick.AddListener(SkipTutorial);
        SetButtonPosition(skipButton, skipButtonPauseButtonPosition);

        gameState.SendStartGame();
        ShowStartAdvise();
    }

    private void ShowStartAdvise()
    {
        carouselSpriteRenderer.sortingLayerName = ignoreFadeLayer;
        fadePlane.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        forceBackgroundRect.gameObject.SetActive(false);
        
        tutorialPopUp.ShowAdviseOnWorldPoint("fly_tutorial_start_advise", startPopUpPoint);
        
        clickManager.SetHandleInputTypes(InputTargetType.OnEndClick, true);
        clickManager.SetHandleInputTypes(InputTargetType.OnCarouselTouch, false);
        clickManager.SetHandleInputTypes(InputTargetType.OnVasylTouch, false);
        
        clickManager.OnEndClick.AddListener(EndShowStartAdvise);
    }
    
    private void EndShowStartAdvise()
    {
        forceBackgroundRect.gameObject.SetActive(true);
        
        clickManager.SetHandleInputTypes(InputTargetType.OnCarouselTouch, true);
        
        clickManager.OnEndClick.RemoveListener(EndShowStartAdvise);
        tutorialPopUp.Hide();
        ShowCarouselTouch();
    }

    private void ShowCarouselTouch()
    {
        tutorialPalmPointer.Initialize();
        InitializeDesiredForcePointer();
        
        tutorialPalmPointer.PlayPalmAnimationOnWordPoint(PalmPointerAnimationType.Press, carouselTouchPoint);
        tutorialPopUp.ShowAdviseOnWorldPoint("hold_advise", holdPopUpPoint, false);
        desiredForcePointer.gameObject.SetActive(true);

        forceAdjuster.BindForceAdjusterRealization(tutorialForceAdjusterRealization);

        tutorialForceAdjusterRealization.OnCarouselStartTouchEvent.AddListener(OnCarouselStartTouch);
        tutorialForceAdjusterRealization.OnCarouselEndTouchEvent.AddListener(OnCarouselEndTouch);
        tutorialForceAdjusterRealization.OnMaximumPointReachEvent.AddListener(EndShowCarouselTouch);
    }

    private void OnCarouselStartTouch()
    {
        tutorialPalmPointer.Hide();
        tutorialPopUp.Hide();
    }

    private void OnCarouselEndTouch()
    {
        tutorialPalmPointer.PlayPalmAnimationOnWordPoint(PalmPointerAnimationType.Press, carouselTouchPoint);
        tutorialPopUp.ShowAdviseOnWorldPoint("hold_advise", holdPopUpPoint, false);
    }
    
    private void EndShowCarouselTouch()
    {
        carouselSpriteRenderer.sortingLayerName = prevCarouselLayer;
        tutorialForceAdjusterRealization.OnCarouselStartTouchEvent.RemoveListener(OnCarouselStartTouch);
        tutorialForceAdjusterRealization.OnCarouselEndTouchEvent.RemoveListener(OnCarouselEndTouch);
        tutorialForceAdjusterRealization.OnMaximumPointReachEvent.RemoveListener(EndShowCarouselTouch);
        
        clickManager.SetHandleInputTypes(InputTargetType.OnVasylTouch, true);
        
        desiredForcePointer.gameObject.SetActive(false);
        ShowVasylTouch();
    }

    private void ShowVasylTouch()
    {
        tutorialPopUp.Hide();
        
        rotateCarousel.Freeze();
        tutorialPalmPointer.PlayPalmAnimationOnWordPoint(PalmPointerAnimationType.Tap, vasylTouchPoint);
        
        vasylSpriteRenderer.sortingLayerName = ignoreFadeLayer;
    }
    
    private void EndShowVasylTouch()
    {
        fadePlane.SetActive(false);
        
        tutorialPalmPointer.Hide();
        vasylSpriteRenderer.sortingLayerName = prevVasylLayer;
    }

    private void ShowEndGameMenuExitButton()
    {
        pauseButton.gameObject.SetActive(true);
        endGameMenuPanel.interactable = false;
        retryButton.interactable = false;
        mainMenuButton.interactable = true;
        doubleResultButton.interactable = false;
        
        mainMenuButton.onClick.AddListener(EndShowEndGameMenuExitButton);
        
        tutorialPalmPointer.PlayPalmAnimationOnUI(PalmPointerAnimationType.Press, 
            mainMenuButton.GetComponent<RectTransform>());
    }
    
    private void EndShowEndGameMenuExitButton()
    {
        retryButton.interactable = true;
        mainMenuButton.interactable = true;
        doubleResultButton.interactable = true;
        endGameMenuPanel.interactable = true;
        
        mainMenuButton.onClick.RemoveListener(EndShowEndGameMenuExitButton);
        End();
    }

    private void End()
    {
        FadePanel.Instance.Stop();
        
        GameState.OnStartFly -= EndShowVasylTouch;
        GameState.OnShowResults -= ShowEndGameMenuExitButton;
        
        skipButton.onClick.RemoveListener(SkipTutorial);
        skipButton.gameObject.SetActive(false);
        SetButtonPosition(skipButton, skipButtonNormalPosition);
        
        forceAdjuster.BindForceAdjusterRealization(gameForceAdjusterRealization);
        
        OnEnd?.Invoke();
    }
    
    private void SkipTutorial()
    {
        FadePanel.Instance.Stop();
        
        OnEnd?.Invoke();
    }
    
    private void InitializeDesiredForcePointer()
    {
        desiredForcePointer.gameObject.SetActive(true);
        var arrowOffset = forceBackgroundRect.rect.width * tutorialForceAdjusterRealization.StopPoint;
        var arrowPosition = desiredForcePointer.transform.localPosition;
        desiredForcePointer.gameObject.transform.localPosition = new Vector3(arrowPosition.x + arrowOffset,
            arrowPosition.y);
    }

    private void OnDestroy()
    {
        GameState.OnStartFly -= EndShowVasylTouch;
        GameState.OnShowResults -= ShowEndGameMenuExitButton;
    }
}
