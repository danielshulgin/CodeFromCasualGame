using System;
using Data.Identification;
using UnityEngine;
using Zenject;


public class LoginManager : MonoBehaviour
{
    private ILoginStrategy _loginStrategy;
    
    public static LoginManager Instance { get; private set; }

    [Inject] private PlayerData _playerData; 
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(Instance);
            Instance = this;
            Debug.Log("Multiple LoginManager instances");
        }
    }

    private void Start()
    {
        if (PlayerSettings.Instance.FirstLogin)
        {
            _loginStrategy = new FirstLoginStrategy();
        }
        else if (PlayerSettings.Instance.SignInRejected)
        {
            _loginStrategy = new UnLoggedStrategy();
        }
        else
        {
            _loginStrategy = new SecondLoginStrategy();
        }
        _loginStrategy.Execute(_playerData);
    }

    private void OnDestroy()
    {
        _loginStrategy.UnSubscribe();
    }

    public void StartLateSignInStrategy(Action<LateSignInResult, PlayerDAO> callback)
    {
        var lateSignInStrategy = new LateSignInStrategy();
        _loginStrategy = lateSignInStrategy;
        lateSignInStrategy.OnResult += callback;
        lateSignInStrategy.Execute(_playerData);
    }

    public void RewriteRemotePlayerData()
    {
        RemoteFileStorage.Instance.Save(LocalFileStorage.Instance.Load());
        PlayerSettings.Instance.FirstDeviceFirebaseSaveHappened = true;
        PlayerSettings.Instance.SignInRejected = false;
    }

    public void RewriteLocalPlayerData(PlayerDAO playerData)
    {
        LocalFileStorage.Instance.Save(playerData);
        _playerData.LoadPlayerData();
        PlayerSettings.Instance.FirstDeviceFirebaseSaveHappened = true;
        PlayerSettings.Instance.SignInRejected = false;
        SceneLoader.Instance.LoadStartScene();
    }
}

public enum LateSignInResult
{
    UserHaveAccountWithData, UserHaveEmptyAccount, Fault 
}

