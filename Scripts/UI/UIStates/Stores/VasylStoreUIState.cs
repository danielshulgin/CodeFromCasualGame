using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class VasylStoreUIState : GameCurrencyStoreUIState
{
    [Inject] private PlayerVasylSkins _playerVasylSkins;
    
    protected override PlayerDataPart DataPart => _playerVasylSkins;
    
    
    protected override void EquipButtonClick()
    {
        base.EquipButtonClick();
        _playerVasylSkins.SelectElkSkin(SelectedItem);
        UpdateTabs();
    }

    protected override List<ItemScriptableObject> GetStoreItemList()
    {
        return _playerVasylSkins
           .VasylSkins
           .Select(elkSkinScriptableObject => elkSkinScriptableObject as ItemScriptableObject)
           .ToList();
    }
}
