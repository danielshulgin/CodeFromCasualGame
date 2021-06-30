using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class PlayerDAO
{
    public Dictionary<int, ItemState> itemStateById;

    public Dictionary<int, int> numberInStackById;

    public int coins;

    public int crystals;

    public int currentBulletSkinId;

    public int currentVasylSkinId;
    
    public int currentLevelId;

    public int currentCannonballId;

    public int currentCannonId;
    
    public List<int> passedTutorialParts = new List<int>();

    public string deviceId = "";

    
    public PlayerDAO() { }

    public PlayerDAO(DefaultPlayerSettings defaultPlayerSettings)
    {
        coins = defaultPlayerSettings.coins;
        crystals = defaultPlayerSettings.crystals;
        currentBulletSkinId = defaultPlayerSettings.currentBulletSkin.id;
        currentVasylSkinId = defaultPlayerSettings.currentVasylSkin.id;
        currentLevelId = defaultPlayerSettings.currentLevel.id;
        currentCannonballId = defaultPlayerSettings.currentCannonball.id;
        currentCannonId = defaultPlayerSettings.currentCannon.id;
        itemStateById = defaultPlayerSettings.ItemStatesById;
        numberInStackById = defaultPlayerSettings.StackItems;
        passedTutorialParts = new List<int>(defaultPlayerSettings.passedTutorialParts);
        deviceId = SystemInfo.deviceUniqueIdentifier;
    }
    
    public void AddNewId(DefaultPlayerSettings defaultPlayerSettings)
    {
        var defaultItemStateById = defaultPlayerSettings.ItemStatesById;
        var defaultStackTypeById = defaultPlayerSettings.StackItems;
        if (currentCannonballId == 0)
        {
            currentCannonballId = defaultPlayerSettings.currentCannonball.id;
        }
        if (currentCannonId == 0)
        {
            currentCannonId = defaultPlayerSettings.currentCannon.id;;
        }
        if (itemStateById == null)
        {
            itemStateById = defaultItemStateById;
        }

        if (numberInStackById == null)
        {
            numberInStackById = defaultStackTypeById;
        }
        
        foreach (var itemId in defaultItemStateById)
        {
            if (!itemStateById.Keys.Contains(itemId.Key))
            {
                itemStateById[itemId.Key] = itemId.Value;
            }
        }
        
        foreach (var stackTypeId in defaultStackTypeById)
        {
            if (!numberInStackById.Keys.Contains(stackTypeId.Key))
            {
                numberInStackById[stackTypeId.Key] = stackTypeId.Value;
            }
        }
    }
}