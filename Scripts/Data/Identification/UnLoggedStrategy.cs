using UnityEngine;

namespace Data.Identification
{
    public class UnLoggedStrategy : ILoginStrategy
    {
        private PlayerData _playerData;
        
        
        public void Execute(PlayerData playerData)
        {
            _playerData = playerData;
            DebugConsole.Instance.Log("Execute: UnLoggedStrategy");
            Debug.Log("OK3");
            SceneLoader.Instance.LoadMainSceneAsync();
        }

        public void UnSubscribe()
        {
            
        }
    }
}