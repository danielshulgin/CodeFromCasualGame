using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameState))]
public class PlayerData : MonoBehaviour
{
    public static event Action<Currency> OnPlayerCurrencyChange;
    
    public event Action OnPlayerDataUpdate;

    [SerializeField] private ScriptableObjectDataBase scriptableObjectDataBase;

    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;
    
    private PlayerDAO _playerData;
    
    public ReadOnlyDictionary<int, ItemState> ItemStateById => new ReadOnlyDictionary<int, ItemState>(_playerData.itemStateById);
    
    public ReadOnlyDictionary<int, int> NumberInStackById => new ReadOnlyDictionary<int, int>(_playerData.numberInStackById);
    
    public ReadOnlyCollection<int> PassedTutorialParts => new ReadOnlyCollection<int>(_playerData.passedTutorialParts);
    
    public ScriptableObjectDataBase ScriptableObjectDataBase => scriptableObjectDataBase;


    public int CurrentBulletSkinId
    {
        get => _playerData.currentBulletSkinId;
        set => _playerData.currentBulletSkinId = value;
    }
    
    public int CurrentLevelId
    {
        get => _playerData.currentLevelId;
        set => _playerData.currentLevelId = value;
    }
    
    public int CurrentVasylSkinId
    {
        get => _playerData.currentVasylSkinId;
        set => _playerData.currentVasylSkinId = value;
    }
    
    public int CurrentCannonballId
    {
        get => _playerData.currentCannonballId;
        set => _playerData.currentCannonballId = value;
    }
    
    public int CurrentCannonId
    {
        get => _playerData.currentCannonId;
        set => _playerData.currentCannonId = value;
    }

    public int Coins => _playerData.coins;
    
    public int Crystals => _playerData.crystals;

    public int LastSelectedBulletId => 
        scriptableObjectDataBase.BulletSkinScriptableObjectsById[_playerData.currentBulletSkinId].bullet.id;


    private void Awake()
    {
        LoadPlayerData();
    }

    private void Start()
    {
        UpdatePlayerData(_playerData);
        FlyResults.OnFinalResult += HandleFinalFlyResults;
    }
    
    private void OnDestroy()
    {
        FlyResults.OnFinalResult -= HandleFinalFlyResults;
    }

    public void LoadDefaults()
    {
        LocalFileStorage.Instance.Save(null);
        _playerData = LocalFileStorage.Instance.Load();
        SceneLoader.Instance.LoadStartScene();
    }

    private void UpdatePlayerData(PlayerDAO playerData)
    {
        CurrentLevelId = playerData.currentLevelId;
        CurrentBulletSkinId = _playerData.currentBulletSkinId;
        
        OnPlayerCurrencyChange?.Invoke(new Currency(playerData.coins, playerData.crystals, CurrencyType.CoinsCrystals));

        OnPlayerDataUpdate?.Invoke();
    }

    public void LoadPlayerData()
    {
        _playerData = LocalFileStorage.Instance.Load();
    }

    public void Save()
    {
        _playerData.deviceId = SystemInfo.deviceUniqueIdentifier;
        LocalFileStorage.Instance.Save(_playerData);
        RemoteFileStorage.Instance.Save(_playerData);
    }

    private void HandleFinalFlyResults(Currency result)
    {
        _playerData.coins += result.coins;
        _playerData.crystals += result.crystals;
        OnPlayerCurrencyChange?.Invoke(new Currency(Coins, Crystals, CurrencyType.CoinsCrystals));
        Save();
    }

    public void ProcessPurchase(PurchaseScriptableObject purchaseScriptableObject)
    {
        _playerData.coins += purchaseScriptableObject.currency.coins;
        _playerData.crystals += purchaseScriptableObject.currency.crystals;

        OnPlayerCurrencyChange?.Invoke(new Currency(Coins, Crystals, CurrencyType.CoinsCrystals));
        
        foreach (var itemScriptableObject in purchaseScriptableObject.makesBought)
        {
            _playerData.itemStateById[itemScriptableObject.id] = ItemState.Bought;
        }
        
        Save();
    }

    public void HandleTutorialPartPath(int partNumber)
    {
        if (_playerData.passedTutorialParts == null)
        {
            _playerData.passedTutorialParts = new List<int>();
        }
        
        if (!_playerData.passedTutorialParts.Contains(partNumber))
        {
            _playerData.passedTutorialParts.Add(partNumber);
        }
        
        Save();
    }
    
    public void AddCurrency(Currency currency)
    {
        AddCurrencyWithoutNotify(currency);
        OnPlayerCurrencyChange?.Invoke(new Currency(Coins, Crystals, CurrencyType.CoinsCrystals));
    }
    
    public void AddCurrencyWithoutNotify(Currency currency)
    {
        _playerData.coins += currency.coins;
        _playerData.crystals += currency.crystals;

        Save();
    }
    
    public bool CanBuyItem(int id)
    {
        if (!SingleItemIdPresentInPlayerData(id)) return false;
        if (ItemStateById[id] == ItemState.Available 
            && scriptableObjectDataBase.ScriptableObjectsById[id].price.CanBuy(Coins, Crystals))
        {
            return true;
        }
        return false;
    }
    
    public bool CanBuyStackItems(int id, int number = 1)
    {
        if (!StackTypeIdPresentInPlayerData(id)) return false;
        if ((scriptableObjectDataBase.StackTypeScriptableObjectsById[id].price * number).CanBuy(Coins, Crystals))
        {
            return true;
        }
        return false;
    }
    
    public bool BuyItem(int id)
    {
        if (CanBuyItem(id))
        {
            var itemScriptableObject = scriptableObjectDataBase.ScriptableObjectsById[id];
            
            _playerData.coins -= itemScriptableObject.price.coins;
            _playerData.crystals -= itemScriptableObject.price.crystals;
            OnPlayerCurrencyChange?.Invoke(new Currency(Coins, Crystals, CurrencyType.CoinsCrystals));
            
            _playerData.itemStateById[id] = ItemState.Bought;
            
            foreach (var scriptableObject in itemScriptableObject.makesAvailable)
            {
                if (_playerData.itemStateById[scriptableObject.id] == ItemState.Locked)
                {
                    _playerData.itemStateById[scriptableObject.id] = ItemState.Available;
                }
            }
            foreach (var scriptableObject in itemScriptableObject.makesBought)
            {
                _playerData.itemStateById[scriptableObject.id] = ItemState.Bought;
            }
            
            Save();
            return true;
        }
        return false;
    }
    
    public bool BuyStackItems(int id, int number)
    {
        if (CanBuyStackItems(id, number))
        {
            var stackTypeScriptableObject = scriptableObjectDataBase.StackTypeScriptableObjectsById[id];
            
            _playerData.coins -= stackTypeScriptableObject.price.coins * number;
            _playerData.crystals -= stackTypeScriptableObject.price.crystals * number;
            OnPlayerCurrencyChange?.Invoke(new Currency(Coins, Crystals, CurrencyType.CoinsCrystals));
            
            _playerData.numberInStackById[id] += number;

            Save();
            return true;
        }
        return false;
    }

    public bool CanUseStackItems(int id, int number)
    {
        return _playerData.numberInStackById[id] >= number;
    }
    
    public bool UseStackItems(int id, int number)
    {
        if (CanUseStackItems(id, number))
        {
            _playerData.numberInStackById[id] -= number;

            Save();
            return true;
        }
        return false;
    }

    public bool SingleItemIdPresentInPlayerData(int id)
    {
        if (ItemStateById.ContainsKey(id))
        {
            return true;
        }
        Debug.Log($"can't find single item id: {id}");
        return false;
    } 
    
    public bool StackTypeIdPresentInPlayerData(int id)
    {
        if (NumberInStackById.ContainsKey(id))
        {
            return true;
        }
        Debug.Log($"can't find stack type id: {id}");
        return false;
    }
}