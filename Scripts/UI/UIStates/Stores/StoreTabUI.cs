using System;
using UnityEngine;
using UnityEngine.UI;

public class StoreTabUI : MonoBehaviour
{
    public event Action<StoreTabUI> OnClick;
    
    [SerializeField] private StoreType storeType;
    
    [SerializeField] private Image backGroundImage;
    
    [SerializeField] private Sprite selectedSprite;

    private Sprite _defaultSprite;
    
    public StoreType StoreType => storeType;
    
    
    private void Awake()
    {
        _defaultSprite = backGroundImage.sprite;
    }
    
    public void HandleClick()
    {
        AudioManager.Instance.PlayWithOverlay("ui_button");
        OnClick?.Invoke(this);
        SetSprite(true);
    }

    public void SetSprite(bool selected)
    {
        if (selected)
        {
            backGroundImage.sprite = selectedSprite;    
        }
        else
        {
            backGroundImage.sprite = _defaultSprite;    
        }
    }
}
