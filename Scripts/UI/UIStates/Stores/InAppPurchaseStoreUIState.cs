using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;


public class InAppPurchaseStoreUIState : StoreUIState
{
    [Inject] private InAppPurchaseManager _inAppPurchaseManager;

    
    public override void Enter()
    {
        base.Enter();
        
        storesUIDependencies.coinsCrystalsButton.interactable = false;
        UIHelperFunctions.ChangeDisabledButtonColor(storesUIDependencies.coinsCrystalsButton, Constants.NormalColor);
        
        storesUIDependencies.buyButton.gameObject.SetActive(true);
        OnEnter?.Invoke();
    }

    public override void Exit()
    {
        base.Exit();
        
        storesUIDependencies.coinsCrystalsButton.interactable = true;
        UIHelperFunctions.ChangeDisabledButtonColor(storesUIDependencies.coinsCrystalsButton, Constants.DisabledColor);
        
        storesUIDependencies.buyButton.gameObject.SetActive(false);
    }
    
    protected override void SelectTab(ITabUI tabUi)
    {
        if (_previousSelectedTab != null)
        {
            _previousSelectedTab.DeSelect();
        }
        _previousSelectedTab = tabUi;
        SelectedItem = tabUi.ItemScriptableObject;
        
        storesUIDependencies.selectedImage.sprite = tabUi.ItemScriptableObject.storeSprite;
    }
    
    protected override void InitializeTabs()
    {
        _tabs = new List<ITabUI>();
        
        foreach (var itemScriptableObject in _itemList)
        {
            AddTab(itemScriptableObject);
        }
    }

    private void AddTab(ItemScriptableObject itemScriptableObject)
    {
        var tab = Instantiate(storesUIDependencies.IAPtabPrefab, storesUIDependencies.tabParent.transform, false);
        var tabUi = tab.GetComponent<InAppPurchaseTabUI>();
        _tabs.Add(tabUi);
        tabUi.UpdateTab(itemScriptableObject, ItemState.Available);
        tabUi.OnSelect += SelectTab;
    }

    protected override void BuyButtonClick()
    {
        _inAppPurchaseManager.BuyProductId(SelectedItem.id);
    }
    
    protected override List<ItemScriptableObject> GetStoreItemList()
    {
        return storesUIDependencies
            .scriptableObjectDataBase
            .PurchaseScriptableObjectsById
            .Values
            .Select(purchaseScriptableObjectById => purchaseScriptableObjectById as ItemScriptableObject)
            .ToList();
    }
}
