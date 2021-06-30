using System;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class CollectedResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    
    [SerializeField] private TextMeshProUGUI crystalsText;
    
    [SerializeField] private float textScaleChangeSize = 1.2f;
    
    [SerializeField] private float textScaleChangeTime = 0.3f;

    private bool _animate;

    private Sequence _coinsAnimationSequence;
    
    private Sequence _crystalsAnimationSequence;
    
    
    private void Awake()
    {
        HandleUpdateResult(new Currency(0, 0, CurrencyType.CoinsCrystals));
        FlyResults.OnUpdateResult += HandleUpdateResult;
        
        GameState.OnStartFly += HandleStartFly;
        GameState.OnEndFly += HandleEndFly;
    }

    private void OnDestroy()
    {
        FlyResults.OnUpdateResult -= HandleUpdateResult;
        
        GameState.OnStartFly -= HandleStartFly;
        GameState.OnEndFly -= HandleEndFly;
    }

    private void HandleUpdateResult(Currency currentResult)
    {
        if (_animate)
        {
            coinsText.transform.localScale = Vector3.one;
            crystalsText.transform.localScale = Vector3.one;
            
            if (currentResult.coins.ToString() != coinsText.text)
            {
                _coinsAnimationSequence?.Kill();
                _coinsAnimationSequence = DOTween.Sequence().Append(
                    coinsText.transform.DOScale(textScaleChangeSize, textScaleChangeTime).SetEase(Ease.OutFlash, 2f));
            }
            if (currentResult.crystals.ToString() != crystalsText.text)
            {
                _crystalsAnimationSequence?.Kill();
                _crystalsAnimationSequence = DOTween.Sequence().Append(
                    crystalsText.transform.DOScale(textScaleChangeSize, textScaleChangeTime).SetEase(Ease.OutFlash, 2f));
            }
        }
        
        coinsText.text = currentResult.coins.ToString();
        crystalsText.text = currentResult.crystals.ToString();
    }

    private void HandleStartFly()
    {
        _animate = true;
    }
    
    private void HandleEndFly()
    {
        _animate = false;
    }
}
