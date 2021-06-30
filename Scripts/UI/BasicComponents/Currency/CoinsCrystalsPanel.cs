using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CoinsCrystalsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    
    [SerializeField] private TextMeshProUGUI crystalsText;

    [Inject] private PlayerData _playerData;
    
    [Inject] private UIStateMachine _uiStateMachine;


    private void Start()
    {
        PlayerData.OnPlayerCurrencyChange += HandlePlayerCurrencyChange;
        HandlePlayerCurrencyChange(new Currency(_playerData.Coins, _playerData.Crystals, CurrencyType.CoinsCrystals));
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        PlayerData.OnPlayerCurrencyChange -= HandlePlayerCurrencyChange;
    }

    public void HandlePlayerCurrencyChange(Currency playerCurrency)
    {
        coinsText.text = playerCurrency.coins.ToString();
        crystalsText.text = playerCurrency.crystals.ToString();
    }

    private void OnClick()
    {
        _uiStateMachine.ShowGenericStore(StoreType.IAP);
    }
}
