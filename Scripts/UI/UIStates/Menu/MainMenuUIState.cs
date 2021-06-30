using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class MainMenuUIState : UIState
{
    public event Action OnEnter;
    
    public event Action OnExit;

    [SerializeField] private LevelSwitcher levelSwitcher;

    [SerializeField] private ClickManager clickManager;
    
    [SerializeField] private GameState gameState;

    [SerializeField] private AdsAchievementsUI adsAchievementsUi;

    [SerializeField] private CanvasGroup storeButtonCanvasGroup;
    
    [SerializeField] private CanvasGroup coinsCrystalsCanvasGroup;

    [SerializeField] private ConfirmPanel confirmPanel;

    [SerializeField] private GameObject adsButton;
    
    [SerializeField] private GameObject cannonballsStoreButton;
    
    [SerializeField] private GameObject cannonsStoreButton;

    [SerializeField] private float forgetClickOnExitButtonTime = 1.5f;

    [SerializeField] private CanvasGroup doubleClickTextCanvasGroup;

    [SerializeField] private Button trapButton;
        
    [SerializeField] private TextMeshProUGUI cannonBallsNumberText;

    [Inject] private PlayerLevels _playerLevels;
        
    [Inject] private PlayerCannonballs _playerCannonballs;

    private bool _oneClickOnExitButton;

    private bool _levelBought = true;

    private Vector3 _trapsButtonStartPosition;
    
    private Vector3 _middleBottomButtonStartPosition;
    
    private RectTransform _trapsButtonRect;
    
    private RectTransform _middleBottomRect;

    private bool _switchingLevel = false;
    
    
    protected override void Start()
    {
        base.Start();
        _trapsButtonRect = trapButton.GetComponent<RectTransform>();
        _trapsButtonStartPosition = trapButton.transform.position;
    }

    public override void Enter()
    {
        //TODO remove rename ElkClick vasyl
        clickManager.OnStartGameButtonTouch.AddListener(HandleStartGameButtonClick);
        clickManager.OnElkTouch.AddListener(HandleElkClick);
        levelSwitcher.enabled = true;
        
        SetActiveCanvasGroup(storeButtonCanvasGroup, true);
        SetActiveCanvasGroup(coinsCrystalsCanvasGroup, true);
        
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;

        LevelSwitcher.OnStartSwitch += HandleStartSwitchLevel;
        LevelSwitcher.OnEndSwitch += UpdateLevelInfo;
        LevelSwitcher.OnEndSwitch += HandleEndSwitchLevel;
        LevelSwitcher.OnCancelSwitch += HandleCancelSwitchLevel;
        
        adsButton.SetActive(true);
        cannonballsStoreButton.SetActive(true);
        cannonBallsNumberText.text = _playerCannonballs.GetNumberInStack(_playerCannonballs.CurrentCannonball).ToString();
        cannonsStoreButton.SetActive(true);

        OnEnter?.Invoke();
        UpdateLevelInfo(_playerLevels.CurrentLevel);
        UpdateButtonPositions();
    }

    public override void Exit()
    {
        clickManager.OnStartGameButtonTouch.RemoveListener(HandleStartGameButtonClick);
        clickManager.OnElkTouch.RemoveListener(HandleElkClick);
        levelSwitcher.enabled = false;
        
        SetActiveCanvasGroup(storeButtonCanvasGroup, false);
        SetActiveCanvasGroup(coinsCrystalsCanvasGroup, false);
        
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        
        adsButton.SetActive(false);
        cannonballsStoreButton.SetActive(false);
        cannonsStoreButton.SetActive(false);

        LevelSwitcher.OnStartSwitch -= HandleStartSwitchLevel;
        LevelSwitcher.OnEndSwitch -= UpdateLevelInfo;
        LevelSwitcher.OnEndSwitch -= HandleEndSwitchLevel;
        LevelSwitcher.OnCancelSwitch -= HandleCancelSwitchLevel;

        OnExit?.Invoke();
    }

    private void OnDestroy()
    {
        LevelSwitcher.OnStartSwitch -= HandleStartSwitchLevel;
        LevelSwitcher.OnEndSwitch -= UpdateLevelInfo;
        LevelSwitcher.OnEndSwitch -= HandleEndSwitchLevel;
        LevelSwitcher.OnCancelSwitch -= HandleCancelSwitchLevel;
    }

    private void HandleEndSwitchLevel(LevelScriptableObject levelScriptableObject)
    {
        _switchingLevel = false;
        storeButtonCanvasGroup.blocksRaycasts = true;
        coinsCrystalsCanvasGroup.blocksRaycasts = true;
        
        if (_playerLevels.LevelBought(levelScriptableObject))
        {
            trapButton.transform.DOMove(_trapsButtonStartPosition, 0.5f);
            cannonballsStoreButton.SetActive(true);
            cannonsStoreButton.SetActive(true);
        }
    }

    private void HandleCancelSwitchLevel(LevelScriptableObject levelScriptableObject)
    {
        _switchingLevel = false;
        storeButtonCanvasGroup.blocksRaycasts = true;
        coinsCrystalsCanvasGroup.blocksRaycasts = true;
        
        if (_playerLevels.LevelBought(_playerLevels.CurrentLevel))
        {
            trapButton.transform.DOMove(_trapsButtonStartPosition, 0.5f);
            cannonballsStoreButton.SetActive(true);
            cannonsStoreButton.SetActive(true);
        }
    }
    
    private void HandleStartSwitchLevel(LevelScriptableObject levelScriptableObject)
    {
        _switchingLevel = true;
        storeButtonCanvasGroup.blocksRaycasts = false;
        coinsCrystalsCanvasGroup.blocksRaycasts = false;

        if (!_playerLevels.LevelBought(levelScriptableObject))
        {
            trapButton.transform.DOMove(_trapsButtonStartPosition + new Vector3(0f, 1.5f * _trapsButtonRect.rect.height), 0.5f);
            cannonballsStoreButton.SetActive(false);
            cannonsStoreButton.SetActive(false);
        }
    }

    public void StoreButtonClick()
    {
        if (_switchingLevel)
        {
            return;
        }

        _uiStateMachine.ShowGenericStore(StoreType.Obstacle);
    }

    public void HandleStartGameButtonClick()
    {
        if (_switchingLevel)
        {
            return;
        }

        gameState.SendStartGame();
    }
    
    public void HandleElkClick()
    {
        //Unlock later
        //uiStateMachine.ChangeState(elkStoreUiState);
    }
    
    public void BuyDuckButtonClick()
    {
        if (_switchingLevel || !_playerLevels.CanBuyNextPartOfLevel())
        {
            return;
        }
        
        confirmPanel.Initialize(_playerLevels.GetNextPartOfLevel(),
            () =>
            {
                _playerLevels.BuyNextPartOfLevel();
                AudioManager.Instance.Stop("ui_buy");
                AudioManager.Instance.PlayWithOverlay("ui_button_lvlbuy_" + UnityEngine.Random.Range(1,3));
            },
            null, false);
    }
    
    public void BuyLevelButtonClick()
    {
        if (_switchingLevel || _levelBought)
        {
            return;
        }
        
        confirmPanel.Initialize(_playerLevels.CurrentLevel, 
            ()=>
            {
                _playerLevels.BuyLevel();
                UpdateLevelInfo(_playerLevels.CurrentLevel);
                if (_levelBought)
                {
                    trapButton.transform.DOMove(_trapsButtonStartPosition, 0.5f);
                    cannonballsStoreButton.SetActive(true);
                    cannonsStoreButton.SetActive(true);
                }
                else
                {
                    trapButton.transform.position = _trapsButtonStartPosition + new Vector3(0f, 1.5f * _trapsButtonRect.rect.height);
                    cannonballsStoreButton.SetActive(false);
                    cannonsStoreButton.SetActive(false);
                }
            },
            null, false);
    }
    
    public void AdsButtonClick()
    {
        if (_switchingLevel)
        {
            return;
        }
        
        adsAchievementsUi.gameObject.SetActive(true);
        adsAchievementsUi.Initialize();
        AudioManager.Instance.PlayWithOverlay("ui_button");
    }
    
    public void CannonballsStoreButtonClick()
    {
        if (_switchingLevel)
        {
            return;
        }
        
        _uiStateMachine.BuyStackItemsPanel.Initialize(_playerCannonballs.CurrentCannonball, HandleCannonballsBuy);
        
    }

    private void HandleCannonballsBuy(int number)
    {
        _playerCannonballs.BuyBombsStack(_playerCannonballs.CurrentCannonball, number);
        cannonBallsNumberText.text = _playerCannonballs.GetNumberInStack(_playerCannonballs.CurrentCannonball).ToString();
    }

    private void UpdateLevelInfo(LevelScriptableObject levelScriptableObject)
    {
        if (_playerLevels.LevelBought(levelScriptableObject))
        {
            _levelBought = true;
        }
        else
        {
            _levelBought = false;
        }
    }

    private void UpdateButtonPositions()
    {
        if (_levelBought)
        {
            trapButton.transform.position = _trapsButtonStartPosition;
        }
        else
        {
            trapButton.transform.position = _trapsButtonStartPosition + new Vector3(0f, 1.5f * _trapsButtonRect.rect.height);
        }
    }

    public override void ExitButtonClick()
    {
        if (_switchingLevel)
        {
            return;
        }
        
        if (adsAchievementsUi.gameObject.activeSelf)
        {
            adsAchievementsUi.gameObject.SetActive(false);
            return;
        }
        
        if (_oneClickOnExitButton)
        {
            Application.Quit();
        }
        
        doubleClickTextCanvasGroup.alpha = 1f;
        
        DOTween.Sequence()
            .AppendInterval(0.5f)
            .Append(doubleClickTextCanvasGroup.DOFade(0f, 1f));
        
        _oneClickOnExitButton = true;
        StartCoroutine(ForgetExitClick());
    }

    IEnumerator ForgetExitClick()
    {
        yield return new WaitForSeconds(forgetClickOnExitButtonTime);
        _oneClickOnExitButton = false;
    }
}
