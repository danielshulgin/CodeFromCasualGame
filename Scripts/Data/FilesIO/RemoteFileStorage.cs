using System;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using GooglePlayGames;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class RemoteFileStorage : MonoBehaviour
{
    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;

    private DatabaseReference reference;
    
    public static event Action OnAuthSuccess;
    
    public static event Action OnAuthFault;

    public static event Action<PlayerDAO> OnLoadPlayerData;
    
    public static event Action OnEmptyPlayerData;
    
    public static event Action OnLoadPlayerDataFault;

    public static event Action OnWrongDeviceIdentifier;
    
    public static event Action OnRightDeviceIdentifier; 
    
    public static event Action OnFailCheckDeviceIdentifier; 
    
    public static RemoteFileStorage Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            Debug.Log("Multiple RemoteFileStorage instances");
            return;
        }

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://************************");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Save(PlayerDAO playerData)
    {
        if (string.IsNullOrEmpty(PlayerSettings.Instance.UserId))
        {
            return;
        }
        
        var json = JsonConvert.SerializeObject(playerData);
        reference.Child("users").Child(PlayerSettings.Instance.UserId).SetRawJsonValueAsync(json);
        PlayerPrefs.SetInt("FirstDeviceFirebaseSaveHappened", 1);
    }

    public void AuthUser()
    {
        var authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
        if (authCode == null)
        {
            OnAuthFault?.Invoke();
            return;
        }
        if (authCode == "")
        {
            authCode = PlayerPrefs.GetString("AuthCode", "");
        }
        PlayerPrefs.SetString("AuthCode", authCode);
        
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.Credential credential =
            Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            DebugConsole.Instance.Log("auth");
            if (task.IsCanceled) {
                DebugConsole.Instance.Log("IsCanceled");
                OnAuthFault?.Invoke();
                return;
            }
            if (task.IsFaulted) {
                DebugConsole.Instance.Log("IsFaulted");
                OnAuthFault?.Invoke();
                return;
            }
            
            DebugConsole.Instance.Log("task.Result");
            Firebase.Auth.FirebaseUser user = task.Result;
            
            if (string.IsNullOrEmpty(user.UserId))
            {
                OnAuthFault?.Invoke();
                return;
            }
            
            PlayerSettings.Instance.UserId = user.UserId;
            DebugConsole.Instance.Log("OnAuthSuccess");
            OnAuthSuccess?.Invoke();
        });
    }

    public void LoadPlayerDataAsync()
    {
        DebugConsole.Instance.Log("LoadPlayerDataAsync");
        if (PlayerSettings.Instance.UserId.Length == 0)
        {
            DebugConsole.Instance.Log("OnLoadPlayerDataFault");
            OnLoadPlayerDataFault?.Invoke();
            return;
        }
        reference.Child("users").Child(PlayerSettings.Instance.UserId).GetValueAsync().ContinueWith(task => {
            DebugConsole.Instance.Log("task");
            if (task.IsFaulted) {
                DebugConsole.Instance.Log("task.IsFaulted");
                OnLoadPlayerDataFault?.Invoke();
            }
            else if (task.IsCompleted) {
                DebugConsole.Instance.Log("task.IsCompleted");
                DataSnapshot snapshot = task.Result;
                var rawJsonValue = snapshot.GetRawJsonValue();
                if (rawJsonValue == null)
                {
                    OnEmptyPlayerData?.Invoke();
                    return;
                }
                var playerData = JsonConvert.DeserializeObject<PlayerDAO>(rawJsonValue);
                OnLoadPlayerData?.Invoke(playerData);
            }
        });
    }

    public void CheckDeviceIdAsync()
    {
        DebugConsole.Instance.Log("CheckDeviceIdAsync");
        reference.Child("users").Child(PlayerSettings.Instance.UserId).Child("deviceId").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                OnFailCheckDeviceIdentifier?.Invoke();
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                var deviceId = (string)snapshot.GetValue(true);
                if (PlayerSettings.Instance.UserId != "" && deviceId != SystemInfo.deviceUniqueIdentifier)
                {
                    OnWrongDeviceIdentifier?.Invoke();
                }

                OnRightDeviceIdentifier?.Invoke();
            }
        });
    }
}
