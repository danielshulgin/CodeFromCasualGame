using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;


public class StoresUIDependencies : MonoBehaviour
{
    [Inject] public ScriptableObjectDataBase scriptableObjectDataBase;
    
    public StoreSelectorUIState storeSelectorUIState;
    
    public DefaultPlayerSettings defaultPlayerSettings;

    public GameObject tabParent;
    
    public GameObject tabPrefab;
    
    public GameObject IAPtabPrefab;
    
    public GameObject tabsSeparatorPrefab;

    public CanvasGroup coinsCrystalsCanvasGroup;
    
    public Button exitButton;
    
    public Button buyButton;
    
    public Button coinsCrystalsButton;

    public Button equipButton;

    public Button editButton;
    
    [Inject] public ConfirmPanel confirmPanel;//TODO

    public Image selectedImage;
    
    public Image raysImage;
    
    public Image soilImage;
    
    public Image itemsBackGroundImage;
    
    public GameObject comingSoonPanel;

    public GameObject characteristicsPanel;
    
    public TextMeshProUGUI characteristicsText;
}
