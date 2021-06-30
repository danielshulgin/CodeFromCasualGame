using System;
using UnityEngine;

namespace Data.Identification
{
    public class SecondLoginStrategy : ILoginStrategy
    {
        private PlayerData _playerData;
        
        
        public void Execute(PlayerData playerData)
        {
            _playerData = playerData;
            DebugConsole.Instance.Log("Execute: SecondLoginStrategy");
            Debug.Log("OK2");
            if (PlayerSettings.Instance.FirstDeviceFirebaseSaveHappened)
            {
                GooglePlayServices.OnSignInFault += HandleSignInFault;
                GooglePlayServices.OnSignInSuccess += HandleSignInSuccess;
                
                RemoteFileStorage.OnRightDeviceIdentifier += HandleRightDeviceIdentifier;
                RemoteFileStorage.OnWrongDeviceIdentifier += HandleWrongDeviceIdentifier;

                GooglePlayServices.Instance.AuthenticateUser();
            }
            else
            {
                SceneLoader.Instance.LoadMainSceneAsync();
            }
        }

        private void HandleSignInSuccess()
        {
            DebugConsole.Instance.Log("SecondLoginStrategy: HandleSignInSuccess");
            RemoteFileStorage.Instance.AuthUser();
            RemoteFileStorage.Instance.CheckDeviceIdAsync();
        }

        private void HandleSignInFault()
        {
            DebugConsole.Instance.Log("SecondLoginStrategy: HandleSignInFault");
            UnSubscribe();
            SceneLoader.Instance.LoadMainSceneAsync();
        }

        private void HandleRightDeviceIdentifier()
        {
            DebugConsole.Instance.Log("SecondLoginStrategy: HandleRightDeviceIdentifier");
            UnSubscribe();
            SceneLoader.Instance.LoadMainSceneAsync();
        }

        private void HandleWrongDeviceIdentifier()
        {
            DebugConsole.Instance.Log("SecondLoginStrategy: HandleWrongDeviceIdentifier");
            PlayerSettings.Instance.UserId = "";
            PlayerSettings.Instance.FirstDeviceFirebaseSaveHappened = false;
            LocalFileStorage.Instance.Save(null);
            UnSubscribe();
            SceneLoader.Instance.LoadStartScene();
        }


        public void UnSubscribe()
        {
            GooglePlayServices.OnSignInFault -= HandleSignInFault;
            GooglePlayServices.OnSignInSuccess -= HandleSignInSuccess;

            RemoteFileStorage.OnRightDeviceIdentifier -= HandleRightDeviceIdentifier;
            RemoteFileStorage.OnWrongDeviceIdentifier -= HandleWrongDeviceIdentifier;
        }
    }
}
