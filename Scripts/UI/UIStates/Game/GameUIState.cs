using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameUIState : UIState
{
    [SerializeField] private CanvasGroup forceBarCanvasGroup;
    
    [SerializeField] private GameState gameState;
    
    [SerializeField] private CanvasGroup pauseButtonCanvasGroup;
    
    [SerializeField] private CanvasGroup subMenuPanelCanvasGroup;

    [SerializeField] private MainMenuUIState mainScreenUiState;
    
    [SerializeField] private float pauseThemeDelay = 0.7f;

    private string _themeName;


    protected override void Awake()
    {
        base.Awake();
        GameState.OnStartFly += TurnOffForceBar;
    }

    private void OnDestroy()
    {
        GameState.OnStartFly -= TurnOffForceBar;
    }

    public override void Enter()
    {
        _themeName = $"theme_{Random.Range(1,3)}_lvl_1";
        AudioManager.Instance.Play(_themeName); //TODO handle switch level
        SetActiveCanvasGroup(_canvasGroup, true);
        SetActiveCanvasGroup(forceBarCanvasGroup, true);
        SetActiveCanvasGroup(pauseButtonCanvasGroup,true);
    }

    public override void Exit()
    {
        AudioManager.Instance.Stop(_themeName);
        AudioManager.Instance.Stop("theme_pause");
        StopAllCoroutines();
        SetActiveCanvasGroup(_canvasGroup, false);
        SetActiveCanvasGroup(pauseButtonCanvasGroup,false);
        SetActiveCanvasGroup(subMenuPanelCanvasGroup, false);
    }

    public void TurnOffForceBar()
    {
        SetActiveCanvasGroup(forceBarCanvasGroup, false);
    }

    public void PauseButtonClick()
    {
        StartCoroutine(PlayPauseThemeWithDelay());
        AudioManager.Instance.Pause(_themeName);
        gameState.SendPause();
        SetActiveCanvasGroup(pauseButtonCanvasGroup,false);
        SetActiveCanvasGroup(subMenuPanelCanvasGroup, true);
    }

    public void ContinueButtonClick()
    {
        AudioManager.Instance.Stop("theme_pause");
        StopAllCoroutines();
        AudioManager.Instance.UnPause(_themeName);
        gameState.SendContinue();
        SetActiveCanvasGroup(pauseButtonCanvasGroup,true);
        SetActiveCanvasGroup(subMenuPanelCanvasGroup, false);
    }
    
        
    public void RetryButtonClick()
    {
        AudioManager.Instance.Stop("theme_pause");
        StopAllCoroutines();
        gameState.SendContinue();
        gameState.SendEndFly();
        gameState.SendEndGame();
        gameState.SendResetGame();
        gameState.SendStartGame();
    }

    public void MainMenuButtonClick()
    {
        AudioManager.Instance.Stop("theme_pause");
        StopAllCoroutines();
        gameState.SendContinue();
        gameState.SendEndFly();
        gameState.SendEndGame();
        gameState.SendResetGame();
        _uiStateMachine.ChangeState(mainScreenUiState);
    }

    IEnumerator PlayPauseThemeWithDelay()
    {
        yield return new WaitForSecondsRealtime(pauseThemeDelay);
        AudioManager.Instance.Play("theme_pause");
    }
}
