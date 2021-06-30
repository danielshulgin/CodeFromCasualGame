using UnityEngine;
using Zenject;

public class PlayerDataPart : MonoBehaviour
{
    [Inject] protected PlayerData _playerData;
    
    
    public virtual ItemState GetItemState(ItemScriptableObject itemScriptableObject)
    {
        return _playerData.ItemStateById[itemScriptableObject.id];
    }
}
