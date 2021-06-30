using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;


public class PlayerLevels : PlayerDataPart
{
    public event Action OnChangeLevel;
    
    public event Action OnChangeTraps;
    
    public static event Action<LevelScriptableObject> OnBuyLevel;

    public static event Action<LevelPartScriptableObject> OnBuyLevelPart; 
    
    public Action<LevelScriptableObject> OnStartSwitchLevel;
    
    public Action<LevelScriptableObject> OnCancelSwitchLevel;

    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;
    
    [Inject] private ScriptableObjectDataBase _scriptableObjectDataBase;

    public ReadOnlyCollection<LevelScriptableObject> LevelScriptableObjects => 
        new ReadOnlyCollection<LevelScriptableObject>(_scriptableObjectDataBase.LevelScriptableObjectsById.Values.ToList());

    
    public LevelScriptableObject CurrentLevel
    {
        get
        {
            if (_playerData.SingleItemIdPresentInPlayerData(_playerData.CurrentLevelId))
            {
                return _scriptableObjectDataBase.LevelScriptableObjectsById[_playerData.CurrentLevelId];
            }
            
            return defaultPlayerSettings.currentLevel;
        }
    }

    public static PlayerLevels Instance { get; private set; }//TODO remove singletone

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Multiple PlayerLevels instances");
        }

        _playerData.OnPlayerDataUpdate += HandlePlayerDataUpdate;
        LevelSwitcher.OnEndSwitch += HandleSwitchLevel;
        LevelSwitcher.OnStartSwitch += HandleStartSwitchLevel;
        LevelSwitcher.OnCancelSwitch += HandleCancelSwitchLevel;
    }

    private void OnDestroy()
    {
        Instance = null;
        LevelSwitcher.OnEndSwitch -= HandleSwitchLevel;
        LevelSwitcher.OnStartSwitch -= HandleStartSwitchLevel;
        LevelSwitcher.OnCancelSwitch -= HandleCancelSwitchLevel;
    }

    private void HandleSwitchLevel(LevelScriptableObject levelScriptableObject)
    {
        if (!_playerData.SingleItemIdPresentInPlayerData(levelScriptableObject.id))
        {
            return;
        }
        _playerData.CurrentLevelId = levelScriptableObject.id;
        OnChangeLevel?.Invoke();
        _playerData.Save();
    }

    public bool BuyNextPartOfLevel()
    {
        if (_playerData.BuyItem(GetNextPartOfLevelId()))
        {
            OnChangeLevel?.Invoke();
            _playerData.Save();
            OnBuyLevelPart?.Invoke(GetCurrentLevelPart(CurrentLevel));
            return true;
        }
        return false;
    }
    
    public bool BuyLevel()
    {
        if (_playerData.BuyItem(CurrentLevel.id))
        {
            OnBuyLevel?.Invoke(CurrentLevel);
            return true;
        }
        return false;
    }

    private void HandleStartSwitchLevel(LevelScriptableObject levelScriptableObject)
    {
        OnStartSwitchLevel?.Invoke(levelScriptableObject);
    }
    
    private void HandleCancelSwitchLevel(LevelScriptableObject levelScriptableObject)
    {
        OnCancelSwitchLevel?.Invoke(levelScriptableObject);
    }
    
    private void HandlePlayerDataUpdate()
    {
        SelectLevel(_playerData.CurrentLevelId);
        OnChangeLevel?.Invoke();
    }

    public void UpdateLevel()
    {
        OnChangeLevel?.Invoke();
    }
    
    public bool CanBuyNextPartOfLevel()
    {
        var nextLevelPartItemState = GetItemState(GetNextPartOfLevel());
        return LevelBought(CurrentLevel) && 
               nextLevelPartItemState != ItemState.Bought;
    }

    public ItemScriptableObject GetNextPartOfLevel()
    {
        return _scriptableObjectDataBase.ScriptableObjectsById[GetNextPartOfLevelId()];
    }
    
    public bool SelectLevel(int id)
    {
        if (!_playerData.SingleItemIdPresentInPlayerData(id))
        {
            id = defaultPlayerSettings.currentLevel.id;
        }
        if (_playerData.ItemStateById[id] == ItemState.Bought)
        {
            _playerData.CurrentLevelId = id;
            OnChangeLevel?.Invoke();
            _playerData.Save();
            return true;
        }
        return false;
    }
    
    public bool BuyTrap(int id)
    {
        if (_playerData.BuyItem(id))
        {
            OnChangeTraps?.Invoke();
            return true;
        }
        return false;
    }

    public LevelPartScriptableObject GetCurrentLevelPart(LevelScriptableObject levelScriptableObject)
    {
        var levelParts = levelScriptableObject.levelPartScriptableObjects;
        var currentLevelPart = levelScriptableObject.levelPartScriptableObjects[0];
        foreach (var levelPartScriptableObject in levelParts)
        {
            if (_playerData.ItemStateById[levelPartScriptableObject.id] == ItemState.Bought)
            {
                currentLevelPart = levelPartScriptableObject;
            }
        }
        return currentLevelPart;
    }

    public int GetNextPartOfLevelId()
    {
        var levelParts = CurrentLevel.levelPrefab.GetComponent<LevelComponent>().levelParts;
        foreach (var levelPart in levelParts)
        {
            if (_playerData.ItemStateById[levelPart.levelPartScriptableObject.id] == ItemState.Available)
            {
                return levelPart.levelPartScriptableObject.id;
            }
        }
        return levelParts[0].levelPartScriptableObject.id;
    }

    public bool LevelBought(LevelScriptableObject levelScriptableObject)
    {
        return _playerData.ItemStateById[levelScriptableObject.id] == ItemState.Bought;
    }
    
    public bool LevelBought()
    {
        return _playerData.ItemStateById[CurrentLevel.id] == ItemState.Bought;
    }
    
    public ItemState GetTrapState(ObstacleScriptableObject trap)
    {
        return _playerData.ItemStateById[trap.id];
    }
    
    public ItemState GetLevelPartState(LevelPartScriptableObject levelPart)
    {
        return _playerData.ItemStateById[levelPart.id];
    }
}
