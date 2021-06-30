using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Zenject;


public class PlayerCannons : PlayerDataPart
{
    public static event Action<CannonScriptableObject> OnChangeCannon;

    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;

    public CannonScriptableObject CurrentCannon =>
        _scriptableObjectDataBase.CannonScriptableObjectById[_playerData.CurrentCannonId];

    public ReadOnlyCollection<CannonScriptableObject> CannonScriptableObjects => 
        new ReadOnlyCollection<CannonScriptableObject>(_scriptableObjectDataBase.CannonScriptableObjectById.Values.ToList());

    [Inject] private ScriptableObjectDataBase _scriptableObjectDataBase;
    
    
    private void Awake()
    {
        _playerData.OnPlayerDataUpdate += HandlePlayerDataUpdate;
    }

    private void HandlePlayerDataUpdate()
    {
        SelectCannon(_playerData.CurrentCannonId);
    }

    public bool SelectCannon(ItemScriptableObject itemScriptableObject)
    {
        return SelectCannon(itemScriptableObject.id);
    }

    private bool SelectCannon(int id)
    {
        if (!_playerData.SingleItemIdPresentInPlayerData(id))
        {
            id = defaultPlayerSettings.currentCannon.id;
        }
        if (_playerData.ItemStateById[id] == ItemState.Bought)
        {
            _playerData.CurrentCannonId = id;
            
            var cannonScriptableObject = _scriptableObjectDataBase.CannonScriptableObjectById[id];
            
            OnChangeCannon?.Invoke(cannonScriptableObject);

            _playerData.Save();
            return true;
        }
        return false;
    }

    public override ItemState GetItemState(ItemScriptableObject itemScriptableObject)
    {
        return base.GetItemState(itemScriptableObject);
    }
}
