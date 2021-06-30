using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StoreSelectorUIState : UIState
{
    [SerializeField] private List<StoreInfo> storeInfos;

    private UIState _currentState;

    private StoreTabUI _currentTabUI;

    
    protected override void Awake()
    {
        base.Awake();
        foreach (var storeInfo in storeInfos.Where(storeInfo => storeInfo.storeType != StoreType.BulletSkin))
        {
            storeInfo.storeTabUI.OnClick += HandleTabClick;
        }
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        Destroy(gameObject);
    }
    
    private void ChangeState(UIState uiState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
            _currentState.OnExit.Invoke();
        }
        _currentState = uiState;
        _currentState.Enter();
        _currentState.OnEnter.Invoke();
    }
    
    public void OpenStore(StoreType storeType)
    {
        var currentStoreInfo = storeInfos.Find(storeInfo => storeInfo.storeType == storeType);
        
        currentStoreInfo.storeTabUI.SetSprite(true);
        _currentTabUI = currentStoreInfo.storeTabUI;
        
        ChangeState(currentStoreInfo.storeUIState);
    }
    
    private void HandleTabClick(StoreTabUI tabUi)
    {
        if (_currentTabUI != null)
        {
            _currentTabUI.SetSprite(false);
        }
        _currentTabUI = tabUi;
        
        var currentStoreInfo = storeInfos.Find(storeInfo => storeInfo.storeType == tabUi.StoreType);
        ChangeState(currentStoreInfo.storeUIState);
    }
}
