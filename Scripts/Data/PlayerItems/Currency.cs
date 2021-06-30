using System;
using UnityEngine;

[System.Serializable]
public struct Currency
{
    public int coins;
    
    public int crystals;

    public CurrencyType currencyType;

    public Currency(int coins, int crystals, CurrencyType currencyType)
    {
        if (coins < 0)
        {
            coins = 0;
        }
        
        if (crystals < 0)
        {
            crystals = 0;
        }
        
        this.coins = coins;
        this.crystals = crystals;
        this.currencyType = currencyType;
    }

    public Currency CalculateLack(int currentCoins, int currentCrystals)
    {
        var coinsLack = Mathf.Abs(currentCoins - coins);
        var crystalsLack = Mathf.Abs(currentCrystals - crystals);
        switch (currencyType)
        {
            case CurrencyType.Coins:
                return new Currency(coinsLack, crystalsLack, currencyType);
            case CurrencyType.Crystals:
                return new Currency(0, crystalsLack, currencyType);
            case CurrencyType.CoinsCrystals:
                return new Currency(coinsLack, crystalsLack, currencyType);
        }
        return new Currency(0, 0, currencyType);
    }

    public Currency CalculateLack(PlayerData playerData)
    {
        return CalculateLack(playerData.Coins, playerData.Crystals);
    }
    
    public bool CanBuy(int currentCoins, int currentCrystals)
    {
        return ((currentCoins - coins) >= 0) && ((currentCrystals - crystals) >= 0);
    }
    
    public static Currency operator *(Currency currency, int number)
    {
        return new Currency(currency.coins * number, currency.crystals * number, currency.currencyType);
    }
    
    public static Currency operator +(Currency first, Currency second)
    {
        return new Currency(first.coins + second.coins, first.crystals + second.crystals, first.currencyType);
    }
}

public enum CurrencyType
{
    Coins, Crystals, CoinsCrystals
}
