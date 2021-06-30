using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractableTrigger : MonoBehaviour
{
    public UnityEvent OnCollect;

    [SerializeField] private bool reusable;

    private bool _collected;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        var breakablePart = other.GetComponent<BreakablePart>();

        if (breakablePart == null || !breakablePart.active || (_collected && !reusable))
        {
            return;
        }

        Collect();
    }

    private void Collect()
    {
        OnCollect.Invoke();
        _collected = true;
    }
}
