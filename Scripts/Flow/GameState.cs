using System;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    // |-->{StartPoint}
    // |        |           
    // |        v           
    // |    OnStartGame     
    // |        |           
    // |        v      
    // |    OnStartFly      
    // |        |          
    // |        |<-------OnContinue
    // |        |           ^        
    // |        |           |        
    // |        |-------->OnPause    
    // |        |                   
    // |        v                  
    // |     OnEndFly
    // |        |              
    // |        V              
    // |    OnEndGame-----|              
    // |        |         |
    // |        v         |
    // |   OnShowResults  |
    // |        |         |
    // |        V         |
    // |----OnResetGame<--|
    
    public static event Action OnStartGame;
    
    public static event Action OnStartFly;
    
    public static event Action OnPause;

    public static event Action OnContinue;
    
    public static event Action OnEndFly;
    
    public static event Action OnEndGame;

    public static event Action OnShowResults;
    
    public static event Action OnResetGame;

    private bool _inFly;

    private bool _inGame;
    
    private bool _paused;
    

    public void SendResetGame()
    {
        OnResetGame?.Invoke();
    }

    public void SendStartFly()
    {
        OnStartFly?.Invoke();
        _inFly = true;
    }
    
    public void SendEndFly()
    {
        OnEndFly?.Invoke();
        _inFly = false;
    }

    public void SendStartGame()
    {
        OnStartGame?.Invoke();
        _inGame = true;
    }
    
    public void SendEndGame()
    {
        OnEndGame?.Invoke();
        _inGame = false;
    }
    
    public void SendPause()
    {
        OnPause?.Invoke();
        Time.timeScale = 0f;
        _paused = true;
    }
    
    public void SendContinue()
    {
        OnContinue?.Invoke();
        Time.timeScale = 1f;
        _paused = false;
    }
    
    public void SendShowResults()
    {
        OnShowResults.Invoke();
    }
}
