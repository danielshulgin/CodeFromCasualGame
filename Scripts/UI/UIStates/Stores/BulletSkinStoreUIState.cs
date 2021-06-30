using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class BulletSkinStoreUIState : GameCurrencyStoreUIState
{
    [Inject] private PlayerBulletSkins _playerBulletSkins;
    
    protected override PlayerDataPart DataPart => _playerBulletSkins;
    

    protected override void EquipButtonClick()
    {
        base.EquipButtonClick();
        _playerBulletSkins.SelectBulletSkin(SelectedItem);
        UpdateTabs();
    }

    protected override List<ItemScriptableObject> GetStoreItemList()
    {
        return _playerBulletSkins
            .BulletSkinScriptableObjects
            .Where(bulletSkinScriptableObject => bulletSkinScriptableObject.bullet == _playerBulletSkins.CurrentBullet)
            .Select(bulletSkinScriptableObject => bulletSkinScriptableObject as ItemScriptableObject)
            .ToList();
    }
    
    protected override void UpdateTabs()
    {
        base.UpdateTabs();
        (_tabs.Find(tabUI => 
            tabUI.ItemScriptableObject.id == _playerBulletSkins.CurrentBulletSkin.id) as GameCurrencyStoreTabUI)?.EnableCheckMark();
    }

    public override void ExitButtonClick()
    {
        storesUIDependencies.storeSelectorUIState.OpenStore(StoreType.Bullet);
    }
}
