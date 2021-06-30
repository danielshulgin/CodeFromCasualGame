using System.Collections.Generic;
using UnityEngine;


public class ItemScriptableObject : ScriptableObject, IHaveStoreName, IHaveStoreSprites
{
    public Currency price;

    public Sprite storeSprite;

    public Sprite storeMiniSprite;

    public int id;

    public string storeNameId = "empty_store_name";

    public List<ItemScriptableObject> makesBought;
    
    public List<ItemScriptableObject> makesAvailable;

    public virtual string StoreName => Localization.Instance.GetTranslation(storeNameId);
    
    public virtual Sprite StoreSprite => storeSprite;
    
    public virtual Sprite StoreMiniSprite => storeMiniSprite;
}
