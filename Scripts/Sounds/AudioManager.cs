using System.Collections.Generic;
using Malee;
using MyBox;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Reorderable] public SoundReorderableArray sounds;

    private Dictionary<string, Sound> _soundsDictionary;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Multiple AudioManager instances");
        }
        
        _soundsDictionary = new Dictionary<string, Sound>();
        
        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            var soundName = sound.name;
            if (soundName== "")
            {
                soundName = sound.clip.name;
            }
            _soundsDictionary[sound.name] = sound;
            sound.source.outputAudioMixerGroup = sound.audioMixerGroup;
        }
    }

    public void Play(string soundName)
    {
        Play(soundName, 1f);
    }
    
    /// <param name="volume">0 1range</param>
    public void Play(string soundName, float volume)
    {
        if (_soundsDictionary.ContainsKey(soundName))
        {
            if (!_soundsDictionary[soundName].source.isPlaying)
            {
                //volume = HelperFunctions.RangeToRange(volume, 0f, 1f, -80f, 0f);
                _soundsDictionary[soundName].source.volume = volume * _soundsDictionary[soundName].volume;
                _soundsDictionary[soundName].source.Play();
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"No sound with name {soundName}");
        }
#endif
    }
    
    public void PlayWithOverlay(string soundName)
    {
        PlayWithOverlay(soundName, 1f);
    }
    
    public void PlayWithOverlay(string soundName, float volume)
    {
        if (_soundsDictionary.ContainsKey(soundName))
        {
            _soundsDictionary[soundName].source.volume = volume * _soundsDictionary[soundName].volume;
            _soundsDictionary[soundName].source.Play();
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"No sound with name {soundName}");
        }
#endif
    }
    
    public void Pause(string soundName)
    {
        if (_soundsDictionary.ContainsKey(soundName))
        {
            _soundsDictionary[soundName].source.Pause();
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"No sound with name {soundName}");
        }
#endif
    }
    
    public void UnPause(string soundName)
    {
        if (_soundsDictionary.ContainsKey(soundName))
        {
            _soundsDictionary[soundName].source.UnPause();
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"No sound with name {soundName}");
        }
#endif
    }

    public void Stop(string soundName)
    {
        if (_soundsDictionary.ContainsKey(soundName))
        {
            _soundsDictionary[soundName].source.Stop();
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"No sound with name {soundName}");
        }
#endif
    }
    
#if UNITY_EDITOR
    [SerializeField] private string DEBUG_soundName;

    [ButtonMethod]
    private void DEBUG_Play()
    {
        AudioManager.Instance.PlayWithOverlay(DEBUG_soundName);
    }
    
    [ButtonMethod]
    private void DEBUG_Stop()
    {
        AudioManager.Instance.Stop(DEBUG_soundName);
    }
#endif
    
    [System.Serializable]
    public class SoundReorderableArray : ReorderableArray<Sound> {
    }
}
