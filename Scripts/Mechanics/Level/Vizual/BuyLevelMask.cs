using System;
using UnityEngine;


public class BuyLevelMask : MonoBehaviour
{
    [SerializeField] private GameObject maskGameObject;
    
    private float _targetOffsetX = 17;
    
    private void Start()
    {
        CalculateTargetOffset();
        ResizeSpriteToScreen();
        LevelSwitcher.OnUpdateRelativePosition += SetBackgroundX;
    }

    private void OnDestroy()
    {
        LevelSwitcher.OnUpdateRelativePosition -= SetBackgroundX;
    }

    private void CalculateTargetOffset()
    {
        var topRightCorner = new Vector2(1, 1);
        var edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        _targetOffsetX = edgeVector.x * 2;
    }

    private void SetBackgroundX(float x)
    {
        var prevPosition = maskGameObject.transform.position;
        maskGameObject.transform.position = new Vector3(x * _targetOffsetX, prevPosition.y, prevPosition.z);
    }
    
    private void ResizeSpriteToScreen() 
    {
        var spriteRenderer = maskGameObject.GetComponent<SpriteMask>();
        if (spriteRenderer == null) return;
     
        maskGameObject.transform.localScale = new Vector3(1,1,1);
     
        var width = spriteRenderer.sprite.bounds.size.x;
        var height = spriteRenderer.sprite.bounds.size.y;
     
        var worldScreenHeight = Camera.main.orthographicSize * 2f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        maskGameObject.transform.localScale = 
            new Vector3(worldScreenWidth / width,worldScreenHeight/ height,1);
    }    
}
