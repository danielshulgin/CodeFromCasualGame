using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerBulletSkins : PlayerDataPart
{
    public static event Action<BulletSkinScriptableObject> OnChangeBulletSkin;

    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;
    
    [SerializeField] private GameObject currentBulletGameObject;

    //[Inject(Id = "bulletParentGameObject")] private GameObject bulletParentGameObject;

    public GameObject BulletGameObject => currentBulletGameObject;
    
    public BulletSkinScriptableObject CurrentBulletSkin =>
        _scriptableObjectDataBase.BulletSkinScriptableObjectsById[_playerData.CurrentBulletSkinId];

    public BulletScriptableObject CurrentBullet => 
        _scriptableObjectDataBase.BulletScriptableObjectsById[CurrentBulletSkin.bullet.id];

    public ReadOnlyCollection<BulletScriptableObject> BulletScriptableObjects => 
        new ReadOnlyCollection<BulletScriptableObject>(_scriptableObjectDataBase.BulletScriptableObjectsById.Values.ToList());
    
    public ReadOnlyCollection<BulletSkinScriptableObject> BulletSkinScriptableObjects => 
        new ReadOnlyCollection<BulletSkinScriptableObject>(_scriptableObjectDataBase.BulletSkinScriptableObjectsById.Values.ToList());
    
    [Inject] private ScriptableObjectDataBase _scriptableObjectDataBase;
    
    
    private void Awake()
    {
        _playerData.OnPlayerDataUpdate += HandlePlayerDataUpdate;
    }
    
    private void Start()
    {
        GameState.OnResetGame += HandleResetGame;
    }
    
    private void OnDestroy()
    {
        GameState.OnResetGame -= HandleResetGame;
    }

    private void HandlePlayerDataUpdate()
    {
        SelectBulletSkin(_playerData.CurrentBulletSkinId);
    }
    
    public void HandleResetGame()
    {
        SelectBulletSkin(_playerData.CurrentBulletSkinId);
    }

    public bool SelectBulletSkin(ItemScriptableObject itemScriptableObject)
    {
        return SelectBulletSkin(itemScriptableObject.id);
    }

    private bool SelectBulletSkin(int id)
    {
        
        if (!_playerData.SingleItemIdPresentInPlayerData(id))
        {
            id = defaultPlayerSettings.currentBulletSkin.id;
        }
        if (_playerData.ItemStateById[id] == ItemState.Bought)
        {
            _playerData.CurrentBulletSkinId = id;
            
            var bulletSkinScriptableObject = _scriptableObjectDataBase.BulletSkinScriptableObjectsById[id];
            
            OnChangeBulletSkin?.Invoke(bulletSkinScriptableObject);

            _playerData.Save();
            return true;
        }
        return false;
    }
}
