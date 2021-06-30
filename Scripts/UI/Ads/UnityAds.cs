using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class UnityAds : MonoBehaviour, IUnityAdsListener
{
    [FormerlySerializedAs("OnSuccesEndWatchEvent")] 
    [FormerlySerializedAs("OnEndWatchEvent")] 
    public UnityEvent OnSuccessEndWatchEvent;

    public UnityEvent OnUnityAdsReadyEvent;
    
#if UNITY_IOS
   private string gameId = "*****";
#elif UNITY_ANDROID
    private string gameId = "*****";
#endif

    public string myPlacementId = "rewardedVideo";

    public bool Ready => Advertisement.IsReady(myPlacementId);


    private void Start () {
        Advertisement.AddListener (this);
        Advertisement.Initialize (gameId, true);
    }
    
    public void ShowRewardedVideo () {
        Advertisement.Show(myPlacementId);
    }

    public void OnUnityAdsReady (string placementId) {
        if (placementId == myPlacementId) {        
            OnUnityAdsReadyEvent.Invoke();
        }
    }

    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished) {
            OnSuccessEndWatchEvent.Invoke();
        } else if (showResult == ShowResult.Skipped) {
        } else if (showResult == ShowResult.Failed) {
            Debug.LogWarning ("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidError (string message) 
    {
    }

    public void OnUnityAdsDidStart (string placementId)
    {
    }
}