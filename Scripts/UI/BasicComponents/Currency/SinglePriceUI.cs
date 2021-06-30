using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SinglePriceUI : PriceUI
{
    [SerializeField] private Sprite coinSprite;
    
    [SerializeField] private Sprite crystalSprite;

    [SerializeField] private Image currencyImage;

    [SerializeField] private TextMeshProUGUI priceText;

    public override void Initialize(Currency price)
    {
        switch (price.currencyType)
        {
            case CurrencyType.Coins:

                UIHelperFunctions.SetVisibleImage(currencyImage, true);
                currencyImage.sprite = coinSprite;
                priceText.text = price.coins.ToString();
                break;
            case CurrencyType.Crystals:
                UIHelperFunctions.SetVisibleImage(currencyImage, true);
                currencyImage.sprite = crystalSprite;
                priceText.text = price.crystals.ToString();
                break;
            case CurrencyType.CoinsCrystals:
                break;
        }
    }

    public override void Hide()
    {
        UIHelperFunctions.SetVisibleImage(currencyImage, false);
        priceText.text = "";
    }
}
