using System;
using UnityEngine;

public class RewardSender : MonoBehaviour
{
    public static event Action<Currency> OnReward;

    [SerializeField] private Currency currency;
    
    
    public void Send()
    {
        OnReward?.Invoke(currency);
    }    
}
