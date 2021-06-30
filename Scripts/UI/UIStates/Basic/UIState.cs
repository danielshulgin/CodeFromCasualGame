using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIState : MonoBehaviour
{
    public UnityEvent OnEnter;
    
    public UnityEvent OnExit;
    
    [Inject] protected UIStateMachine _uiStateMachine;
    
    protected CanvasGroup _canvasGroup;
    

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void Start()
    {
       // _canvasGroup = GetComponent<CanvasGroup>();
    }
    
    protected static void SetActiveCanvasGroup(CanvasGroup canvasGroup, bool active)
    {
        UIHelperFunctions.SetActiveCanvasGroup(canvasGroup, active);
    }

    public abstract void Enter();
    
    public abstract void Exit();

    public virtual void UpdateState() { }

    public virtual void ExitButtonClick()
    {
    }
}
