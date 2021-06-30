using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class UIItemsStream : MonoBehaviour
{
    public Action OnEndStream;
    
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private AnimationCurve xPositionByTime;
    
    [SerializeField] private AnimationCurve yPositionByTime;
    
    [SerializeField] private AnimationCurve scaleByTime = AnimationCurve.Constant(0f, 1f, 1f);

    [SerializeField] private AnimationCurve angleByTime = AnimationCurve.Constant(0f, 1f, 0f); 

    [FormerlySerializedAs("streamTime")] [SerializeField] private float flyTime = 0.1f;

    [SerializeField] private float nextItemDelay = 0.1f;

    private List<RectTransform> _items = new List<RectTransform>();
    

    public void StartStream(int itemsNumber, Vector3 startPoint)
    {
        StartCoroutine(StreamRoutine(itemsNumber, startPoint, transform.position, null));
    }
    
    public void StartStream(int itemsNumber, Vector3 startPoint, Action callBack)
    {
        StartCoroutine(StreamRoutine(itemsNumber, startPoint, transform.position, callBack));
    }
    
    public void StartStream(int itemsNumber, Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(StreamRoutine(itemsNumber, startPoint, endPoint, null));
    }
    
    public void StartStream(int itemsNumber, Vector3 startPoint, Vector3 endPoint, Action callBack)
    {
        StartCoroutine(StreamRoutine(itemsNumber, startPoint, endPoint, callBack));
    }

    public void StopStream()
    {
        StopAllCoroutines();
        for(var i = _items.Count - 1; i >=0; i--)
        {
            Destroy(_items[i].gameObject);
        }
        _items.Clear();
    }

    IEnumerator StreamRoutine(int number, Vector3 startPoint, Vector3 endPoint, Action callBack)
    {
        var fromDestinationInWorldSpace = endPoint - transform.position;
        var fromDestinationInLocalSpace = transform.InverseTransformVector(fromDestinationInWorldSpace);
        
        var toDestinationInWorldSpace = endPoint - startPoint;
        var toDestinationInLocalSpace = transform.InverseTransformVector(toDestinationInWorldSpace);
        
        for (int i = 0; i < number; i++)
        {
            var item = Instantiate(itemPrefab, transform);
            var rectTransform = item.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            _items.Add(rectTransform);
            
            StartCoroutine(StreamItemRoutine(rectTransform, toDestinationInLocalSpace, fromDestinationInLocalSpace));
            if (i == number - 1)
            {
                StartCoroutine(SandEndStream(callBack));
            }
            yield return new WaitForSeconds(nextItemDelay);
        }
    }
    
    IEnumerator StreamItemRoutine(RectTransform itemRect, Vector3 delta, Vector3 startDelta)
    {
        for (var time = 0f; time < 1f; time += Time.deltaTime / flyTime)
        {
            var position = new Vector2(startDelta.x + delta.x * (xPositionByTime.Evaluate(time) - 1),
                startDelta.y + delta.y * (yPositionByTime.Evaluate(time) - 1));
            itemRect.anchoredPosition = position;
            itemRect.transform.localScale = Vector3.one * scaleByTime.Evaluate(time); 
            itemRect.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angleByTime.Evaluate(time) * 360f)); 
            yield return null;
        }
        Destroy(itemRect.gameObject);
        _items.Remove(itemRect);
    }

    IEnumerator SandEndStream(Action callBack)
    {
        yield return new WaitForSeconds(flyTime);
        callBack?.Invoke();
    }
}
