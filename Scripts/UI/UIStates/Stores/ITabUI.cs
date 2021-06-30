public interface ITabUI
{
    ItemScriptableObject ItemScriptableObject { get; }
    
    void UpdateTab(ItemScriptableObject itemScriptableObject, ItemState itemState);
    
    void UpdateItemState(ItemScriptableObject itemScriptableObject, ItemState itemState);
    
    void Select();

    void SelectWithoutSound();
    
    void DeSelect();
    
    void SetInteractable(bool interactable);
}