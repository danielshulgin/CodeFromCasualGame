using System;
using System.Collections.Generic;
using System.Linq;
using Malee;
using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Zenject;

public class AdsAchievementsManager : MonoBehaviour
{
    public UnityEvent OnChange;
    
    public UnityEvent OnAdsReady;
    
    public Action OnSuccessEndWatch;

    [SerializeField] private UnityAds unityAds;
    
    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;
    
    [Inject] private PlayerLevels _playerLevels;
    
    [Inject] private PlayerData _playerData;

    private DateTime _lastUpdateTime;

    public TimeSpan TimeToNewAchievements { get; private set; }

    public bool AdsReady => unityAds.Ready;
    
    public int CurrentTaskIndex { get; private set; }
    
    public List<Currency> LastAdsRewards { get; private set; }

    public List<Currency> DefaultAdsRewards => defaultPlayerSettings.adsRewardsCurrency;
    
    public List<Currency> RecalculatedAdsRewards { get; private set; }


    private void Awake()
    {
        RestoreLastUpdateTime();
        LastAdsRewards = LoadLastRewards();
    }

    private void RestoreLastUpdateTime()
    {
        var lastUpdateTimeString = PlayerPrefs.GetString("AdsAchievementLastUpdateTime", "");

        if (lastUpdateTimeString == "")
        {
            UpdateAchievement();
            return;
        }

        _lastUpdateTime = DateTime.FromBinary(Convert.ToInt64(lastUpdateTimeString));
    }

    private void OnEnable()
    {
        unityAds.OnUnityAdsReadyEvent.AddListener(HandleAdsReady);
        unityAds.OnSuccessEndWatchEvent.AddListener(SuccessEndWatch);
        var currencyAdd = CalculateCurrencyAdd();
        RecalculatedAdsRewards = CalculateAdsRewards(currencyAdd);
        TryUpdateAchievement();
        CurrentTaskIndex = PlayerPrefs.GetInt("CompleteAdsTasks", 0);
        OnChange.Invoke();
    }
    
    private void OnDisable()
    {
        unityAds.OnUnityAdsReadyEvent.RemoveListener(HandleAdsReady);
        unityAds.OnSuccessEndWatchEvent.RemoveListener(SuccessEndWatch);
    }
    
    private void Update()
    {
        TimeToNewAchievements = new TimeSpan(23 - DateTime.Now.Hour, 59 - DateTime.Now.Minute, 0);
        TryUpdateAchievement();
    }
    
    private bool TryUpdateAchievement()
    {
        var updateTimeSpan = DateTime.Now.Subtract(_lastUpdateTime);
        if (updateTimeSpan.TotalDays < 0 || updateTimeSpan.TotalDays >= 1)
        {
            UpdateAchievement();
            return true;
        }
        return false;
    }

    [ButtonMethod]
    private void UpdateAchievement()
    {
        _lastUpdateTime = DateTime.Now;
        PlayerPrefs.SetString("AdsAchievementLastUpdateTime", DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetInt("CompleteAdsTasks", 0);
        PlayerPrefs.SetString("LastAdsRewards", "");
        CurrentTaskIndex = 0;
        OnChange.Invoke();
    }
    
    public void ShowRewardedVideo()
    {
        unityAds.ShowRewardedVideo();
    }
    
    private void SuccessEndWatch()
    {
        PlayerPrefs.SetInt("CompleteAdsTasks", CurrentTaskIndex + 1);
        SaveLastRewards(RecalculatedAdsRewards[CurrentTaskIndex], CurrentTaskIndex);
        RewardPlayer();
        ++CurrentTaskIndex;
        
        OnSuccessEndWatch.Invoke();
    }

    private void RewardPlayer()
    {
        _playerData.AddCurrencyWithoutNotify(RecalculatedAdsRewards[CurrentTaskIndex]);
    }
    
    private void HandleAdsReady()
    {
        OnAdsReady.Invoke();
    }

    private Currency CalculateCurrencyAdd()
    {
        var coinsAdd = 0;
        foreach (var levelPart in _playerLevels.CurrentLevel.levelPartScriptableObjects)
        {
            if (_playerLevels.GetItemState(levelPart) == ItemState.Bought)
            {
                var boughtObstaclesNumber = levelPart
                    .increasingRewardObstacles
                    .Count(obstacle => _playerLevels.GetItemState(obstacle) == ItemState.Bought);
                
                coinsAdd += boughtObstaclesNumber * levelPart.obstacleAdsRewardIncrease.coins;
                
                coinsAdd += levelPart.selfAdsRewardIncrease.coins;
            }
        }
        return new Currency(coinsAdd, 0, CurrencyType.CoinsCrystals);
    }
    
    private List<Currency> CalculateAdsRewards(Currency currencyAdd)
    {
        var adsRewards = new List<Currency>();
        foreach (var defaultCurrency in DefaultAdsRewards)
        {
            adsRewards.Add(new Currency(defaultCurrency.coins + currencyAdd.coins,
                    defaultCurrency.crystals, defaultCurrency.currencyType));
        }
        return adsRewards;
    }

    private void SaveLastRewards(Currency lastReward, int lastRewardIndex)
    {
        LastAdsRewards[lastRewardIndex] = lastReward;
        var lastRewardsString = JsonUtility.ToJson(new LastAdsRewardDAO(LastAdsRewards));
        PlayerPrefs.SetString("LastAdsRewards", lastRewardsString);
    }

    private List<Currency> LoadLastRewards()
    {
        var defaultValues = new List<Currency>();
        foreach (var reward in DefaultAdsRewards)
        {
            var copy = reward;
            defaultValues.Add(copy);
        }
        var lastRewardsString = PlayerPrefs.GetString("LastAdsRewards", "");
        if (lastRewardsString == "")
        {
            return defaultValues;
        }

        var lastAdsRewards = JsonUtility.FromJson<LastAdsRewardDAO>(lastRewardsString);
        if (lastAdsRewards == null)
        {
            return defaultValues;
        }

        return lastAdsRewards.lastAdsReward;
    }

    [System.Serializable]
    private class LastAdsRewardDAO
    {
        public List<Currency> lastAdsReward;

        public LastAdsRewardDAO(List<Currency> lastAdsReward)
        {
            this.lastAdsReward = lastAdsReward;
        }
    }
}
