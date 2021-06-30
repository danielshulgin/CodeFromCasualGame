using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class EndGameUIState : UIState
{
    public event Action OnEnter;
    
    public event Action OnExit;
    
    [SerializeField] private GameState gameState;
    
    [SerializeField] private UnityAdsButton doubleResultsButton;

    [SerializeField] private FlyResults flyResults;
    
    [SerializeField] private MainMenuUIState mainScreenUiState;
 
    [SerializeField] private TextMeshProUGUI coinsText;
    
    [SerializeField] private TextMeshProUGUI crystalsText;

    [SerializeField] private TextMeshProUGUI pathText;
    
    [SerializeField] private TextMeshProUGUI hitsText;
    
    [SerializeField] private Button unityAdsButton;

    
    protected override void Start()
    {
        base.Start();
        FlyResults.OnChangeResults += UpdateResults;
    }

    private void OnDestroy()
    {
        FlyResults.OnChangeResults -= UpdateResults;
    }

    public override void Enter()
    {
        SetActiveCanvasGroup(_canvasGroup, true);
        pathText.text = ((int)flyResults.PathLength).ToString();
        hitsText.text = (flyResults.Hits).ToString();
        UpdateResults();
        unityAdsButton.interactable = true;
        OnEnter?.Invoke();
        doubleResultsButton.TryOn();
    }

    public override void Exit()
    {
        SetActiveCanvasGroup(_canvasGroup, false);
        unityAdsButton.interactable = false;
        //TODO
        OnExit?.Invoke();
    }

    public void MainMenuButtonClick()
    {
        gameState.SendResetGame();
        _uiStateMachine.ChangeState(mainScreenUiState);
    }
    
    public void RetryButtonClick()
    {
        gameState.SendResetGame();
        gameState.SendStartGame();
    }

    private void UpdateResults()
    {
        coinsText.text = flyResults.Result.coins.ToString();
        crystalsText.text = flyResults.Result.crystals.ToString();
    }
}
