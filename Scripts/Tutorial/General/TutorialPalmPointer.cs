using System.Collections.Generic;
using UnityEngine;

public class TutorialPalmPointer : MonoBehaviour
{
    [SerializeField] private GameObject palmPointerPrefab;
    
    [SerializeField] private Canvas canvas;
    
    private GameObject _palmPointer;
    
    private Animator _palmPointerAnimator;

    private Dictionary<PalmPointerAnimationType, string> _animationTypeToString;
    
    public void Initialize()
    {
        if (_palmPointer == null)
        {
            _palmPointer = Instantiate(palmPointerPrefab, canvas.transform);
            _palmPointer.transform.SetSiblingIndex(_palmPointer.transform.GetSiblingIndex() - 1);
        }
        _palmPointerAnimator = _palmPointer.GetComponent<Animator>();
        _animationTypeToString = new Dictionary<PalmPointerAnimationType, string>
        {
            [PalmPointerAnimationType.Press] = "palm_pointer_press", 
            [PalmPointerAnimationType.Tap] = "palm_pointer_tap"
        };
    }
    
    public void Hide()
    {
        _palmPointer.SetActive(false);
    }

    public void PlayPalmAnimationOnUI(PalmPointerAnimationType animationType, RectTransform targetPoint,
        Vector2 offset = new Vector2())
    {
        _palmPointer.SetActive(true);
        _palmPointer.transform.SetParent(targetPoint);
        _palmPointer.GetComponent<RectTransform>().transform.position = (Vector2)targetPoint.position + offset;
        _palmPointerAnimator.Play(_animationTypeToString[animationType]);
    }
    
    public void PlayPalmAnimationOnWordPoint(PalmPointerAnimationType animationType, Transform targetPoint,
        Vector2 offset = new Vector2())
    {
        _palmPointer.SetActive(true);
        
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(targetPoint.position);  
        _palmPointer.transform.SetParent(canvas.transform);
        
        var canvasRect = canvas.GetComponent<RectTransform>();
        var viewportPosition= Camera.main.WorldToViewportPoint(targetPoint.transform.position);
        var worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        
        _palmPointer.GetComponent<RectTransform>()
            .anchoredPosition = worldObjectScreenPosition + offset;
        
        _palmPointerAnimator.Play(_animationTypeToString[animationType]);
    }
}

public enum PalmPointerAnimationType
{
    Tap, Press
}
