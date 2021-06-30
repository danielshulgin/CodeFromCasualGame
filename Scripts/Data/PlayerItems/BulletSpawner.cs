using System;
using UnityEngine;
using Zenject;


public class BulletSpawner : MonoBehaviour
{
    public static event Action<GameObject> OnChangeBullet;

    private GameObject _currentBulletGameObject;

    [Inject] private PlayerBulletSkins _playerBulletSkins;
    
    
    private void Start()
    {
        PlayerBulletSkins.OnChangeBulletSkin += HandleChangeBulletSkin;
        HandleChangeBulletSkin(_playerBulletSkins.CurrentBulletSkin);
    }

    private void OnDestroy()
    {
        PlayerBulletSkins.OnChangeBulletSkin -= HandleChangeBulletSkin;
    }

    private void HandleChangeBulletSkin(BulletSkinScriptableObject bulletSkinScriptableObject)
    {
        if(_currentBulletGameObject!= null)
            Destroy(_currentBulletGameObject);
                
        _currentBulletGameObject = Instantiate(
            bulletSkinScriptableObject.bulletPrefab, transform);
            
        OnChangeBullet?.Invoke(_currentBulletGameObject);
        _currentBulletGameObject.SetActive(false);
    }
}
