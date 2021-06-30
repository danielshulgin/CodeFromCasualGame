using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DoublePriceUI : PriceUI
{
    [SerializeField] private Sprite coinSprite;
    
    [SerializeField] private Sprite crystalSprite;

    [SerializeField] private Image leftCurrencyImage;
    
    [SerializeField] private Image rightCurrencyImage;

    [SerializeField] private TextMeshProUGUI leftPriceText;
    
    [SerializeField] private TextMeshProUGUI rightPriceText;
    
    [SerializeField] private PriceUIUpdateMode priceUiUpdateMode;
    

    public override void Initialize(Currency price)
    {
        switch (price.currencyType)
        {
            case CurrencyType.Coins:
                if (priceUiUpdateMode == PriceUIUpdateMode.SingleLeft)
                {
                    leftCurrencyImage.gameObject.SetActive(true);
                    rightCurrencyImage.gameObject.SetActive(false);
                    leftPriceText.gameObject.SetActive(true);
                    rightPriceText.gameObject.SetActive(false);
                    
                    leftCurrencyImage.sprite = coinSprite;
                    leftPriceText.text = price.coins.ToString();
                }
                else
                {
                    leftCurrencyImage.gameObject.SetActive(false);
                    rightCurrencyImage.gameObject.SetActive(true);
                    leftPriceText.gameObject.SetActive(false);
                    rightPriceText.gameObject.SetActive(true);
                    
                    rightCurrencyImage.sprite = coinSprite;
                    rightPriceText.text = price.coins.ToString();
                }
                break;
            case CurrencyType.Crystals:
                if (priceUiUpdateMode == PriceUIUpdateMode.SingleLeft)
                {
                    leftCurrencyImage.gameObject.SetActive(true);
                    rightCurrencyImage.gameObject.SetActive(false);
                    leftPriceText.gameObject.SetActive(true);
                    rightPriceText.gameObject.SetActive(false);
                    
                    leftCurrencyImage.sprite = crystalSprite;
                    leftPriceText.text = price.crystals.ToString();
                }
                else
                {
                    leftCurrencyImage.gameObject.SetActive(false);
                    rightCurrencyImage.gameObject.SetActive(true);
                    leftPriceText.gameObject.SetActive(false);
                    rightPriceText.gameObject.SetActive(true);

                    rightCurrencyImage.sprite = crystalSprite;
                    rightPriceText.text = price.crystals.ToString();
                }
                break;
            case CurrencyType.CoinsCrystals:
                leftCurrencyImage.gameObject.SetActive(true);
                rightCurrencyImage.gameObject.SetActive(true);
                leftPriceText.gameObject.SetActive(true);
                rightPriceText.gameObject.SetActive(true);

                leftCurrencyImage.sprite = crystalSprite;
                rightCurrencyImage.sprite = coinSprite;
                leftPriceText.text = price.crystals.ToString();
                rightPriceText.text = price.coins.ToString();
                break;
        }
    }

    public override void Hide()
    {
        leftCurrencyImage.gameObject.SetActive(false);
        rightCurrencyImage.gameObject.SetActive(false);
        leftPriceText.gameObject.SetActive(false);
        rightPriceText.gameObject.SetActive(false);
    }
}

public enum PriceUIUpdateMode
{
    SingleLeft, SingleRight
}
