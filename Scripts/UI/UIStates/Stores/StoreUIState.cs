using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class StoreUIState : UIState
{
    [SerializeField] protected StoresUIDependencies storesUIDependencies;

    protected ITabUI _previousSelectedTab;

    protected List<ItemScriptableObject> _itemList; 
    
    protected List<ITabUI> _tabs;

    public ItemScriptableObject SelectedItem { get; protected set; }
    
    
    public override void Enter()
    {
        SetActiveCanvasGroup(_canvasGroup, true);
        SetActiveCanvasGroup(storesUIDependencies.coinsCrystalsCanvasGroup, true);
        
        storesUIDependencies.exitButton.onClick.AddListener(ExitButtonClick);
        storesUIDependencies.buyButton.onClick.AddListener(BuyButtonClick);

        _itemList = GetStoreItemList();
        foreach (Transform child in storesUIDependencies.tabParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        InitializeTabs();
        if (_tabs.Count > 0)
        {
            _tabs[0].Select();
        }
    }

    public override void Exit()
    {
        storesUIDependencies.exitButton.onClick.RemoveListener(ExitButtonClick);
        storesUIDependencies.buyButton.onClick.RemoveListener(BuyButtonClick);

        SetActiveCanvasGroup(_canvasGroup, false);
        SetActiveCanvasGroup(storesUIDependencies.coinsCrystalsCanvasGroup, false); 
        
        storesUIDependencies.buyButton.gameObject.SetActive(false);
        _previousSelectedTab = null;
        _tabs = new List<ITabUI>();
    }

    protected abstract void SelectTab(ITabUI tabUi);

    protected abstract void InitializeTabs();

    public void SetInteractableTabs(bool interactable)
    {
        if (_tabs == null)
            return;
        
        foreach (var tab in _tabs)
        {
            tab.SetInteractable(interactable);
        }
    }

    public void SetInteractableTab(int tabIndex, bool interactable)
    {
        if (_tabs == null || tabIndex < 0 || tabIndex >= _tabs.Count)
            return;
        
        _tabs[tabIndex].SetInteractable(interactable);
    }

    public void SelectTab(int tabIndex)
    {
        if (_tabs == null || tabIndex < 0 || tabIndex >= _tabs.Count)
                    return;
        SelectTab(_tabs[tabIndex]);
    }

    public override void ExitButtonClick()
    {
        _uiStateMachine.ShowMainMenu();
    }

    protected abstract void BuyButtonClick();

    protected abstract List<ItemScriptableObject> GetStoreItemList();
    
    public virtual void SetInteractable(bool interactable)
    {
        storesUIDependencies.buyButton.interactable = interactable;
        storesUIDependencies.exitButton.interactable = interactable;
        
        var color = interactable ? Constants.NormalColor : Constants.DisabledColor;
        storesUIDependencies.selectedImage.color = color;
        storesUIDependencies.soilImage.color = color;
        storesUIDependencies.raysImage.color = color;
        storesUIDependencies.itemsBackGroundImage.color = color;
        storesUIDependencies.comingSoonPanel.GetComponent<Image>().color = color;

        storesUIDependencies.coinsCrystalsButton.interactable = interactable;

        storesUIDependencies.editButton.interactable = interactable;
        storesUIDependencies.equipButton.interactable = interactable;

        SetInteractableTabs(interactable);
    }
}
