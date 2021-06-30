using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

//TODO rename
public class BodyPartsResetHandler : MonoBehaviour
{
    [SerializeField] private RotateCarousel rotateCarousel;
    
    [Inject] private PlayerBulletSkins _playerBulletSkins;

    private Transform _parent;
    
    private List<Vector3> _startPositions;
    
    private List<Quaternion>  _startRotations;
    
    private List<GameObject> _children;
    
    private Vector3 _startPosition;
    
    private Quaternion  _startRotation;


    private void Awake()
    {
        BulletSpawner.OnChangeBullet += HandleChangeBullet;
    }

    private void Start()
    {
        GameState.OnStartFly += HandleStartFly;
    }

    private void OnDestroy()
    {
        BulletSpawner.OnChangeBullet -= HandleChangeBullet;
        GameState.OnStartFly -= HandleStartFly;
    }

    private void HandleChangeBullet(GameObject bullet)
    {
        _parent = bullet.transform;  
        SaveAllChildren();
    }
    
    private void SaveAllChildren()
    {
        _startPosition = _parent.transform.position;
        _startRotation = _parent.transform.rotation;
        _startPositions = new List<Vector3>();
        _startRotations = new List<Quaternion>();
        _children = new List<GameObject>();
        for (int i = 0; i< _parent.transform.childCount; i++)
        {
            var child = _parent.transform.GetChild(i).gameObject;
            _children.Add(child);
            _startPositions.Add(child.transform.position);
            _startRotations.Add(child.transform.rotation);
        }
    }
    
    public void HandleStartFly()
    {
        var bulletParts = _parent.GetComponent<BulletParts>();
        
        var parentBodyPartImpactHandler = bulletParts.pivotBodyPartImpactHandler;
        if (parentBodyPartImpactHandler != null)
        {
            parentBodyPartImpactHandler.Initialize();
        }

        var parentBulletHp = bulletParts.bulletHp;
        if (parentBulletHp != null)
        {
            parentBulletHp.Initialize();
        }
    }
}
