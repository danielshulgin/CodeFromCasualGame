using System;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class LevelSwitcher : SwipeSwitcher
{
    public static event Action<LevelScriptableObject> OnEndSwitch;
    
    public static event Action<LevelScriptableObject> OnStartSwitch;
    
    public static event Action<LevelScriptableObject> OnCancelSwitch;
    
    public static event Action<float> OnUpdateRelativePosition;
    
    [SerializeField] private ClickManager clickManager;

    [Inject] private PlayerLevels _playerLevels;

    private LevelScriptableObject _nextLevelScriptableObject;
    
    private int _maxLevelNumber = 2;

    protected override float Width => Screen.width;

    protected override bool Press => clickManager.Press;


    private void Awake()
    {
        clickManager.OnSwipeRight.AddListener(touch =>
        {
            HandleSwap(touch.deltaPosition.x, true);
        });
        clickManager.OnSwipeLeft.AddListener(touch =>
        {
            HandleSwap(touch.deltaPosition.x, false);
        });
    }

    private void Start()
    {
        _maxLevelNumber = _playerLevels
            .LevelScriptableObjects
            .Select(so=>so.levelNumber)
            .Max();
    }

    protected override void HandleStartSwitch(bool rightDirection)
    {
        _nextLevelScriptableObject = GetNextLevel(rightDirection);
        OnStartSwitch?.Invoke(_nextLevelScriptableObject);
    }

    protected override void HandleEndSwitch()
    {
        OnEndSwitch?.Invoke(_nextLevelScriptableObject);
        AudioManager.Instance.PlayWithOverlay("ui_button_lvlchng");
    }

    protected override void HandleCancelSwitch()
    {
        OnCancelSwitch?.Invoke(_nextLevelScriptableObject);
    }

    protected override void HandleUpdateRelativePosition(float position)
    {
        OnUpdateRelativePosition?.Invoke(position);
    }

    private LevelScriptableObject GetNextLevel(bool rightDirection)
    {
        var currentLevelNumber = _playerLevels.CurrentLevel.levelNumber;
        var newLevelNumber = rightDirection ? currentLevelNumber - 1 : currentLevelNumber + 1;
        
        if (newLevelNumber < 1)
        {
            newLevelNumber = _maxLevelNumber;
        }
        else if (newLevelNumber > _maxLevelNumber)
        {
            newLevelNumber = 1;
        }
        
        return _playerLevels
            .LevelScriptableObjects
            .Single(levelScriptableObject => levelScriptableObject.levelNumber == newLevelNumber);
    }
}
