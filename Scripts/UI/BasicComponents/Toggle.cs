using System.Collections;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    public MyBoolEvent OnValueChanged;
    
    [SerializeField] private bool value;
    
    [SerializeField] private RectTransform handle;
    
    
    [SerializeField] [Range(0f, 1f)] private float speed = 0.2f;
    
    private bool _moving;
    
    private Vector3 _startAnchoredPosition;
    
    private Vector3 _targetAnchoredPosition;

    private float _epsilonOffsetX = 2f;

    private void Awake()
    {
        var rectTransform = GetComponent<RectTransform>();
        _startAnchoredPosition = handle.anchoredPosition;
        var rectWidth = rectTransform.rect.width;
        _targetAnchoredPosition = new Vector3( rectWidth - _startAnchoredPosition.x, _startAnchoredPosition.y);
    }

    public void ToggleClick()
    {
        if (!_moving)
        {
            value = !value;
            StartCoroutine(StartMovingHandle());
            OnValueChanged.Invoke(value);
        }
    }

    public void SetToggle(bool active)
    {
        value = active;
        if (active)
        {
            handle.anchoredPosition = _targetAnchoredPosition;
            return;
        }
        handle.anchoredPosition = _startAnchoredPosition;
    }
    
    private IEnumerator StartMovingHandle()
    {
        _moving = true;
        
        if (value)
        {
            while (_targetAnchoredPosition.x - handle.anchoredPosition.x > _epsilonOffsetX)
            {
                var handleNewAnchoredPosition = Vector3.Lerp(handle.anchoredPosition,_targetAnchoredPosition, speed);
                handle.anchoredPosition = handleNewAnchoredPosition;
                yield return null;
            }
        }
        else
        {
            while (handle.anchoredPosition.x - _startAnchoredPosition.x > _epsilonOffsetX)
            {
                var handleNewAnchoredPosition = Vector3.Lerp(handle.anchoredPosition,_startAnchoredPosition, speed);
                handle.anchoredPosition = handleNewAnchoredPosition;
                yield return null;
            }
        }
        
        _moving = false;
    }
}
