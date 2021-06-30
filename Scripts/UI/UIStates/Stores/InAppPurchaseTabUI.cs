using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InAppPurchaseTabUI : MonoBehaviour, ITabUI
{
    public event Action<ITabUI> OnSelect = i => { };
    
    [SerializeField] private Image itemImage;

    [SerializeField] private TextMeshProUGUI purchaseNameText;

    [SerializeField] private Sprite selectedSprite;
    
    [SerializeField] private Button tabButton;

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
        itemImage.sprite = itemScriptableObject.storeMiniSprite;
        purchaseNameText.text = itemScriptableObject.StoreName.FirstLetterToUpperCase();
    }

    public void UpdateItemState(ItemScriptableObject itemScriptableObject, ItemState itemState)
    {
        
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

    public void DeSelect()
    {
        GetComponent<Image>().sprite = _defaultSprite;
    }

    public void SetInteractable(bool interactable)
    {
        tabButton.interactable = interactable;
        itemImage.color = interactable ? _normalItemImageColor : Constants.DisabledColor;
    }
}
