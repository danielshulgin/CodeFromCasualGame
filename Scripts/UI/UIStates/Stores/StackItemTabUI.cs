public class StackItemTabUI : GameCurrencyStoreTabUI
{
    public override void UpdateItemState(ItemScriptableObject itemScriptableObject, ItemState itemState)
    {
        base.UpdateItemState(itemScriptableObject, itemState);
        stateImage.gameObject.SetActive(false);
    }

    public override void EnableCheckMark()
    {
        stateImage.gameObject.SetActive(true);
        UIHelperFunctions.SetVisibleImage(stateImage, true);
        stateImage.sprite = checkMarkSprite;
    }
}
