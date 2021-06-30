using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadePanel : MonoBehaviour
{
    [SerializeField] private Color transparentBlack = new Color(1f, 1f, 1f, 0f);
    
    [SerializeField] private Color black = new Color(1f, 1f, 1f, 1f);
    
    [SerializeField] private float standardDuration = 1.2f;
    
    private Image _panel;

    private Sequence _sequence;
    
    public static FadePanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Multiple FadePanel instances");
        }

        _panel = GetComponent<Image>();
    }

    [ButtonMethod]
    public void UnFade()
    {
        UnFade(standardDuration);
    }
    
    public void UnFade(float duration)
    {
        _sequence = DOTween.Sequence()
            .Append(_panel.DOColor(black, 0f))
            .Append(_panel.DOColor(transparentBlack, duration));
    }

    public void Stop()
    {
        _sequence?.Kill();
        _panel.color = transparentBlack;
    }
}
