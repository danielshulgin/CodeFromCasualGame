using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;


public class ConfirmPanel : MonoBehaviour
{
    public event Action OnConfirmButtonClick = () => { };
    
    public event Action OnRejectButtonClick = () => { };
    
    public event Action OnCantBuy = () => { };
    
    [SerializeField] private Button confirmBuyButton;
    
    [SerializeField] private Button rejectBuyButton;
    
    [SerializeField] private TextMeshProUGUI confirmText;

    [SerializeField] protected CanvasGroup BuyPanelCanvasGroup;

    [SerializeField] private PriceUI priceUI;
    
    [Inject] private PlayerData _playerData;

    private int _selectedId;

    private Action _confirmButtonClickAction;
    
    private Action _rejectButtonClickAction;

    private bool _itemMode;


    public void Initialize(ItemScriptableObject itemScriptableObject, Action confirmButtonClickAction,
        Action rejectButtonClickAction, bool itemMode = true)
    {
        _itemMode = itemMode;
        _selectedId = itemScriptableObject.id;
        UIHelperFunctions.SetActiveCanvasGroup(BuyPanelCanvasGroup, true);
        
        _rejectButtonClickAction = rejectButtonClickAction;
        _confirmButtonClickAction = confirmButtonClickAction;
        
        if (_playerData.ItemStateById[itemScriptableObject.id] == ItemState.Available && 
            _playerData.CanBuyItem(itemScriptableObject.id))
        {
            confirmText.text = 
                $"{Localization.Instance.GetTranslation("confirm_buy_text")} {((IHaveStoreName)itemScriptableObject).StoreName}?";
            
            confirmBuyButton.gameObject.SetActive(true);
            rejectBuyButton.gameObject.SetActive(true);
            
            priceUI.Initialize(itemScriptableObject.price);
            
            AudioManager.Instance.PlayWithOverlay("ui_rushure");
        }
        else
        {
            var lackPrice = itemScriptableObject.price.CalculateLack(_playerData);
            priceUI.Initialize(lackPrice);
            
            confirmText.text = 
                $"{Localization.Instance.GetTranslation("money_lack_text")}";
            
            confirmBuyButton.gameObject.SetActive(false);
            rejectBuyButton.gameObject.SetActive(false);
            
            AudioManager.Instance.PlayWithOverlay("ui_nomoney");
            OnCantBuy();
        }
    }

    public void Initialize(string text, Action confirmButtonClickAction, Action rejectButtonClickAction)
    {
        _itemMode = false;
        
        UIHelperFunctions.SetActiveCanvasGroup(BuyPanelCanvasGroup, true);
        
        _rejectButtonClickAction = rejectButtonClickAction;
        _confirmButtonClickAction = confirmButtonClickAction;
        
        confirmText.text = text;
        
        confirmBuyButton.gameObject.SetActive(true);
        rejectBuyButton.gameObject.SetActive(true);
        priceUI.Hide();
    }
    
    public void RejectBuyButtonClick()
    {
        AudioManager.Instance.PlayWithOverlay("ui_exit");
        UIHelperFunctions.SetActiveCanvasGroup(BuyPanelCanvasGroup,false);
        OnRejectButtonClick();
        _rejectButtonClickAction?.Invoke();
    }

    public virtual void ConfirmBuyButtonClick()
    {
        AudioManager.Instance.PlayWithOverlay("ui_buy");
        if (_itemMode)
        {
            if (_playerData.CanBuyItem(_selectedId))
            {
                _playerData.BuyItem(_selectedId);
                OnConfirmButtonClick();
            }
        }
        UIHelperFunctions.SetActiveCanvasGroup(BuyPanelCanvasGroup, false);
        _confirmButtonClickAction?.Invoke();
    }
}
