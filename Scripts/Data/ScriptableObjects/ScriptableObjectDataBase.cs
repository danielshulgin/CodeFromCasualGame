using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


public class ScriptableObjectDataBase : MonoBehaviour
{
    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;
    
    [SerializeField] private int lastId = 0;

    private Dictionary<int, ItemScriptableObject> _scriptableObjectsById;

    private Dictionary<int, BulletScriptableObject> _bulletScriptableObjectsById;
    
    private Dictionary<int, BulletSkinScriptableObject> _bulletSkinScriptableObjectsById;
    
    private Dictionary<int, VasylSkinScriptableObject> _vasylScriptableObjectsById;

    private Dictionary<int, LevelScriptableObject> _levelScriptableObjectsById;

    private Dictionary<int, CannonScriptableObject> _cannonScriptableObjectById;
    
    private Dictionary<int, PurchaseScriptableObject> _purchaseScriptableObjectsById;

    private Dictionary<int, StackTypeScriptableObject> _stackTypeScriptableObjectsById;

    private Dictionary<int, CannonballScriptableObject> _cannonballScriptableObjectById;


    public int LastId => lastId;

    public ReadOnlyDictionary<int, ItemScriptableObject> ScriptableObjectsById 
        => new ReadOnlyDictionary<int, ItemScriptableObject>(_scriptableObjectsById);
    
    public ReadOnlyDictionary<int, BulletScriptableObject> BulletScriptableObjectsById 
        => new ReadOnlyDictionary<int, BulletScriptableObject>(_bulletScriptableObjectsById);
    
    public ReadOnlyDictionary<int, BulletSkinScriptableObject> BulletSkinScriptableObjectsById 
        => new ReadOnlyDictionary<int, BulletSkinScriptableObject>(_bulletSkinScriptableObjectsById);
    
    public ReadOnlyDictionary<int, VasylSkinScriptableObject> VasylScriptableObjectsById 
        => new ReadOnlyDictionary<int, VasylSkinScriptableObject>(_vasylScriptableObjectsById);

    public ReadOnlyDictionary<int, LevelScriptableObject> LevelScriptableObjectsById 
        => new ReadOnlyDictionary<int, LevelScriptableObject>(_levelScriptableObjectsById);
    
    public ReadOnlyDictionary<int, CannonScriptableObject> CannonScriptableObjectById 
        => new ReadOnlyDictionary<int, CannonScriptableObject>(_cannonScriptableObjectById);
    
    public ReadOnlyDictionary<int, PurchaseScriptableObject> PurchaseScriptableObjectsById 
        => new ReadOnlyDictionary<int, PurchaseScriptableObject>(_purchaseScriptableObjectsById);
    
    public ReadOnlyDictionary<int, StackTypeScriptableObject> StackTypeScriptableObjectsById 
        => new ReadOnlyDictionary<int, StackTypeScriptableObject>(_stackTypeScriptableObjectsById);
    
    public ReadOnlyDictionary<int, CannonballScriptableObject> CannonballScriptableObjectById 
        => new ReadOnlyDictionary<int, CannonballScriptableObject>(_cannonballScriptableObjectById);
    

    private void Awake()
    {
        CreateDictionaries();
        InitializeDictionaries();
    }

    private void CreateDictionaries()
    {
        _scriptableObjectsById = new Dictionary<int, ItemScriptableObject>();
        _bulletScriptableObjectsById = new Dictionary<int, BulletScriptableObject>();
        _vasylScriptableObjectsById = new Dictionary<int, VasylSkinScriptableObject>();
        _bulletSkinScriptableObjectsById = new Dictionary<int, BulletSkinScriptableObject>();
        _levelScriptableObjectsById = new Dictionary<int, LevelScriptableObject>();
        _cannonScriptableObjectById = new Dictionary<int, CannonScriptableObject>();
        _purchaseScriptableObjectsById = new Dictionary<int, PurchaseScriptableObject>();
        _stackTypeScriptableObjectsById = new Dictionary<int, StackTypeScriptableObject>();
        _cannonballScriptableObjectById = new Dictionary<int, CannonballScriptableObject>();
    }

    private void InitializeDictionaries()
    {
        foreach (var scriptableObjectItemState in defaultPlayerSettings.scriptableObjectsInventory)
        {
            AddScriptableObjectToDictionaries(scriptableObjectItemState.itemScriptableObject);
        }
        foreach (var stackItem in defaultPlayerSettings.stackItems)
        {
            AddStackTypeScriptableObjectToDictionaries(stackItem.stackTypeScriptableObject);
        }
    }

    public void AddScriptableObject(ItemScriptableObject itemScriptableObject, ItemState itemState = ItemState.Available)
    {
        defaultPlayerSettings.scriptableObjectsInventory.Add(new ScriptableObjectInInventory()
        {
            itemScriptableObject = itemScriptableObject,
            itemState = itemState
        });
        
        ++lastId;
        itemScriptableObject.id = lastId;
    }
    
    public void AddStackTypeScriptableObject(StackTypeScriptableObject stackTypeScriptableObject)
    {
        defaultPlayerSettings.stackItems.Add(new StackItem()
        {
            stackTypeScriptableObject = stackTypeScriptableObject,
            number = 0
        });
        
        ++lastId;
        stackTypeScriptableObject.id = lastId;
    }

    private void AddScriptableObjectToDictionaries(ItemScriptableObject itemScriptableObject)
    {
        _scriptableObjectsById[itemScriptableObject.id] = itemScriptableObject;
        switch (itemScriptableObject)
        {
            case BulletScriptableObject bulletScriptableObject:
                _bulletScriptableObjectsById[bulletScriptableObject.id] = bulletScriptableObject;
                break;
            case VasylSkinScriptableObject vasylSkinScriptableObject:
                _vasylScriptableObjectsById[vasylSkinScriptableObject.id] = vasylSkinScriptableObject;
                break;
            case BulletSkinScriptableObject bulletSkinScriptableObject:
                _bulletSkinScriptableObjectsById[bulletSkinScriptableObject.id] = bulletSkinScriptableObject;
                break;
            case LevelScriptableObject levelScriptableObject:
                _levelScriptableObjectsById[levelScriptableObject.id] = levelScriptableObject;
                break;
            case CannonScriptableObject cannonScriptableObject:
                _cannonScriptableObjectById[cannonScriptableObject.id] = cannonScriptableObject;
                break;
            case PurchaseScriptableObject purchaseScriptableObject:
                _purchaseScriptableObjectsById[purchaseScriptableObject.id] = purchaseScriptableObject;
                break;
        }
    }
    
    private void AddStackTypeScriptableObjectToDictionaries(StackTypeScriptableObject stackTypeScriptableObject)
    {
        _stackTypeScriptableObjectsById[stackTypeScriptableObject.id] = stackTypeScriptableObject;
        switch (stackTypeScriptableObject)
        {
            case CannonballScriptableObject cannonballScriptableObject:
                _cannonballScriptableObjectById[cannonballScriptableObject.id] = cannonballScriptableObject;
                break;
        }
    }

    public void RemoveScriptableObject(ItemScriptableObject itemScriptableObject)
    {
        defaultPlayerSettings.scriptableObjectsInventory.Remove(defaultPlayerSettings.scriptableObjectsInventory.ToList().Find(scriptableObjectInventory => 
            scriptableObjectInventory.itemScriptableObject == itemScriptableObject));
    }
    
    public void RemoveStackTypeScriptableObject(StackTypeScriptableObject stackTypeScriptableObject)
    {
        defaultPlayerSettings.stackItems.Remove(defaultPlayerSettings.stackItems.ToList().Find(stackItem => 
            stackItem.stackTypeScriptableObject == stackTypeScriptableObject));
    }
}
