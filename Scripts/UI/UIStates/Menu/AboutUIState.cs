using UnityEngine;
using Zenject;


public class AboutUIState : UIState
{
    public override void Enter()
    {
        SetActiveCanvasGroup(_canvasGroup, true);
    }

    public override void Exit()
    {
        SetActiveCanvasGroup(_canvasGroup, false);
    }

    //TODO remove
    [SerializeField] private int addsCoins = 10000;
    
    [SerializeField] private int addsCrystals = 100;

    [Inject] private PlayerData _playerData;
    
    public void AddCoins()
    {
        _playerData.AddCurrency(new Currency(addsCoins, 0, CurrencyType.Coins));
    }
    
    public void AddCrystals()
    {
        _playerData.AddCurrency(new Currency(0, addsCrystals, CurrencyType.Crystals));
    }
    //

    public override void ExitButtonClick()
    {
        _uiStateMachine.ShowSettingsUIState();
    }
}
