using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class UIStateMachineInstaller: MonoInstaller
{
    [SerializeField] private UIStateMachine uiStateMachine;
    
    [SerializeField] private ConfirmPanel confirmPanel;

    
    public override void InstallBindings()
    {
        Container.Bind<ConfirmPanel>().FromInstance(confirmPanel);//TODO

        Container.Bind<UIStateMachine>().FromInstance(uiStateMachine);
        
        Container.Bind<DiContainer>().WithId("MainSceneContainer").FromInstance(Container);//TODO
    }
}
