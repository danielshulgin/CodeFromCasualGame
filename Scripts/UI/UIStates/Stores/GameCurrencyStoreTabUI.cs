using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameCurrencyStoreTabUI : MonoBehaviour, ITabUI
{
    public event Action<ITabUI> OnSelect = i => { };
    
    [SerializeField] private Image itemImage;
    
    [SerializeField] protected Image stateImage;

    [SerializeField] private Sprite lockItemSprite;
    
    [FormerlySerializedAs("enableItemSprite")] [SerializeField] protected Sprite checkMarkSprite;

    [SerializeField] private Sprite selectedSprite;

    [SerializeField] private PriceUI priceUI;

    [SerializeField] private Button tabButton;
    
    [SerializeField] private TextMeshProUGUI itemText;
    
    private Color _normalItemImageColor;

    private Sprite _defaultSprite;

    public ItemScriptableObject ItemScriptableObject { get; private set; }


    private void Awake()
    {
        _defaultSprite = GetComponent<Image>().sprite;
        _normalItemImageColor = itemImage.color;
    }

    public void UpdateTab(ItemScriptableObject itemScriptableObject, ItemState itemState)
    {
        ItemScriptableObject = itemScriptableObject;
        UpdateItemState(itemScriptableObject, itemState);
        itemImage.sprite = itemScriptableObject.storeMiniSprite;
        itemText.text = itemScriptableObject.StoreName.FirstLetterToUpperCase();
    }

    public virtual void UpdateItemState(ItemScriptableObject itemScriptableObject, ItemState itemState)
    {
        switch (itemState)
        {
            case ItemState.Bought:
                priceUI.Hide();
                UIHelperFunctions.SetVisibleImage(stateImage, false);
                break;
            case ItemState.Available:
                UIHelperFunctions.SetVisibleImage(stateImage, false);
                priceUI.Initialize(itemScriptableObject.price);
                break;
            case ItemState.Locked:
                priceUI.Hide();
                UIHelperFunctions.SetVisibleImage(stateImage, true);
                stateImage.sprite = lockItemSprite;
                break;
        }
    }

    public void Select()
    {
        AudioManager.Instance.PlayWithOverlay("ui_button");
        SelectWithoutSound();
    }

    public void SelectWithoutSound()
    {
        OnSelect(this);
        GetComponent<Image>().sprite = selectedSprite;
    }

    public virtual void EnableCheckMark()
    {
        priceUI.Hide();
        UIHelperFunctions.SetVisibleImage(stateImage, true);
        stateImage.sprite = checkMarkSprite;
    }
    
    public void DeSelect()
    {
        GetComponent<Image>().sprite = _defaultSprite;
    }

    public void SetInteractable(bool interactable)
    {
        tabButton.interactable = interactable;
        
        if (interactable)
        {
            var color = _normalItemImageColor;
            itemImage.color = color;
            stateImage.color = new Color(color.r, color.g, color.b, stateImage.color.a);
            GetComponent<Image>().color = color;
        }
        else
        {
            var color = Constants.DisabledColor;
            itemImage.color = color;
            stateImage.color = new Color(color.r, color.g, color.b, stateImage.color.a);
            GetComponent<Image>().color = color;
        }
    }
}

