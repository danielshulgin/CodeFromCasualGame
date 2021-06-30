using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;

public class Localization : MonoBehaviour
{
    public Action OnLanguageChanged;
    
    [SerializeField] private string fileName = "LocalizationDictionary.csv";
    
    [SerializeField] private char columnSeparator = ';';
    
    private char _rowSeparator = '\n';
    
    private string _currentLanguage = "English";

    private Dictionary<string, string> _translations;
    
    public string[] Languages { get; private set; }
    
    public string[] NativeLanguagesNames { get; private set; }
    
    private ReadOnlyDictionary<string, string> Translations
    {
        get
        {
            if (_translations == null)
                UpdateLocalizationFile();

            return new ReadOnlyDictionary<string, string>(_translations);
        }
    }

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (value != _currentLanguage)
            {
                _currentLanguage = value;
                UpdateLocalizationFile();
                OnLanguageChanged?.Invoke();
            }
        }
    }

    public static Localization Instance { get; private set; }

    
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Multiple Localization instances");
        }
        
        InitializeLanguageArrays();
    }

    public void ChangeLanguage(int languageIndex)
    {
        CurrentLanguage = Languages[languageIndex];
    } 

    private void InitializeLanguageArrays()
    {
        var textFile = Resources.Load<TextAsset>(fileName);
        Languages = textFile.text.Split(_rowSeparator)
            .First()
            .Split(columnSeparator)
            .Skip(1)//skip default id
            .ToArray();
        
        NativeLanguagesNames = textFile.text.Split(_rowSeparator)[1]
            .Split(columnSeparator)
            .Skip(1)//skip default id
            .ToArray();
    }
    
    private void UpdateLocalizationFile()
    {
        var textFile = Resources.Load<TextAsset>(fileName);
        var lines = textFile.text.Split(_rowSeparator);
        
        _translations = new Dictionary<string, string>(lines.Length- 1);

        var languageColumnIndex = GetLanguageColumnIndex();
        
        var linesLength = lines.Length;
        
        if (lines[lines.Length - 1].Length == 0)
        {
            linesLength -= 1;
        }
        for (int i = 1; i < linesLength; i++)
        {
            var stringTranslations = lines[i].TrimEnd('\r', '\n').Split(columnSeparator);
            _translations[stringTranslations[0]] = stringTranslations[languageColumnIndex];
        }
    }

    private int GetLanguageColumnIndex()
    {
        return Array.IndexOf(Languages, _currentLanguage) + 1;//skip 0 column with id
    }
    
    public string GetTranslation(string id)
    {
        if (Translations.ContainsKey(id))
            return Translations[id];
        else
        {
            Debug.Log($"Translations does not contain a string with {id} for {_currentLanguage}");
            return "empty";
        }
    }
}
