using System;
using UnityEngine;

public class UIStateMachine : MonoBehaviour
{
    [SerializeField] private GameUIState gameUIState;
    
    [SerializeField] private AboutUIState aboutUIState;
    
    [SerializeField] private EndGameUIState endGameState;
    
    [SerializeField] private MainMenuUIState mainScreenUiState;

    [SerializeField] private UIState startState;
    
    [SerializeField] private ConfirmPanel confirmPanel;
    
    [SerializeField] private BuyStackItemsPanel buyStackItemsPanel;

    private UIState _currentState;
    
    public ConfirmPanel ConfirmPanel => confirmPanel;
    
    public BuyStackItemsPanel BuyStackItemsPanel => buyStackItemsPanel;

    
    private void Start()
    {
        startState.Enter();
        _currentState = startState;
        GameState.OnStartGame += ShowGameUI;
        GameState.OnResetGame += ShowMainMenu;
        GameState.OnShowResults += ShowResults;
    }

    private void OnDestroy()
    {
        GameState.OnStartGame -= ShowGameUI;
        GameState.OnResetGame -= ShowMainMenu;
        GameState.OnShowResults -= ShowResults;
    }

    private void Update()
    {
        _currentState.UpdateState();
    }

    public void ChangeState(UIState uiState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
            _currentState.OnExit.Invoke();
        }
        _currentState = uiState;
        _currentState.Enter();
        _currentState.OnEnter.Invoke();
    }

    public void ShowResults()
    {
        ChangeState(endGameState);
    }

    public void ShowGameUI()
    {
        ChangeState(gameUIState);
    }
    
    public void ShowSettingsUIState()
    {
        var settingsUIStatePrefab = Resources.Load("Prefabs/UIStates/SettingsUIState") as GameObject;
        var settingsUIStateInstance = Instantiate(settingsUIStatePrefab, transform);
        settingsUIStateInstance.transform.SetSiblingIndex(0);
        ChangeState(settingsUIStateInstance.GetComponent<SettingsUIState>());
    }

    public void ShowGenericStore(StoreType storeType)
    {
        var genericStorePrefab = Resources.Load("Prefabs/UIStates/GenericStorePanel") as GameObject;
        var genericStorePanelInstance = Instantiate(genericStorePrefab, FindObjectOfType<Canvas>().transform);
        genericStorePanelInstance.transform.SetSiblingIndex(0);
        var storeSelectorUIState = genericStorePanelInstance.GetComponent<StoreSelectorUIState>();
        ChangeState(storeSelectorUIState);
        storeSelectorUIState.OpenStore(storeType);
    }
    
    public void ShowAboutUI()
    {
        ChangeState(aboutUIState);
    }

    public void ShowMainMenu()
    {
        ChangeState(mainScreenUiState);
    }
    
    public void ShowAboutMenu()
    {
        var aboutUIStatePrefab = Resources.Load("Prefabs/UIStates/AboutPanel") as GameObject;
        var aboutUIStateInstance = Instantiate(aboutUIStatePrefab, transform);
        ChangeState(aboutUIStateInstance.GetComponent<AboutUIState>());
    }

    public void ExitButtonClick()
    {
        _currentState.ExitButtonClick();
    }
}
