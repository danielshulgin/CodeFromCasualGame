using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private Transform levelParent;
    
    [SerializeField] private GameObject currentLevel;
    
    [FormerlySerializedAs("currentBackGround")] 
    [SerializeField] private GameObject currentMainMenuBackGround;

    [Inject] private PlayerLevels _playerLevels;

    private GameObject _newMainMenuBackGround;


    private void Awake()
    {
        _playerLevels.OnStartSwitchLevel += HandleStartSwitchLevel;
        _playerLevels.OnCancelSwitchLevel += HandleCancelSwitchLevel;
        
        _playerLevels.OnChangeLevel += UpdateGameLevel;
        _playerLevels.OnChangeLevel += UpdateMainMenuBackGround;
        _playerLevels.OnChangeTraps += UpdateGameLevel;
        
        PlayerLevels.OnBuyLevel += HandleLevelBought;
        
        GameState.OnStartGame += HandleStartGame;
        GameState.OnResetGame += HandleResetGame;
        UpdateGameLevel();
    }
    
    private void OnDestroy()
    {
        _playerLevels.OnStartSwitchLevel -= HandleStartSwitchLevel;
        _playerLevels.OnCancelSwitchLevel -= HandleCancelSwitchLevel;
        
        _playerLevels.OnChangeLevel -= UpdateGameLevel;
        _playerLevels.OnChangeLevel -= UpdateMainMenuBackGround;
        _playerLevels.OnChangeTraps -= UpdateGameLevel;
        
        PlayerLevels.OnBuyLevel -= HandleLevelBought;
        
        GameState.OnStartGame -= HandleStartGame;
        GameState.OnResetGame -= HandleResetGame;
    }

    public void HandleLevelBought(LevelScriptableObject levelScriptableObject)
    {
        UpdateGameLevel();
        var levelMask = currentMainMenuBackGround.GetComponent<LevelMask>();
        levelMask.SetFadeActive(!_playerLevels.LevelBought(_playerLevels.CurrentLevel));
    }
    
    public void UpdateGameLevel()
    {
        if (_playerLevels.CurrentLevel != null)
        {
            if (currentLevel != null)
            {
                Destroy(currentLevel);
                var levelGameObject = Instantiate(_playerLevels.CurrentLevel.levelPrefab, levelParent);
            
                levelGameObject.transform.position = transform.position;
                currentLevel = levelGameObject;
            }
            
            var levelComponent = currentLevel.GetComponent<LevelComponent>();

            InitializeLevelTraps(levelComponent);
            InitializeLevelParts(levelComponent);

            currentLevel.SetActive(false);
        }
    }

    private void InitializeLevelTraps(LevelComponent levelComponent)
    {
        foreach (var obstacleType in levelComponent.obstacleTypes)
        {
            if (_playerLevels.GetTrapState(obstacleType.obstacleScriptableObject) == ItemState.Bought)
            {
                foreach (var gameObjectToActivate in obstacleType.gameObjectsToActivate)
                {
                    gameObjectToActivate.SetActive(true);
                }
            }
            else
            {
                foreach (var gameObjectToActivate in obstacleType.gameObjectsToActivate)
                {
                    gameObjectToActivate.SetActive(false);
                }
            }
        }
    }

    private void InitializeLevelParts(LevelComponent levelComponent)
    {
        foreach (var levelPart in levelComponent.levelParts)
        {
            if (_playerLevels.GetLevelPartState(levelPart.levelPartScriptableObject) == ItemState.Bought)
            {
                foreach (var wall in levelPart.walls)
                {
                    wall.SetActive(false);
                }
            }
        }
    }

    private void HandleStartSwitchLevel(LevelScriptableObject levelScriptableObject)
    {
        if (_newMainMenuBackGround!= null)
        {
            Destroy(_newMainMenuBackGround);
        }
        _newMainMenuBackGround = Instantiate(_playerLevels.GetCurrentLevelPart(levelScriptableObject).mainMenuBackGroundPrefab, levelParent);
        _newMainMenuBackGround.GetComponentInChildren<DuckSprite>().Initialize(levelScriptableObject);
        _newMainMenuBackGround.transform.localPosition = Vector3.zero;
        
        var levelMask = _newMainMenuBackGround.GetComponent<LevelMask>();
        levelMask.SwitchMask(SpriteMaskInteraction.VisibleOutsideMask);
        
        levelMask.SetFadeActive(!(_playerLevels.LevelBought(levelScriptableObject)));
        
        currentMainMenuBackGround.GetComponent<LevelMask>().SwitchMask(SpriteMaskInteraction.VisibleInsideMask);
    }

    private void HandleCancelSwitchLevel(LevelScriptableObject levelScriptableObject)
    {
        Destroy(_newMainMenuBackGround);
    }

    public void HandleStartGame()
    {
        currentLevel.SetActive(true);
        currentMainMenuBackGround.SetActive(false);
    }

    public void HandleResetGame()
    {
        currentLevel.SetActive(false);
        currentMainMenuBackGround.SetActive(true);
    }

    private void UpdateMainMenuBackGround()
    {
        Destroy(currentMainMenuBackGround);
        if (_newMainMenuBackGround != null)
        {
            Destroy(currentMainMenuBackGround);
            currentMainMenuBackGround = _newMainMenuBackGround;
            _newMainMenuBackGround = null;
        }
        else
        {
            currentMainMenuBackGround = Instantiate(_playerLevels.GetCurrentLevelPart(_playerLevels.CurrentLevel)
                .mainMenuBackGroundPrefab, levelParent);
            currentMainMenuBackGround.GetComponentInChildren<DuckSprite>().Initialize(_playerLevels.CurrentLevel);
            currentMainMenuBackGround.transform.localPosition = Vector3.zero;
        }
        
        var levelMask = currentMainMenuBackGround.GetComponent<LevelMask>();
        levelMask.SetFadeActive(!_playerLevels.LevelBought(_playerLevels.CurrentLevel));
    }
}
