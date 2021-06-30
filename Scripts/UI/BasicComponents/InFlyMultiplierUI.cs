using System;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;

public class InFlyMultiplierUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    
    [SerializeField] private InFlyMultiplier inFlyMultiplier;
    
    [SerializeField] private float textScaleChangeSize = 1.3f;

    [SerializeField] private float textScaleChangeTime = 0.3f;
    
    [SerializeField] private float changeBaseInFlyMultiplierTime = 0.5f;
    
    [SerializeField] private Color changeBaseInFlyMultiplierColor = new Color(1f, 0.01f, 0f, 1f);

    [SerializeField] private float finalTextAlpha = 0.5f;

    private Sequence _textEffectsSequence;
    
    
    private void Awake()
    {
        inFlyMultiplier.OnChangeInFlyMultiplier += HandleChangeInFlyMultiplier;
        inFlyMultiplier.OnResetInFlyMultiplier += HandleResetInFlyMultiplier;
        inFlyMultiplier.OnChangeBaseInFlyMultiplier += HandleChangeBaseInFlyMultiplier;

        GameState.OnEndFly += HandleEndFly;
    }

    [ButtonMethod]
    private void Scale()
    {
        transform.localScale = Vector3.one;
        transform.DOScale(2f, 2f);
    }

    private void HandleEndFly()
    {
        _textEffectsSequence?.Kill();
        text.alpha = 0f;
        text.text = "1X";
    }

    private void HandleChangeInFlyMultiplier(float value)
    {
        text.alpha = 1f;
        text.transform.localScale = Vector3.one;
        text.color = new Color(1f, 1f, 1f, 1f);
        _textEffectsSequence?.Kill();

        _textEffectsSequence = DOTween.Sequence()
            .Append(text.transform.DOScale(textScaleChangeSize, textScaleChangeTime).SetEase(Ease.OutFlash, 2f))
            .Join(text.DOFade(finalTextAlpha,
                inFlyMultiplier.InFlyMultiplierResetTime));
        
        SetTextString(value);
    }

    private void HandleChangeBaseInFlyMultiplier(float value)
    {
        text.alpha = 1f;
        text.color = changeBaseInFlyMultiplierColor;
        text.transform.localScale = Vector3.one;
        _textEffectsSequence?.Kill();

        _textEffectsSequence = DOTween.Sequence()
            .Append(text.transform.DOScale(textScaleChangeSize, textScaleChangeTime).SetEase(Ease.OutFlash, 2f))
            .Join(text.DOFade(finalTextAlpha,
                inFlyMultiplier.InFlyMultiplierResetTime));
        
        SetTextString(value);
    }

    private void SetTextString(float value)
    {
        if (value - (int)value > float.Epsilon)
        {
            text.text = $"{value:0.0}" + "X"; 
        }
        else
        {
            text.text = (int)value + "X"; 
        }
    }

    private void HandleResetInFlyMultiplier(float value)
    {
        text.alpha = 0f;
        text.transform.localScale = Vector3.one;
        _textEffectsSequence?.Kill();
    }
}
