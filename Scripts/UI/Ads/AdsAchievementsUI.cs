using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class AdsAchievementsUI : MonoBehaviour
{
    [SerializeField] private AdsAchievementsManager adsAchievementsManager;
    
    [SerializeField] private CoinsCrystalsPanel coinsCrystalsPanel;

    [SerializeField] private Button watchButton;
    
    [SerializeField] private RectTransform gradientMaskRectTransform;
    
    [SerializeField] private RectTransform gradientContentRectTransform;
    
    [SerializeField] private TextMeshProUGUI timeToNewAchievement;

    [SerializeField] private List<AdsTab> adsItems = new List<AdsTab>();
    
    [SerializeField] private UIItemsStream coinsStream;
    
    [SerializeField] private UIItemsStream crystalsStream;
    
    [SerializeField] private float scaleChangeTime = 0.7f;
    
    [SerializeField] private float coinsCrystalsChangeTime = 1.5f;
    
    [FormerlySerializedAs("coinsInStreamNumber")] [SerializeField] private int maxCoinsInStreamNumber = 5;
    
    [FormerlySerializedAs("crystalsInStreamNumber")] [SerializeField] private int maxCrystalsInStreamNumber = 6;
    
    [Inject] private PlayerData _playerData;


    private void Start()
    {
        adsAchievementsManager.OnChange.AddListener(Initialize);
        adsAchievementsManager.OnSuccessEndWatch += SuccessEndWatch;
        coinsCrystalsPanel.GetComponent<Button>().onClick.AddListener(Exit);
    }

    private void OnDisable()
    {
        coinsStream.StopStream();
        crystalsStream.StopStream();
        StopAllCoroutines();
        coinsCrystalsPanel.HandlePlayerCurrencyChange(new Currency(_playerData.Coins, _playerData.Crystals, CurrencyType.CoinsCrystals));
    }

    private void OnDestroy()
    {
        adsAchievementsManager.OnChange.RemoveListener(Initialize);
        adsAchievementsManager.OnSuccessEndWatch -= SuccessEndWatch;
        coinsCrystalsPanel.GetComponent<Button>().onClick.RemoveListener(Exit);
        coinsStream.StopStream();
        crystalsStream.StopStream();
    }

    public void Initialize()
    {
        UpdateScale();
        
        for (var i = 0; i < adsAchievementsManager.CurrentTaskIndex; i++)
        {
            adsItems[i].priceUI.Initialize(adsAchievementsManager.LastAdsRewards[i]);
        }

        for (var i = adsAchievementsManager.CurrentTaskIndex; i < adsAchievementsManager.RecalculatedAdsRewards.Count; i++)
        {
            adsItems[i].priceUI.Initialize(adsAchievementsManager.RecalculatedAdsRewards[i]);
        }
        
        watchButton.interactable = adsAchievementsManager.AdsReady 
                    && adsAchievementsManager.CurrentTaskIndex < adsAchievementsManager.RecalculatedAdsRewards.Count;
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        var remainingTime = adsAchievementsManager.TimeToNewAchievements;
        timeToNewAchievement.text = $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}";
    }

    public void Watch()
    {
        adsAchievementsManager.ShowRewardedVideo();
    }

    private void SuccessEndWatch()
    {
        watchButton.interactable = adsAchievementsManager.AdsReady && adsAchievementsManager.CurrentTaskIndex < 5;
        UpdateScale(false);
        StartCoroutine(PlayerCoinsCrystalsChangeRoutine());
        StartCurrencyStream();
        AudioManager.Instance.PlayWithOverlay("ui_award");
    }

    private void StartCurrencyStream()
    {
        var offset = adsItems[adsAchievementsManager.CurrentTaskIndex - 1].leftStreamStartPosition.rect.width / 2f;
        
        var startLeftRectPosition =
            adsItems[adsAchievementsManager.CurrentTaskIndex - 1].leftStreamStartPosition.transform.position;
        var leftStreamStart = new Vector3(startLeftRectPosition.x - offset, startLeftRectPosition.y);
        
        var startRightRectPosition =
            adsItems[adsAchievementsManager.CurrentTaskIndex - 1].rightStreamStartPosition.transform.position;
        var rightStreamStart = new Vector3(startRightRectPosition.x - offset, startRightRectPosition.y);
        
        var rewardCurrency = adsAchievementsManager
            .RecalculatedAdsRewards[adsAchievementsManager.CurrentTaskIndex - 1];
        switch (rewardCurrency.currencyType)
        {
            case CurrencyType.Coins:
                coinsStream.StartStream(
                    Mathf.Clamp( maxCoinsInStreamNumber, 0, rewardCurrency.coins), leftStreamStart);
                break;
            case CurrencyType.Crystals:
                crystalsStream.StartStream(
                    Mathf.Clamp( maxCrystalsInStreamNumber, 0, rewardCurrency.crystals), leftStreamStart);
                break;
            case CurrencyType.CoinsCrystals:
                crystalsStream.StartStream(
                    Mathf.Clamp( maxCrystalsInStreamNumber, 0, rewardCurrency.crystals), leftStreamStart);
                coinsStream.StartStream(
                    Mathf.Clamp( maxCoinsInStreamNumber, 0, rewardCurrency.coins), rightStreamStart);
                break;
        }
    }

    private void AdsReady()
    {
        if (adsAchievementsManager.CurrentTaskIndex < adsAchievementsManager.RecalculatedAdsRewards.Count)
        {
            watchButton.interactable = true;
        }
    }

    private void UpdateScale(bool immediately = true)
    {
        var value = 1f;
        
        if (adsAchievementsManager.CurrentTaskIndex <= 0)
        {
            value = 0.001f;
        }
        else if (adsAchievementsManager.CurrentTaskIndex < 5)
        {
            value = 0.178f * adsAchievementsManager.CurrentTaskIndex;
        }

        if (immediately)
        {
            gradientMaskRectTransform.localScale = new Vector3(1f, value, 1f);
            gradientContentRectTransform.localScale = new Vector3(1f, 1f / value, 1f);
        }
        else
        {
            StartCoroutine(ScaleChangeRoutine(gradientMaskRectTransform.localScale.y, value ));
        }
    }
    
    IEnumerator ScaleChangeRoutine(float prevScale, float newScale)
    {
        var rewardCurrency = adsAchievementsManager
            .RecalculatedAdsRewards[adsAchievementsManager.CurrentTaskIndex  - 1];
        var delta = newScale - prevScale;
        for (var time = 0f; time < 1f; time += Time.deltaTime / scaleChangeTime)
        {
            var value = prevScale + delta * time;
            gradientMaskRectTransform.localScale = new Vector3(1f, value, 1f);
            gradientContentRectTransform.localScale = new Vector3(1f, 1f / value, 1f);
            yield return null;
        }
    }
    
    IEnumerator PlayerCoinsCrystalsChangeRoutine()
    {
        var rewardCurrency = adsAchievementsManager
            .RecalculatedAdsRewards[adsAchievementsManager.CurrentTaskIndex  - 1];
        var playerCoinsBefore = _playerData.Coins - rewardCurrency.coins;
        var playerCrystalsBefore = _playerData.Crystals - rewardCurrency.crystals;
        for (var time = 0f; time < 1f; time += Time.deltaTime / coinsCrystalsChangeTime)
        {
            var currentCoins = playerCoinsBefore + Mathf.CeilToInt(time * rewardCurrency.coins);
            var currentCrystals = playerCrystalsBefore + Mathf.CeilToInt(time * rewardCurrency.crystals);
            coinsCrystalsPanel.HandlePlayerCurrencyChange(new Currency(currentCoins, currentCrystals, CurrencyType.CoinsCrystals));
            yield return null;
        }
        coinsCrystalsPanel.HandlePlayerCurrencyChange(new Currency(_playerData.Coins, _playerData.Crystals, CurrencyType.CoinsCrystals));
    }
}
