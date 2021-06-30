using UnityEngine;
using UnityEngine.UI;

public static class UIHelperFunctions
{
    public static void SetActiveCanvasGroup(CanvasGroup canvasGroup, bool active)
    {
        if (active)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            return;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public static void SetVisibleImage(Image image, bool visible)
    {
        var prevColor = image.color;
        image.color = new Color(prevColor.r, prevColor.g, prevColor.b, visible ? 1f: 0f);
    }
    
    public static void ChangeNormalButtonColor(Button button, Color newColor)
    {
        var buttonColorBlock = button.colors;
        buttonColorBlock.normalColor = newColor;
        button.colors = buttonColorBlock;
    }
    
    public static void ChangeDisabledButtonColor(Button button, Color newColor)
    {
        var buttonColorBlock = button.colors;
        buttonColorBlock.disabledColor = newColor;
        button.colors = buttonColorBlock;
    }
    
    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
        rect.x -= (transform.pivot.x * size.x);
        rect.y -= ((1.0f - transform.pivot.y) * size.y);
        return rect;
    }
}
