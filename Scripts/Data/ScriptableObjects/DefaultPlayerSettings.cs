using System.Collections.Generic;
using System.Linq;
using Malee;
using MyBox;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlayerSettings", menuName = "MyScriptableObjects/DefaultPlayerSettings", order = 2)] 
public class DefaultPlayerSettings : ScriptableObject
{
    public int coins = 100;

    public int crystals = 0;
    
    public LevelScriptableObject currentLevel;
    
    public BulletSkinScriptableObject currentBulletSkin;
    
    public VasylSkinScriptableObject currentVasylSkin;
    
    public CannonballScriptableObject currentCannonball;
    
    public CannonScriptableObject currentCannon;
    
    [Reorderable] public InventoryReorderableArray scriptableObjectsInventory;
    
    [Reorderable] public StackItemsReorderableArray stackItems;

    public bool gameSounds = true;
    
    public bool UISounds = true;
    
    public bool musicSounds = true;
    
    public bool physicVibration = false;

    public bool blood = true;
    
    public bool sight = true;
    
    public List<int> passedTutorialParts;
    
    public List<ItemScriptableObject> comingSoon;
    
    public List<Currency> adsRewardsCurrency;
    
    public Dictionary<int, ItemState> ItemStatesById
    {
        get
        {
            var result = new Dictionary<int, ItemState>();
            foreach (var scriptableObjectItemState in scriptableObjectsInventory)
            {
                result[scriptableObjectItemState.itemScriptableObject.id] 
                    = scriptableObjectItemState.itemState;
            }
            return result;
        }
    }
    
    public Dictionary<int, int> StackItems
    {
        get
        {
            var result = new Dictionary<int, int>();
            foreach (var stackItem in stackItems)
            {
                result[stackItem.stackTypeScriptableObject.id] 
                    = stackItem.number;
            }
            return result;
        }
    }

#if UNITY_EDITOR
    [ButtonMethod]
    public void RemoveNullItems()
    {
        AssetDatabase.StartAssetEditing();
        var nullScriptableObjectsInInventory = new List<ScriptableObjectInInventory>();
        for (var i = 0; i < scriptableObjectsInventory.Count; i++)
        {
            if (scriptableObjectsInventory[i].itemScriptableObject == null)
            {
                nullScriptableObjectsInInventory.Add(scriptableObjectsInventory[i]);
            }
        }
        foreach (var nullScriptableObjectInInventory in nullScriptableObjectsInInventory)
        {
            scriptableObjectsInventory.Remove(nullScriptableObjectInInventory);
        }
        
        var nullStackItems = new List<StackItem>();
        for (var i = 0; i < stackItems.Count; i++)
        {
            if (stackItems[i].stackTypeScriptableObject == null)
            {
                nullStackItems.Add(stackItems[i]);
            }
        }
        foreach (var nullStackItem in nullStackItems)
        {
            stackItems.Remove(nullStackItem);
        }

        AssetDatabase.StopAssetEditing();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
    
    [System.Serializable]
    public class InventoryReorderableArray : ReorderableArray<ScriptableObjectInInventory> {
    }
    
    [System.Serializable]
    public class StackItemsReorderableArray : ReorderableArray<StackItem> {
    }
}