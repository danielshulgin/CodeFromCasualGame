using UnityEngine;

public class AndroidVibrator
{
    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject currentActivity;
    public AndroidJavaObject sysService;
    
    public AndroidVibrator()
    {
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        sysService = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    }

    //Functions from https://developer.android.com/reference/android/os/Vibrator.html
    public void SingleVibration()
    {
        sysService.Call("vibrate");
    }


    public void Vibrate(int milliseconds)
    {
        sysService.Call("vibrate", (long)milliseconds);
    }

    public void Vibrate(long[] pattern, int repeat)
    {
        sysService.Call("vibrate", pattern, repeat);
    }


    public void Cancel()
    {
        sysService.Call("cancel");
    }

    public bool HasVibrator()
    {
        return sysService.Call<bool>("hasVibrator");
    }
}
