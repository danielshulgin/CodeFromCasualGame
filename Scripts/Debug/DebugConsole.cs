using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class DebugConsole : MonoBehaviour
{
    [SerializeField] private Text ConsoleText;
    
    public static DebugConsole Instance { get; private set; }

    private string _text;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
            Debug.Log("Multiple DebugConsole instances");
        }
        
    }

    public void Log(string message)
    {
        _text += "\n" + message;
        if (ConsoleText != null)
        {
            ConsoleText.text = _text;
        }
    }
}
