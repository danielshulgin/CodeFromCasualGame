using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CannonStoreUIState : GameCurrencyStoreUIState
{
    [Inject] private PlayerCannons _playerCannons;

    private List<TabsSeparator> _gradeSeparators;
    
    protected override PlayerDataPart DataPart => _playerCannons;

    
    public override void Enter()
    {
        base.Enter();
        _tabs[SelectedCannonIndex()].SelectWithoutSound();
    }

    protected override void InitializeTabs()
    {
        _tabs = new List<ITabUI>();
        _gradeSeparators = new List<TabsSeparator>();
        foreach (Grade grade in Enum.GetValues(typeof(Grade)))
        {
            var gradeCannons = _playerCannons.CannonScriptableObjects.Where(cannon => cannon.grade == grade);
            
            if (gradeCannons.ToList().Count == 0)
            {
                continue;
            }
            AddGradeSeparator(grade);
            foreach (var cannon in gradeCannons)
            {
                AddTab(cannon, DataPart.GetItemState(cannon));       
            }
        }
    }

    private void AddGradeSeparator(Grade grade)
    {
        var tabSeparatorGameObject = Instantiate(storesUIDependencies.tabsSeparatorPrefab, storesUIDependencies.tabParent.transform, false);
        var tabsSeparator = tabSeparatorGameObject.GetComponent<TabsSeparator>();
        tabsSeparator.Initialize(Localization.Instance.GetTranslation("item_grade") + " " + grade.ToString().ToUpper());
        _gradeSeparators.Add(tabsSeparator);
    }
    
    protected override void EquipButtonClick()
    {
        base.EquipButtonClick();
        _playerCannons.SelectCannon(SelectedItem as CannonScriptableObject);
        UpdateTabs();
    }

    protected override void UpdateTabs()
    {
        base.UpdateTabs();
        (_tabs.Find(tabUI => 
            tabUI.ItemScriptableObject.id == _playerCannons.CurrentCannon.id) as GameCurrencyStoreTabUI)?.EnableCheckMark();
    }

    protected override List<ItemScriptableObject> GetStoreItemList()
    {
        return _playerCannons
            .CannonScriptableObjects
            .Select(cannonScriptableObject => cannonScriptableObject as ItemScriptableObject)
            .ToList();
    }
    
    protected override void SelectTab(ITabUI tabUi)
    {
        base.SelectTab(tabUi);
        storesUIDependencies
            .equipButton
            .gameObject
            .SetActive(tabUi.ItemScriptableObject != _playerCannons.CurrentCannon 
                       && _playerCannons.GetItemState(SelectedItem) == ItemState.Bought);
    }
    
    private int SelectedCannonIndex()
    {
        var itemsAsCannons = _itemList.Select(item => (CannonScriptableObject) item).ToList();
        return itemsAsCannons.FindIndex(cannonSO => cannonSO == _playerCannons.CurrentCannon);
    }
}