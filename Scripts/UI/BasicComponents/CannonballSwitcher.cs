using System;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class CannonballSwitcher : SwipeSwitcher, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int pageCount = 3;

    public int currentPage = 1;
    
    public RectTransform frameRect;
    
    public RectTransform panelRect;

    private float startPos;
    
    private Canvas canvas;
    
    protected override float Width => frameRect.rect.width * canvas.scaleFactor / 4f;

    protected override bool Press => _press;
    
    private bool _press;

    public static AnimationCurve temp; 


    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        startPos = panelRect.anchoredPosition.x;
        Debug.Log("panelRect.rect.width: " + panelRect.rect.width);
    }

    protected override void HandleStartSwitch(bool rightDirection)
    {
        var nextPage = rightDirection ? currentPage + 1 : currentPage - 1;
        
        if (nextPage < 0 || nextPage > pageCount)
        {
            return;
        }
    }

    protected override void HandleEndSwitch()
    {
        if (_rightDirection)
        {
            startPos += frameRect.rect.width / 2f;
        }
        else
        {
            startPos -= frameRect.rect.width / 2f;
        }
    }

    protected override void HandleCancelSwitch()
    {
        
    }

    protected override void HandleUpdateRelativePosition(float position)
    {
        Debug.Log(position);
        panelRect.anchoredPosition = 
            new Vector2(startPos + position * frameRect.rect.width / 2f, panelRect.anchoredPosition.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _press = true;
        float difference = eventData.delta.x;
        if (difference < 0)
        {
            HandleSwap(difference, false);    
        }
        else
        {
            HandleSwap(difference, true);
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _press = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
