using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BulletStoreUIState : GameCurrencyStoreUIState
{
    [SerializeField] private GameObject heart;

    [Inject] private PlayerBulletSkins _playerBulletSkins;

    protected override PlayerDataPart DataPart => _playerBulletSkins;

    
    public override void Enter()
    {
        base.Enter();
        storesUIDependencies.editButton.onClick.AddListener(EditButtonClick);
        _tabs[SelectedBulletIndex()].SelectWithoutSound();
        storesUIDependencies.characteristicsPanel.SetActive(true);
        heart.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        storesUIDependencies.editButton.onClick.RemoveListener(EditButtonClick);
        storesUIDependencies.characteristicsPanel.SetActive(false);
        heart.SetActive(false);
        storesUIDependencies.characteristicsText.text = "";
    }

    private void EditButtonClick()
    {
        storesUIDependencies.storeSelectorUIState.OpenStore(StoreType.BulletSkin);
    }

    protected override void EquipButtonClick()
    {
        base.EquipButtonClick();
        _playerBulletSkins.SelectBulletSkin((SelectedItem as BulletScriptableObject).firstSkin);
        storesUIDependencies.editButton.gameObject.SetActive(true);
        UpdateTabs();
    }

    protected override void UpdateTabs()
    {
        base.UpdateTabs();
        (_tabs.Find(tabUI => 
            tabUI.ItemScriptableObject.id == _playerBulletSkins.CurrentBullet.id) as GameCurrencyStoreTabUI)?.EnableCheckMark();
    }

    protected override List<ItemScriptableObject> GetStoreItemList()
    {
        return _playerBulletSkins
            .BulletScriptableObjects
            .Select(bulletScriptableObject => bulletScriptableObject as ItemScriptableObject)
            .ToList();
    }
    
    protected override void SelectTab(ITabUI tabUi)
    {
        storesUIDependencies
            .editButton
            .gameObject
            .SetActive(tabUi.ItemScriptableObject == _playerBulletSkins.CurrentBullet);
        base.SelectTab(tabUi);
        storesUIDependencies.characteristicsText.text = (tabUi.ItemScriptableObject as BulletScriptableObject)?.hp.ToString();
    }
    
    private int SelectedBulletIndex()
    {
        var itemsAsBulletSkins = _itemList.Select(item => (BulletScriptableObject) item).ToList();
        return itemsAsBulletSkins.FindIndex(bulletSO => bulletSO == _playerBulletSkins.CurrentBullet);
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
