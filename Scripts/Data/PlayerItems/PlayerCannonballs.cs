using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerCannonballs : PlayerDataPart
{
    public static event Action<CannonballScriptableObject> OnChangeCannonball;
    
    public Action<CannonballScriptableObject, int> OnUpdateNumberInStack;//TODO 
    
    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;

    [Inject] private ScriptableObjectDataBase _scriptableObjectDataBase;

    public CannonballScriptableObject CurrentCannonball =>
        _scriptableObjectDataBase.CannonballScriptableObjectById[_playerData.CurrentCannonballId];

    public ReadOnlyCollection<CannonballScriptableObject> CannonballScriptableObjects => 
        new ReadOnlyCollection<CannonballScriptableObject>(_scriptableObjectDataBase.CannonballScriptableObjectById.Values.ToList());


    private void Awake()
    {
        _playerData.OnPlayerDataUpdate += HandlePlayerDataUpdate;
    }

    public int GetNumberInStack(CannonballScriptableObject stackTypeScriptableObject)
    {
        return _playerData.NumberInStackById[stackTypeScriptableObject.id];
    }
    
    public bool CanBuyBombsStack(CannonballScriptableObject stackTypeScriptableObject, int number = 1)
    {
        return _playerData.CanBuyStackItems(stackTypeScriptableObject.id, number);
    }
    
    public bool BuyBombsStack(CannonballScriptableObject stackTypeScriptableObject, int number = 1)
    {
        var result = _playerData.BuyStackItems(stackTypeScriptableObject.id, number);
        if (result)
        {
            OnUpdateNumberInStack?.Invoke(stackTypeScriptableObject, number);
        }
        return result;
    }
    
    public bool CanUseBombs(CannonballScriptableObject stackTypeScriptableObject, int number = 1)
    {
        return _playerData.CanUseStackItems(stackTypeScriptableObject.id, number);
    }
    
    public bool UseStackItems(CannonballScriptableObject stackTypeScriptableObject, int number = 1)
    {
        var result = _playerData.UseStackItems(stackTypeScriptableObject.id, number);
        if (result)
        {
            OnUpdateNumberInStack?.Invoke(stackTypeScriptableObject, number);
        }
        return result;
    }

    private void HandlePlayerDataUpdate()
    {
        SelectCannonball(_playerData.CurrentCannonballId);
    }

    public bool SelectCannonball(ItemScriptableObject itemScriptableObject)
    {
        return SelectCannonball(itemScriptableObject.id);
    }

    private bool SelectCannonball(int id)
    {
        if (!_playerData.StackTypeIdPresentInPlayerData(id))
        {
            id = defaultPlayerSettings.currentCannonball.id;
        }
        _playerData.CurrentCannonballId = id;
            
        var cannonballScriptableObject = _scriptableObjectDataBase.CannonballScriptableObjectById[id];
        
        OnChangeCannonball?.Invoke(cannonballScriptableObject);

        _playerData.Save();
        return true;
    }

    public override ItemState GetItemState(ItemScriptableObject itemScriptableObject)
    {
        return ItemState.Available;
    }
}
