using System;
using System.Collections;
using Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class Cannon : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private float z = -5;

    [SerializeField] private float radius;
    
    [SerializeField] private float maxFlyTime = 5f;

    [SerializeField] private float explosionCannonBallLenghtK = 0.1f;
    
    [SerializeField] private float gravity = -0.1f;
    
    [SerializeField] private float minInputPower = 0.4f;
    
    [SerializeField] private float minTrajectoryLenghtK = 0.1f;
    
    [SerializeField] private float maxTrajectoryLenghtK = 0.8f;
    
    [SerializeField] private float minCannonBallSpeed = 1f;
    
    [SerializeField] private float manCannonBallSpeed = 100f;

    [Inject] private PlayerCannonballs _playerCannonballs;
    
    [Inject] private PlayerBulletSkins _playerBulletSkins;
    
    [Inject] private PlayerCannons _playerCannons;

    private BulletScriptableObject _bulletScriptableObject;
    
    private Rigidbody2D _bulletRigidBody;

    private CannonballScriptableObject _cannonballScriptableObject;
    
    private CannonScriptableObject _cannonScriptableObject;


    private void Awake()
    {
        BulletSpawner.OnChangeBullet += HandleChangeBullet;
        PlayerCannonballs.OnChangeCannonball += HandleChangeCannonball;
        PlayerCannons.OnChangeCannon += HandleChangeCannon;
        HandleChangeCannonball(_playerCannonballs.CurrentCannonball);
        HandleChangeCannon(_playerCannons.CurrentCannon);
    }

    private void OnDestroy()
    {
        BulletSpawner.OnChangeBullet -= HandleChangeBullet;
        PlayerCannonballs.OnChangeCannonball -= HandleChangeCannonball;
        PlayerCannons.OnChangeCannon -= HandleChangeCannon;
    }

    public void LaunchCannonball(Vector3 startOffset, Vector3 direction, float inputPower)
    {
        var cannonBallGameObject = Instantiate(_cannonballScriptableObject.cannonballPrefab);
        var cannonBall = cannonBallGameObject.GetComponent<Cannonball>();
        direction = RandomizeStartDirection(direction, _cannonScriptableObject.maxDeflectionAngle);
        
        if (_cannonballScriptableObject.cannonballImpactType == CannonballImpactType.Collision)
        {
            StartCollisionCannonBall(cannonBall, startOffset, direction, inputPower);
        }
        else if (_cannonballScriptableObject.cannonballImpactType == CannonballImpactType.Explosion)
        {
            StartExplosionCannonBall(cannonBall, startOffset, direction, inputPower);
        }
    }

    private void StartCollisionCannonBall(Cannonball cannonball, Vector2 startOffset, Vector2 direction, float inputPower)
    {
        var recalculatedInputPower = HelperFunctions.RangeToRange(inputPower, 0f, 1f, minInputPower, 1f);
        
        var velocity = GetCannonBallStartVelocity(direction, recalculatedInputPower);

        cannonball.StartMove(startOffset, velocity, gravity, maxFlyTime, true, _cannonballScriptableObject.maxBounceFlyTime);
        cannonball.OnCollisionWithBullet += (d) => AddImpact(d, recalculatedInputPower);
    }
    
    private void StartExplosionCannonBall(Cannonball cannonball, Vector2 startOffset, Vector2 direction, float inputPower)
    {
        var recalculatedInputPower = HelperFunctions.RangeToRange(inputPower, 0f, 1f, minInputPower, 1f);
        
        var velocity = GetCannonBallStartVelocity(direction, recalculatedInputPower);

        var screenHeightInWordUnits = Camera.main.orthographicSize * 2f;
        var lengthK = _cannonScriptableObject.shotForce / _cannonballScriptableObject.mass * explosionCannonBallLenghtK;
        lengthK = Mathf.Clamp(lengthK, minTrajectoryLenghtK, maxTrajectoryLenghtK);
        
        cannonball.StartMove(startOffset, velocity, gravity, inputPower * screenHeightInWordUnits * lengthK / velocity.magnitude, false);
        cannonball.OnEndFly += SpawnExplosion;
    }

    private Vector2 GetCannonBallStartVelocity(Vector2 direction, float recalculatedInputPower)
    {
        var speed = recalculatedInputPower * _cannonScriptableObject.shotForce / _cannonballScriptableObject.mass; 
        speed = Mathf.Clamp(speed, minCannonBallSpeed, manCannonBallSpeed);
        return speed * direction;
    }

    private Vector2 RandomizeStartDirection(Vector2 direction, float maximumDeflectionAngle)
    {
        var deflectionAngle = UnityEngine.Random.Range(-1f, 1f) * maximumDeflectionAngle;
        return Quaternion.AngleAxis(deflectionAngle, Vector3.forward) * direction;
    }

    private void SpawnExplosion(Vector2 position)
    {
        Vector3 wordPosition = new Vector3(position.x, position.y, z);
        var explosion = Instantiate(explosionPrefab);
        explosion.transform.position = wordPosition;

        var distanceToExplosion = _bulletRigidBody.transform.position - wordPosition;
        if(distanceToExplosion.magnitude > radius)
            return;
        
        _bulletRigidBody.AddForce((radius - distanceToExplosion.magnitude) 
                                   *  _cannonballScriptableObject.explosionImpact
                                   /  _bulletScriptableObject.cannonBallCollisionResistance
                                   *  distanceToExplosion.normalized);
    }

    private void AddImpact(Vector2 direction, float inputPower)
    {
        _bulletRigidBody.AddForce(inputPower
                                  * _cannonballScriptableObject.collisionImpact
                                  *  _cannonScriptableObject.shotForce
                                  /  _bulletScriptableObject.cannonBallExplosionResistance
                                  * direction);
    }

    private void HandleChangeBullet(GameObject bullet)
    {
        _bulletRigidBody = bullet.GetComponent<Rigidbody2D>();
        _bulletScriptableObject = _playerBulletSkins.CurrentBullet;
    }
    
    private void HandleChangeCannonball(CannonballScriptableObject cannonballScriptableObject)
    {
        _cannonballScriptableObject = cannonballScriptableObject;
    }
    
    private void HandleChangeCannon(CannonScriptableObject cannonScriptableObject)
    {
        _cannonScriptableObject = cannonScriptableObject;
    }
}
