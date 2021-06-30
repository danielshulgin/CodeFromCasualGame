using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class DebugTrajectory : MonoBehaviour
{

    [SerializeField] private GameState gameState;
    
    [SerializeField] private GameObject bullet;
    
    [SerializeField] private float interval = 0.2f;

    [SerializeField] private bool updateEveryFixedUpdate = true;
    
    [SerializeField] private int maxTrajectoryPointsCount = 100000;
 
    [Inject] private PlayerBulletSkins _playerBulletSkins; 

    private List<Vector3> _trajectoryPoints = new List<Vector3>();
    
    private LineRenderer _lineRenderer;

    
    private void Start()
    {
        //Save few clicks in non critical place(Debug!!!!) 
        if(gameState == null)
            gameState = FindObjectOfType<GameState>();
        if(_playerBulletSkins == null)
            _playerBulletSkins = FindObjectOfType<PlayerBulletSkins>();
        /*if(bullet == null)
            bullet = _playerBulletSkins.BulletGameObject;*/
        _lineRenderer = GetComponent<LineRenderer>();
        
        GameState.OnStartFly += HandleStartFly;
        GameState.OnEndFly += HandleEndFly;
        
       // _playerBulletSkins.OnChangeBullet += ResetBullet;
    }

    private void OnDestroy()
    {
        GameState.OnStartFly -= HandleStartFly;
        GameState.OnEndFly -= HandleEndFly;
    }

    IEnumerator SaveTrajectoryPoints()
    {
        while (_trajectoryPoints.Count < maxTrajectoryPointsCount)
        {
            _trajectoryPoints.Add(bullet.transform.position);
            if (updateEveryFixedUpdate)
            {    
                yield return new WaitForFixedUpdate();
            }
            else
            {
                yield return new WaitForSeconds(interval);
            }
        }
    }

    private void HandleStartFly()
    {
        _trajectoryPoints = new List<Vector3>();
        StartCoroutine(SaveTrajectoryPoints());
    }

    private void HandleEndFly()
    {
        StopAllCoroutines();
        SetLineRenderer(_trajectoryPoints);
    }

    private void ResetBullet(GameObject bullet)
    {
        this.bullet = bullet;
    }
    
    private void SetLineRenderer(List<Vector3> positions)
    {
        _lineRenderer.positionCount = positions.Count;
        _lineRenderer.SetPositions(positions.ToArray());
    }
}
