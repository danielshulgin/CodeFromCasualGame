using System;
using System.Dynamic;

public class LateSignInStrategy : ILoginStrategy
{
    public event Action<LateSignInResult, PlayerDAO> OnResult;
    
    private PlayerData _playerData;
    
    
    public void Execute(PlayerData playerData)
    {
        _playerData = playerData;
        DebugConsole.Instance.Log("Execute: LateSignInStrategy");
        GooglePlayServices.OnSignInFault += HandleSignInFault;
        GooglePlayServices.OnSignInSuccess += HandleSignInSuccess;
        
        RemoteFileStorage.OnAuthFault += HandleAuthFault;
        RemoteFileStorage.OnAuthSuccess += HandleAuthSuccess;
        
        RemoteFileStorage.OnLoadPlayerData += HandleLoadPlayerDataSuccess;
        RemoteFileStorage.OnLoadPlayerDataFault += HandleLoadPlayerDataFault;
        RemoteFileStorage.OnEmptyPlayerData += HandleEmptyPlayerData;

        GooglePlayServices.Instance.AuthenticateUser();
    }

    private void HandleSignInFault()
    {
        DebugConsole.Instance.Log("LateSignInStrategy: HandleSignInFault");
        OnResult?.Invoke(LateSignInResult.Fault, null);
        UnSubscribe();
        GooglePlayServices.Instance.SignOutUser();
        Reset();
    }

    private void HandleSignInSuccess()
    {
        DebugConsole.Instance.Log("LateSignInStrategy: HandleSignInSuccess");
        RemoteFileStorage.Instance.AuthUser();
    }
    
    private void HandleAuthFault()
    {
        DebugConsole.Instance.Log("LateSignInStrategy: HandleAuthFault");
        UnSubscribe();
        GooglePlayServices.Instance.SignOutUser();
        OnResult?.Invoke(LateSignInResult.Fault, null);
        Reset();
    }

    private void HandleAuthSuccess()
    {
        DebugConsole.Instance.Log("LateSignInStrategy: HandleAuthSuccess");
        RemoteFileStorage.Instance.LoadPlayerDataAsync();
    }
    
    private void HandleLoadPlayerDataSuccess(PlayerDAO player)
    {
        DebugConsole.Instance.Log("LateSignInStrategy: HandleLoadPlayerDataSuccess");
        UnSubscribe();
        OnResult?.Invoke(LateSignInResult.UserHaveAccountWithData, player);
        Reset();
    }

    private void HandleEmptyPlayerData()
    {
        DebugConsole.Instance.Log("LateSignInStrategy: HandleEmptyPlayerData");
            
        OnResult?.Invoke(LateSignInResult.UserHaveEmptyAccount, null);
        UnSubscribe();
        Reset();
    }
        
    private void HandleLoadPlayerDataFault()
    {
        DebugConsole.Instance.Log("LateSignInStrategy: HandleLoadPlayerDataFault");
        PlayerSettings.Instance.UserId = "";
        UnSubscribe();
        GooglePlayServices.Instance.SignOutUser();
        OnResult?.Invoke(LateSignInResult.Fault, null);
        Reset();
    }


    public void UnSubscribe()
    {
        GooglePlayServices.OnSignInFault -= HandleSignInFault;
        GooglePlayServices.OnSignInSuccess -= HandleSignInSuccess;
        
        RemoteFileStorage.OnAuthFault -= HandleAuthFault;
        RemoteFileStorage.OnAuthSuccess -= HandleAuthSuccess;
        
        RemoteFileStorage.OnLoadPlayerData -= HandleLoadPlayerDataSuccess;
        RemoteFileStorage.OnLoadPlayerDataFault -= HandleLoadPlayerDataFault;
        RemoteFileStorage.OnEmptyPlayerData -= HandleEmptyPlayerData;
    }

    public void Reset()
    {
        OnResult = null;
    }
}
