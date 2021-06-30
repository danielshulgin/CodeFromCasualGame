using System;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;

    [SerializeField] private AudioMixer audioMixer;
    
    private bool _gameAudio;
    
    public bool GameAudio
    {
        get => _gameAudio;
        set
        {
            _gameAudio = value;
            PlayerPrefs.SetInt("GameSounds", value ? 1 : 0);
            audioMixer.SetFloat("GameVolume", _gameAudio ? 0f : -80f);
        }
    }
    
    private bool _UISounds;
    
    public bool UISounds
    {
        get => _UISounds;
        set
        {
            _UISounds = value;
            PlayerPrefs.SetInt("UISounds", value ? 1 : 0);
            audioMixer.SetFloat("UIVolume", value ? 0f : -80f);
        }
    }
    
    private bool _musicSounds;
    
    public bool MusicSounds
    {
        get => _musicSounds;
        set
        {
            _musicSounds = value;
            PlayerPrefs.SetInt("MusicSounds", value ? 1 : 0);
            audioMixer.SetFloat("MusicVolume", value ? 0f : -80f);
        }
    }
    
    private bool _physicVibration;
    
    public bool PhysicVibration
    {
        get => _physicVibration;
        set
        {
            _physicVibration = value;
            PlayerPrefs.SetInt("PhysicVibration", value ? 1 : 0);
        }
    }
    
    private bool _blood;
    
    public bool Blood
    {
        get => _blood;
        set
        {
            _blood = value;
            PlayerPrefs.SetInt("Blood", value ? 1 : 0);
        }
    }
    
    private bool _sight;
    
    public bool Sight
    {
        get => _sight;
        set
        {
            _sight = value;
            PlayerPrefs.SetInt("Sight", value ? 1 : 0);
        }
    }
    
    private bool _firstLogin;
    
    public bool FirstLogin
    {
        get => _firstLogin;
        set
        {
            _firstLogin = value;
            PlayerPrefs.SetInt("FirstLogin", value ? 1 : 0);
        }
    }
    
    private bool _signInRejected;
    
    public bool SignInRejected
    {
        get => _signInRejected;
        set
        {
            _signInRejected = value;
            PlayerPrefs.SetInt("SignInRejected", value ? 1 : 0);
        }
    }

    private bool _firstDeviceFirebaseSaveHappened;
    
    public bool FirstDeviceFirebaseSaveHappened
    {
        get => _firstDeviceFirebaseSaveHappened;
        set
        {
            _firstDeviceFirebaseSaveHappened = value;
            PlayerPrefs.SetInt("FirstDeviceFirebaseSaveHappened", value ? 1 : 0);
        }
    }
    
    private string _userId;
    
    public string UserId
    {
        get => _userId;
        set
        {
            _userId = value;
            PlayerPrefs.SetString("UserId", value);
        }
    }

    
    public static PlayerSettings Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            Debug.Log("Multiple PlayerSettings instances");
            return;
        }

        _gameAudio = PlayerPrefs.GetInt("GameSounds", defaultPlayerSettings.gameSounds ? 1 : 0) == 1;

        _UISounds = PlayerPrefs.GetInt("UISounds", defaultPlayerSettings.UISounds ? 1 : 0) == 1;

        _musicSounds = PlayerPrefs.GetInt("MusicSounds", defaultPlayerSettings.musicSounds ? 1 : 0) == 1;

        _physicVibration = PlayerPrefs.GetInt("PhysicVibration", defaultPlayerSettings.physicVibration ? 1 : 0) == 1;
        
        _blood = PlayerPrefs.GetInt("Blood", defaultPlayerSettings.blood ? 1 : 0) == 1;
        
        _sight = PlayerPrefs.GetInt("Sight", defaultPlayerSettings.sight ? 1 : 0) == 1;
        
        _firstLogin = PlayerPrefs.GetInt("FirstLogin", 1) == 1;
        
        _signInRejected = PlayerPrefs.GetInt("SignInRejected", 0) == 1;

        _firstDeviceFirebaseSaveHappened = PlayerPrefs.GetInt("FirstDeviceFirebaseSaveHappened", 0) == 1;
        
        _userId = PlayerPrefs.GetString("UserId", "");
    }

    private void Start()
    {
        audioMixer.SetFloat("GameVolume", _gameAudio ? 0f : -80f);
        
        audioMixer.SetFloat("UIVolume", _UISounds ? 0f : -80f);
        
        audioMixer.SetFloat("MusicVolume", _musicSounds ? 0f : -80f);
    }
}
