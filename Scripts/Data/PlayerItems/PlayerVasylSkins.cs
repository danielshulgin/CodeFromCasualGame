using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerVasylSkins : PlayerDataPart
{
    public Action OnChangeElk;

    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;
    
    [Inject] private ScriptableObjectDataBase _scriptableObjectDataBase;

    public VasylSkinScriptableObject LastSelectedVasylSkin => 
        _scriptableObjectDataBase.VasylScriptableObjectsById[_playerData.CurrentVasylSkinId];
    
    public ReadOnlyCollection<VasylSkinScriptableObject> VasylSkins => 
        new ReadOnlyCollection<VasylSkinScriptableObject>(_scriptableObjectDataBase.VasylScriptableObjectsById.Values.ToList());

    private void Awake()
    {
        _playerData.OnPlayerDataUpdate += HandlePlayerDataUpdate;
    }
    
    public bool SelectElkSkin(ItemScriptableObject itemScriptableObject)
    {
        return SelectElkSkin(itemScriptableObject.id);
    }

    private bool SelectElkSkin(int id)
    {
        if (!_playerData.SingleItemIdPresentInPlayerData(id))
        {
            id = defaultPlayerSettings.currentVasylSkin.id;
        }
        if (_playerData.ItemStateById[id] == ItemState.Bought)
        {
            _playerData.CurrentVasylSkinId = id;
            _playerData.Save();
            
            OnChangeElk?.Invoke();
            return true;
        }
        return false;
    }

    public bool BuyElkSkin(int id)
    {
        return _playerData.BuyItem(id);
    }

    private void HandlePlayerDataUpdate()
    {
        SelectElkSkin(_playerData.CurrentVasylSkinId);
    }
}
