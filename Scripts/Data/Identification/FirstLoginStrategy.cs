using System;
using UnityEngine;

namespace Data.Identification
{
    public class FirstLoginStrategy : ILoginStrategy
    {
        private PlayerData _playerData;
        
        public void Execute(PlayerData playerData)
        {
            _playerData = playerData;
            Debug.Log("OK1");
            DebugConsole.Instance.Log("Execute: FirstLoginStrategy");
            ResetToDefaults();
            PlayerSettings.Instance.FirstLogin = false;
            
            LocalFileStorage.Instance.Save(null);

            GooglePlayServices.OnSignInFault += HandleSignInFault;
            GooglePlayServices.OnSignInSuccess += HandleSignInSuccess;
            
            RemoteFileStorage.OnAuthFault += HandleAuthFault;
            RemoteFileStorage.OnAuthSuccess += HandleAuthSuccess;

            RemoteFileStorage.OnLoadPlayerData += HandleLoadPlayerDataSuccess;
            RemoteFileStorage.OnEmptyPlayerData += HandleEmptyPlayerData;
            RemoteFileStorage.OnLoadPlayerDataFault += HandleLoadPlayerDataFault;

            GooglePlayServices.Instance.SignOutUser();
            GooglePlayServices.Instance.AuthenticateUser();
        }

        private void ResetToDefaults()
        {
            PlayerSettings.Instance.SignInRejected = false;
            PlayerSettings.Instance.FirstDeviceFirebaseSaveHappened = false;
            PlayerSettings.Instance.UserId = "";
        }

        private void HandleSignInFault()
        {
            DebugConsole.Instance.Log("FirstLoginStrategy: HandleSignInFault");
            PlayerSettings.Instance.SignInRejected = true;
            UnSubscribe();
            SceneLoader.Instance.LoadMainSceneAsync();
        }

        private void HandleSignInSuccess()
        {
            DebugConsole.Instance.Log("FirstLoginStrategy: HandleSignInSuccess");
            RemoteFileStorage.Instance.AuthUser();
        }
        
        private void HandleAuthFault()
        {
            DebugConsole.Instance.Log("FirstLoginStrategy: HandleAuthFault");
            UnSubscribe();
            SceneLoader.Instance.LoadMainSceneAsync();
        }

        private void HandleAuthSuccess()
        {
            DebugConsole.Instance.Log("FirstLoginStrategy: HandleAuthSuccess");
            
            RemoteFileStorage.Instance.LoadPlayerDataAsync();
        }
        
        private void HandleLoadPlayerDataSuccess(PlayerDAO player)
        {
            DebugConsole.Instance.Log("FirstLoginStrategy: HandleLoadPlayerDataSuccess");
            
            LocalFileStorage.Instance.Save(player);
            PlayerSettings.Instance.FirstDeviceFirebaseSaveHappened = true;
            SceneLoader.Instance.LoadMainSceneAsync();
            _playerData.LoadPlayerData();
        }

        private void HandleEmptyPlayerData()
        {
            DebugConsole.Instance.Log("FirstLoginStrategy: HandleEmptyPlayerData");
            
            LocalFileStorage.Instance.Save(null);
            RemoteFileStorage.Instance.Save(LocalFileStorage.Instance.Load());
            _playerData.LoadPlayerData();
            PlayerSettings.Instance.FirstDeviceFirebaseSaveHappened = true;
            UnSubscribe();
            SceneLoader.Instance.LoadMainSceneAsync();
        }
        
        private void HandleLoadPlayerDataFault()
        {
            DebugConsole.Instance.Log("FirstLoginStrategy: HandleLoadPlayerDataFault");
            _playerData.LoadPlayerData();
            UnSubscribe();
            SceneLoader.Instance.LoadMainSceneAsync();
        }

        public void UnSubscribe()
        {
            GooglePlayServices.OnSignInFault -= HandleSignInFault;
            GooglePlayServices.OnSignInSuccess -= HandleSignInSuccess;
            
            RemoteFileStorage.OnAuthFault -= HandleAuthFault;
            RemoteFileStorage.OnAuthSuccess -= HandleAuthSuccess;
            
            RemoteFileStorage.OnLoadPlayerData -= HandleLoadPlayerDataSuccess;
            RemoteFileStorage.OnEmptyPlayerData -= HandleEmptyPlayerData;
            RemoteFileStorage.OnLoadPlayerDataFault -= HandleLoadPlayerDataFault;

            RemoteFileStorage.OnLoadPlayerDataFault -= HandleLoadPlayerDataFault;
        }
    }
}
