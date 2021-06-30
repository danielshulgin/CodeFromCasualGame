using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class ObstaclesStoreUIState : GameCurrencyStoreUIState
{
    [SerializeField] private Sprite lockedSprite;
    
    [SerializeField] private List<RectTransform> skullsRects;

    [SerializeField] private float skullSpaceWidth = 30f;
    
    [SerializeField] private GameObject airObstacleHolder;

    [Inject] private PlayerLevels _playerLevels;

    private List<TabsSeparator> _tabsSeparators;
    

    protected override PlayerDataPart DataPart => _playerLevels;


    public override void Enter()
    {
        base.Enter();
        storesUIDependencies.characteristicsPanel.SetActive(true);
        airObstacleHolder.SetActive(false);
        
        var obstacleScriptableObject = (ObstacleScriptableObject)SelectedItem;
        airObstacleHolder.SetActive(obstacleScriptableObject.airObstacle && _playerLevels.GetItemState(obstacleScriptableObject) != ItemState.Locked);
    }

    public override void Exit()
    {
        base.Exit();
        storesUIDependencies.characteristicsPanel.SetActive(false);
        airObstacleHolder.SetActive(false);
        foreach (var skullsRect in skullsRects)
        {
            skullsRect.gameObject.SetActive(false);
        }
    }
    
    protected override void InitializeTabs()
    {
        _tabs = new List<ITabUI>();
        _tabsSeparators = new List<TabsSeparator>();
        for (var i = 0; i < _playerLevels.CurrentLevel.levelPartScriptableObjects.Count; i++)
        {
            AddTabSeparator(i + 1);
            var levelPart = _playerLevels.CurrentLevel.levelPartScriptableObjects[i];
            foreach (var obstacleScriptableObject in levelPart.obstaclesInStore)
            {
                AddTab(obstacleScriptableObject, DataPart.GetItemState(obstacleScriptableObject));
            }
        }
    }

    private void AddTabSeparator(int number)
    {
        var tabSeparatorGameObject = Instantiate(storesUIDependencies.tabsSeparatorPrefab, storesUIDependencies.tabParent.transform, false);
        var tabsSeparator = tabSeparatorGameObject.GetComponent<TabsSeparator>();
        tabsSeparator.Initialize(Localization.Instance.GetTranslation("level_part_separator_name") + " " + number);
        _tabsSeparators.Add(tabsSeparator);
    }

    protected override void ConfirmBuyButtonClick()
    {
        base.ConfirmBuyButtonClick();
        _playerLevels.UpdateLevel();
    }
    
    protected override void SelectTab(ITabUI tabUi)
    {
        if (_previousSelectedTab != null)
        {
            _previousSelectedTab.DeSelect();
        }
        _previousSelectedTab = tabUi;
        
        SelectedItem = tabUi.ItemScriptableObject;
        var itemState = DataPart.GetItemState(tabUi.ItemScriptableObject);
        var selectedImageSprite = tabUi.ItemScriptableObject.storeSprite;
        if (itemState == ItemState.Available)
        {
            storesUIDependencies.buyButton.gameObject.SetActive(true);
        }
        else if (itemState == ItemState.Locked)
        {
            selectedImageSprite = lockedSprite;
        }
        else
        {
            storesUIDependencies.buyButton.gameObject.SetActive(false);
        }

        storesUIDependencies.selectedImage.sprite = selectedImageSprite;
        LockItemIfComingSoon();
        
        var obstacleScriptableObject = (ObstacleScriptableObject)SelectedItem;
        UpdateCharacteristics(obstacleScriptableObject.skullsNumber);
        
        airObstacleHolder.SetActive(obstacleScriptableObject.airObstacle && itemState != ItemState.Locked);
    }
    
    protected override List<ItemScriptableObject> GetStoreItemList()
    {
        var obstacles = new List<ItemScriptableObject>();
        
        foreach (var t in _playerLevels.CurrentLevel.levelPartScriptableObjects)
        {
            obstacles.AddRange(t.obstaclesInStore);
        }

        return obstacles;
    }

    private void UpdateCharacteristics(int skullsNumber)
    {
        foreach (var skullsRect in skullsRects)
        {
            skullsRect.gameObject.SetActive(false);
        }
        var skullWidth = skullsRects[0].rect.width;
        var skullPosition = -(skullsNumber - 1) * (skullWidth + skullSpaceWidth) / 2f;
        for (var i = 0; i < skullsNumber; i++)
        {
            skullsRects[i].gameObject.SetActive(true);
            skullsRects[i].anchoredPosition = new Vector2(skullPosition, 0f);
            skullPosition += skullWidth + skullSpaceWidth;
        }
        for (var i = skullsNumber; i < skullsRects.Count; i++)
        {
            skullsRects[i].gameObject.SetActive(false);
        }
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);
        SetInteractableCharacteristicsPanel(interactable);
    }

    public void SetInteractableCharacteristicsPanel(bool interactable)
    {
        var color = interactable ? Constants.NormalColor : Constants.DisabledColor;
        storesUIDependencies.characteristicsPanel.GetComponent<Image>().color
            = color;
        foreach (var skullRect in skullsRects)
        {
            skullRect.GetComponent<Image>().color = color;    
        }

        foreach (var tabsSeparator in _tabsSeparators)
        {
            tabsSeparator.SetInteractable(interactable);
        }
    }
}
