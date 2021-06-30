using System.Collections;
using UnityEngine;

public class CompleteDieUIState : UIState
{
    [SerializeField] private DoublePriceUI rewardCurrencyUI;
    
    [SerializeField] private float coinsCrystalsChangeTime = 0.7f;

    
    protected override void Awake()
    {
        base.Awake();
        FlyResults.OnAddDieReward += HandleCompleteDieReward;
    }

    public override void Enter()
    {
        SetActiveCanvasGroup(_canvasGroup, true);
    }

    public override void Exit()
    {
        SetActiveCanvasGroup(_canvasGroup, false);
    }

    private void HandleCompleteDieReward(Currency reward)
    {
        StartCoroutine(RewardAnimationRoutine(reward));
    }
    
    IEnumerator RewardAnimationRoutine(Currency reward)
    {
        for (var time = 0f; time < 1f; time += Time.deltaTime / coinsCrystalsChangeTime)
        {
            var currentCoins = Mathf.CeilToInt(time * reward.coins);
            var currentCrystals = Mathf.CeilToInt(time * reward.crystals);
            rewardCurrencyUI.Initialize(new Currency(currentCoins, currentCrystals, reward.currencyType));
            yield return null;
        }
        
        rewardCurrencyUI.Initialize(new Currency(reward.coins, reward.crystals, reward.currencyType));
    }
}
