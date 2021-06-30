using System.Collections.Generic;
using UnityEngine;

public abstract class GameCurrencyStoreUIState : StoreUIState
{
    protected virtual PlayerDataPart DataPart { get; }

    public override void Enter()
    {
        base.Enter();
        storesUIDependencies.equipButton.onClick.AddListener(EquipButtonClick);
        UpdateTabs();
    }

    public override void Exit()
    {
        base.Exit();
        storesUIDependencies.equipButton.onClick.RemoveListener(EquipButtonClick);
        storesUIDependencies.equipButton.gameObject.SetActive(false);
        storesUIDependencies.editButton.gameObject.SetActive(false);
        storesUIDependencies.comingSoonPanel.gameObject.SetActive(false);
    }
    
    protected override void InitializeTabs()
    {
        _tabs = new List<ITabUI>();
        
        foreach (var itemScriptableObject in _itemList)
        {
            AddTab(itemScriptableObject, DataPart.GetItemState(itemScriptableObject));
        }
    }
    
    protected override void SelectTab(ITabUI tabUi)
    {
        if (_previousSelectedTab != null)
        {
            _previousSelectedTab.DeSelect();
        }
        _previousSelectedTab = tabUi;
        
        SelectedItem = tabUi.ItemScriptableObject;
        if (DataPart.GetItemState(tabUi.ItemScriptableObject) == ItemState.Available)
        {
            storesUIDependencies.buyButton.gameObject.SetActive(true);
        }
        else
        {
            storesUIDependencies.buyButton.gameObject.SetActive(false);
        }
        storesUIDependencies.selectedImage.sprite = tabUi.ItemScriptableObject.storeSprite;
        LockItemIfComingSoon();
    }

    protected virtual void AddTab(ItemScriptableObject itemScriptableObject, ItemState itemState)
    {
        var tab = Instantiate(storesUIDependencies.tabPrefab, storesUIDependencies.tabParent.transform, false);
        var tabUi = tab.GetComponent<GameCurrencyStoreTabUI>();
        _tabs.Add(tabUi);
        tabUi.UpdateTab(itemScriptableObject, itemState);
        tabUi.OnSelect += SelectTab;
    }

    protected virtual void UpdateTabs()
    {
        for (var i = 0; i < _tabs.Count; i++)
        {
            var itemScriptableObject = _itemList[i];
            _tabs[i].UpdateTab(itemScriptableObject, DataPart.GetItemState(itemScriptableObject));
        }
    }

    protected override void BuyButtonClick()
    {
        storesUIDependencies.confirmPanel.Initialize(SelectedItem,
            ()=> ConfirmBuyButtonClick(), null);
    }

    protected virtual void ConfirmBuyButtonClick()
    {
        UpdateTabs();
        _previousSelectedTab.Select();
    }

    protected virtual void EquipButtonClick()
    {
        storesUIDependencies.equipButton.gameObject.SetActive(false);
    }

    protected void LockItemIfComingSoon()
    {
        if (!storesUIDependencies.defaultPlayerSettings.comingSoon.Contains(SelectedItem))
        {
            storesUIDependencies.comingSoonPanel.SetActive(false);
            return;
        }
        storesUIDependencies.buyButton.gameObject.SetActive(false);
        storesUIDependencies.editButton.gameObject.SetActive(false);
        storesUIDependencies.comingSoonPanel.SetActive(true);
    }
}
