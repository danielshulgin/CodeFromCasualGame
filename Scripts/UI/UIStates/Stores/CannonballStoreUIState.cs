using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CannonballStoreUIState : GameCurrencyStoreUIState
{
    [SerializeField] private GameObject tabPrefab;
    
    [SerializeField] private TextMeshProUGUI numberText;
    
    [Inject] private PlayerCannonballs _playerCannonballs;

    protected override PlayerDataPart DataPart => _playerCannonballs;

    private List<TabsSeparator> _gradeSeparators;
    
    
    public override void Enter()
    {
        base.Enter();
        _tabs[SelectedCannonballIndex()].SelectWithoutSound();
        storesUIDependencies.characteristicsPanel.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        storesUIDependencies.characteristicsPanel.SetActive(false);
        numberText.text = "";
    }

    protected override void InitializeTabs()
    {
        _tabs = new List<ITabUI>();
        _gradeSeparators = new List<TabsSeparator>();
        
        foreach (Grade grade in Enum.GetValues(typeof(Grade)))
        {
            var gradeCannonballs = 
                _playerCannonballs.CannonballScriptableObjects.Where(cannonball => cannonball.grade == grade);
            
            if (gradeCannonballs.ToList().Count == 0)
            {
                continue;
            }
            
            AddGradeSeparator(grade);
            foreach (var cannonball in gradeCannonballs)
            {
                AddTab(cannonball, DataPart.GetItemState(cannonball));       
            }
        }
    }
    
    protected override void AddTab(ItemScriptableObject itemScriptableObject, ItemState itemState)
    {
        var tab = Instantiate(tabPrefab, storesUIDependencies.tabParent.transform, false);
        var tabUi = tab.GetComponent<GameCurrencyStoreTabUI>();
        _tabs.Add(tabUi);
        tabUi.UpdateTab(itemScriptableObject, itemState);
        tabUi.OnSelect += SelectTab;
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
        var cannonballScriptableObject = SelectedItem as CannonballScriptableObject;
        _playerCannonballs.SelectCannonball(cannonballScriptableObject);
        storesUIDependencies.buyButton.gameObject.SetActive(true);
        UpdateTabs();
    }

    protected override void UpdateTabs()
    {
        base.UpdateTabs();
        (_tabs.Find(tabUI => 
            tabUI.ItemScriptableObject.id == _playerCannonballs.CurrentCannonball.id) as GameCurrencyStoreTabUI)?.EnableCheckMark();
    }

    protected override List<ItemScriptableObject> GetStoreItemList()
    {
        return _playerCannonballs
            .CannonballScriptableObjects
            .Select(bulletScriptableObject => bulletScriptableObject as ItemScriptableObject)
            .ToList();
    }
    
    protected override void SelectTab(ITabUI tabUi)
    {
        if (_previousSelectedTab != null)
        {
            _previousSelectedTab.DeSelect();
        }
        _previousSelectedTab = tabUi;
        SelectedItem = tabUi.ItemScriptableObject;
        storesUIDependencies.buyButton.gameObject.SetActive(true);
        storesUIDependencies.selectedImage.sprite = tabUi.ItemScriptableObject.storeSprite;
        LockItemIfComingSoon();
        
        storesUIDependencies
            .equipButton
            .gameObject
            .SetActive(tabUi.ItemScriptableObject != _playerCannonballs.CurrentCannonball);

        var cannonballScriptableObject = SelectedItem as CannonballScriptableObject;
        numberText.text = _playerCannonballs.GetNumberInStack(cannonballScriptableObject).ToString();
    }

    protected override void BuyButtonClick()
    {
        _uiStateMachine.BuyStackItemsPanel.Initialize(SelectedItem as StackTypeScriptableObject, HandleBuy);
    }

    private void HandleBuy(int number)
    {
        var cannonballScriptableObject = SelectedItem as CannonballScriptableObject;
        _playerCannonballs.BuyBombsStack(cannonballScriptableObject, number);
        numberText.text = _playerCannonballs.GetNumberInStack(cannonballScriptableObject).ToString();
    }

    private int SelectedCannonballIndex()
    {
        var itemsAsCannonballs = _itemList.Select(item => (CannonballScriptableObject) item).ToList();
        return itemsAsCannonballs.FindIndex(cannonballSO => cannonballSO == _playerCannonballs.CurrentCannonball);
    }
    
    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);
        
        SetInteractableCharacteristicsPanel(interactable);
    }

    public void SetInteractableCharacteristicsPanel(bool interactable)
    {
        storesUIDependencies.characteristicsPanel.GetComponent<Image>().color
            = interactable ? Constants.NormalColor : Constants.DisabledColor;
    }
}
