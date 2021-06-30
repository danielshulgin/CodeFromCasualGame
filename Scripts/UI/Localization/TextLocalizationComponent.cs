using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalizationComponent : MonoBehaviour
{
    [SerializeField] private string defaultString = "";
    
    private TextMeshProUGUI _text;
    

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Localization.Instance.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage();
    }

    private void UpdateLanguage()
    {
        if (!string.IsNullOrEmpty(defaultString))
            _text.text = Localization.Instance.GetTranslation(defaultString);
        else
            Debug.Log($"The string of text object with name: {name}, defaultString: {defaultString} is empty");
    }

    [ContextMenu("GetDefaultString")]
    private void GetDefaultString()
    {
        _text.text = defaultString;
    }
    
}
