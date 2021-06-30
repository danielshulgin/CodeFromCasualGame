using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class BuyStackItemsPanel : MonoBehaviour
{
    [SerializeField] private Button confirmBuyButton;
    
    [SerializeField] private TMP_InputField inputField;
    
    [SerializeField] private Slider slider;

    [SerializeField] protected CanvasGroup panelCanvasGroup;

    [SerializeField] private PriceUI priceUI;
    
    [Inject] private PlayerData _playerData;
    
    private StackTypeScriptableObject _currentStackTypeScriptableObject;
    
    private int _buyNumber;
    
    private int _maxBuyNumber;
    
    private Action<int> _confirmButtonClickAction;

    
    private void Awake()
    {
        inputField.shouldHideMobileInput = true;
    }

    public void Initialize(StackTypeScriptableObject stackTypeScriptableObject, Action<int> confirmButtonClickAction)
    {
        UIHelperFunctions.SetActiveCanvasGroup(panelCanvasGroup, true);

        _confirmButtonClickAction = confirmButtonClickAction;
        _currentStackTypeScriptableObject = stackTypeScriptableObject;
        
        _maxBuyNumber = GetMaxBuyNumber(stackTypeScriptableObject);
        slider.maxValue = _maxBuyNumber;
        
        if (_playerData.Coins < stackTypeScriptableObject.price.coins || _playerData.Crystals < stackTypeScriptableObject.price.crystals)
        {
            confirmBuyButton.interactable = false;
            SetBuyNumber(0);
            priceUI.Initialize(new Currency(0, 0, _currentStackTypeScriptableObject.price.currencyType));
        }
        else
        {
            confirmBuyButton.interactable = true;
            SetBuyNumber(1);
            priceUI.Initialize(_currentStackTypeScriptableObject.price);
        }
    }

    private int GetMaxBuyNumber(StackTypeScriptableObject stackTypeScriptableObject)
    {
        var maxBuyNumberByCoins = 0;
        if (stackTypeScriptableObject.price.coins > 0)
        {
            maxBuyNumberByCoins = _playerData.Coins / stackTypeScriptableObject.price.coins;
        }
        
        var maxBuyNumberByCrystals = 0;
        if (stackTypeScriptableObject.price.crystals > 0)
        {
            maxBuyNumberByCrystals = _playerData.Crystals / stackTypeScriptableObject.price.crystals;
        }

        if (stackTypeScriptableObject.price.currencyType == CurrencyType.CoinsCrystals)
        {
            return maxBuyNumberByCoins < maxBuyNumberByCrystals ? maxBuyNumberByCoins : maxBuyNumberByCrystals;
        }
        if (stackTypeScriptableObject.price.currencyType == CurrencyType.Crystals)
        {
            return maxBuyNumberByCrystals;
        }
        return maxBuyNumberByCoins;
    }

    public void ExitButtonClick()
    {
        UIHelperFunctions.SetActiveCanvasGroup(panelCanvasGroup, false);
    }

    public virtual void ConfirmBuyButtonClick()
    {
        _confirmButtonClickAction.Invoke(_buyNumber);
        UIHelperFunctions.SetActiveCanvasGroup(panelCanvasGroup, false);
    }

    public void HandleSliderValueChanged()
    {
        priceUI.Initialize(_currentStackTypeScriptableObject.price * ((int)slider.value));
        inputField.SetTextWithoutNotify(slider.value.ToString());
        _buyNumber = (int)slider.value;
    }

    public void HandleInputFiledValueChanged(string value)
    {
        var intValue = 0;
        if (value != "")
        {
            intValue = int.Parse(value);
        }
        else
        {
            return;
        }
        
        if (!SetBuyNumber(intValue))
        {
            inputField.SetTextWithoutNotify(_buyNumber.ToString());
        }
    }

    public void HandleIncreaseButtonClick()
    {
        SetBuyNumber(_buyNumber + 1);
    }

    public void HandleDecreaseButtonClick()
    {
        SetBuyNumber(_buyNumber - 1);
    }

    private bool SetBuyNumber(int number)
    {
        if (number < 0 || number > _maxBuyNumber)
        {
            return false;
        }
        
        _buyNumber = number;
        priceUI.Initialize(_currentStackTypeScriptableObject.price * _buyNumber);
        inputField.SetTextWithoutNotify(_buyNumber.ToString());
        slider.SetValueWithoutNotify(_buyNumber);
        
        return true;
    }
}
