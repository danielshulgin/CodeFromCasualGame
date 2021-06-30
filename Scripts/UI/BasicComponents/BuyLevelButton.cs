using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;


public class BuyLevelButton : MonoBehaviour
{
    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;

    [SerializeField] private RectTransform buttonsPivotRectTransform;
    
    [SerializeField] private GameObject middleButtonGameObject;
    
    [SerializeField] private GameObject leftButtonGameObject;
    
    [SerializeField] private GameObject rightButtonGameObject;
    
    [SerializeField] private GameObject middleComingSoonGameObject;
    
    [SerializeField] private GameObject leftComingSoonGameObject;
    
    [SerializeField] private GameObject rightComingSoonGameObject;
    
    [SerializeField] private GameObject middleBlurPanel;
    
    [SerializeField] private GameObject leftBlurPanel;
    
    [SerializeField] private GameObject rightBlurPanel;

    [SerializeField] private PriceUI middleButtonPrice;
    
    [SerializeField] private PriceUI leftButtonPrice;
    
    [SerializeField] private PriceUI rightButtonPrice;

    [SerializeField] private RectTransform canvasRectTransform;

    [Inject] private PlayerLevels _playerLevels;

    private RectTransform _leftButtonTransform;
    
    private RectTransform _rightButtonTransform;
    
    private RectTransform _middleBlurPanelRect;
    
    private RectTransform _leftBlurPanelRect;
    
    private RectTransform _rightBlurPanelRect; 
    

    private void Awake()
    {
        _leftButtonTransform = leftButtonGameObject.GetComponent<RectTransform>();
        _rightButtonTransform = rightButtonGameObject.GetComponent<RectTransform>();

        _middleBlurPanelRect = middleBlurPanel.GetComponent<RectTransform>();
        _leftBlurPanelRect = leftBlurPanel.GetComponent<RectTransform>();
        _rightBlurPanelRect = rightBlurPanel.GetComponent<RectTransform>();
        
        LevelSwitcher.OnEndSwitch += UpdateButtonsState;
        LevelSwitcher.OnStartSwitch += HandleStartSwitchLevel;
        LevelSwitcher.OnUpdateRelativePosition += UpdatePositions;
        
        PlayerLevels.OnBuyLevel += UpdateButtonsState;
    }

    private void OnDestroy()
    {
        LevelSwitcher.OnEndSwitch -= UpdateButtonsState;
        LevelSwitcher.OnStartSwitch -= HandleStartSwitchLevel;
        LevelSwitcher.OnUpdateRelativePosition -= UpdatePositions;
        
        PlayerLevels.OnBuyLevel -= UpdateButtonsState;
    }

    private void Start()
    {
        var screenWidth = canvasRectTransform.rect.width;

        _leftButtonTransform.anchoredPosition = new Vector2(-screenWidth, 0f);
        _rightButtonTransform.anchoredPosition = new Vector2( screenWidth, 0f);
        
        _leftBlurPanelRect.anchoredPosition = new Vector2(-screenWidth, 0f);
        _rightBlurPanelRect.anchoredPosition = new Vector2( screenWidth, 0f);

        
        _leftBlurPanelRect.sizeDelta = canvasRectTransform.sizeDelta; 
        _rightBlurPanelRect.sizeDelta = canvasRectTransform.sizeDelta; 
        _middleBlurPanelRect.sizeDelta = canvasRectTransform.sizeDelta;
        
        UpdateButtonsState(_playerLevels.CurrentLevel);
    }

    private void UpdateButtonsState(LevelScriptableObject levelScriptableObject)
    {
        UpdateCurrentLevelObjects(levelScriptableObject);
        
        buttonsPivotRectTransform.anchoredPosition = 
            new Vector2(0f, 0f);
    }

    private void HandleStartSwitchLevel(LevelScriptableObject nextLevelScriptableObject)
    {
        UpdateCurrentLevelObjects(_playerLevels.CurrentLevel);
        UpdateNextLevelObjects(nextLevelScriptableObject);
    }
    
    private void UpdateCurrentLevelObjects(LevelScriptableObject currentLevelScriptableObject)
    {
        var currentLevelNotBought = !_playerLevels.LevelBought(currentLevelScriptableObject);
        middleButtonGameObject.SetActive(currentLevelNotBought);
        middleBlurPanel.SetActive(currentLevelNotBought);
        middleButtonPrice.Initialize(currentLevelScriptableObject.price);

        var currentLevelComingSoon = defaultPlayerSettings.comingSoon.Contains(currentLevelScriptableObject);
        middleComingSoonGameObject.SetActive(currentLevelComingSoon);
    }

    private void UpdateNextLevelObjects(LevelScriptableObject nextLevelScriptableObject)
    {
        var nextLevelNotBought = !_playerLevels.LevelBought(nextLevelScriptableObject);
        rightButtonPrice.Initialize(nextLevelScriptableObject.price);
        leftButtonPrice.Initialize(nextLevelScriptableObject.price);
        
        leftBlurPanel.SetActive(nextLevelNotBought);
        rightBlurPanel.SetActive(nextLevelNotBought);
        
        leftButtonGameObject.SetActive(nextLevelNotBought);
        rightButtonGameObject.SetActive(nextLevelNotBought);

        var nextLevelComingSoon = defaultPlayerSettings.comingSoon.Contains(nextLevelScriptableObject);
        leftComingSoonGameObject.SetActive(nextLevelComingSoon);
        rightComingSoonGameObject.SetActive(nextLevelComingSoon);
    }
    
    private void UpdatePositions(float x)
    {
        buttonsPivotRectTransform.anchoredPosition = 
            new Vector2(x * canvasRectTransform.sizeDelta.x, buttonsPivotRectTransform.anchoredPosition.y);
    }
}
