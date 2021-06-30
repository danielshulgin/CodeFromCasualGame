using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UnityAdsButton : MonoBehaviour
{
    public UnityEvent OnSuccessEndWatchEvent;
    
    [SerializeField] private UnityAds unityAds;
    
    [SerializeField] private Button button;
    

    private void OnEnable()
    {
        unityAds.OnSuccessEndWatchEvent.AddListener(OnSuccessEndWatch);
        TryOn();
    }

    private void OnDisable()
    {
        unityAds.OnSuccessEndWatchEvent.RemoveListener(OnSuccessEndWatch);
    }

    public void ShowRewardedVideo() 
    {
        unityAds.ShowRewardedVideo();
        button.interactable = false;
    }
    
    public void OnUnityAdsReady() 
    {
        button.interactable = unityAds.Ready;
    }
    
    public void OnSuccessEndWatch() 
    {
        button.interactable = false;
        OnSuccessEndWatchEvent.Invoke();
    }
    
    public void TryOn()
    {
        button.interactable = unityAds.Ready;
    }
}
