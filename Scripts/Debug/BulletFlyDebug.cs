using System;
using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BulletFlyDebug : MonoBehaviour
{
    [SerializeField] private bool showDebug = true;
    
    [Foldout("Dependencies", true)]
    [SerializeField] private BulletCollision bulletCollision;
    
    [SerializeField] private GameState gameState;

    [SerializeField] private Text debugText;

    [Inject] private PlayerBulletSkins _playerBulletSkins;

    private Rigidbody2D _bulletRigidbody2D;
    
    private BulletHP _bulletHp;

    private bool _fly;
    
    private Vector2 _bulletVelocityInCurrentFrame = Vector2.zero;
    private Vector2 _bulletVelocityInPreviousFrame = Vector2.zero;

    
    private void Start()
    {
        GameState.OnStartFly += HandleStartFly;
        GameState.OnEndFly += HandleEndFly;
        //_playerBulletSkins.OnChangeBullet += ChangeBullet;
        bulletCollision.OnFullBodyCollisionEnter2D += HandleBulletCollision;
    }

    private void OnDestroy()
    {
        GameState.OnStartFly -= HandleStartFly;
        GameState.OnEndFly -= HandleEndFly;
    }

    private void HandleStartFly()
    {
        _fly = true;
    }

    private void HandleEndFly()
    {
        _fly = false;
        debugText.text = "";
    }

    private void ChangeBullet(GameObject bullet)
    {
        _bulletRigidbody2D = bullet.GetComponent<Rigidbody2D>();
        _bulletHp = bullet.GetComponent<BulletHP>();
    }

    private void HandleBulletCollision(Collision2D collision2D)
    {
        if (!showDebug || !_fly)
            return;
        StartCoroutine(CollisionDebugRoutine(collision2D));
    }

    private void Update()
    {
        if (!showDebug || !_fly || _bulletRigidbody2D == null) 
            return;
        
        var velocity = _bulletRigidbody2D.velocity;
        debugText.text = $"Velocity\ny:{velocity.y:0.00}\n" +
                         $"x:{velocity.x:0.00}\n" +
                         $"magnitude:{velocity.magnitude:0.00}\n" +
                         $"angular velocity:{_bulletRigidbody2D.angularVelocity:0.00}\n" +
                         $"hp:{_bulletHp.Hp}\n";
    }

    private void FixedUpdate()
    {
        if (!showDebug || !_fly || _bulletRigidbody2D == null) 
            return;
        _bulletVelocityInPreviousFrame = _bulletVelocityInCurrentFrame;
        _bulletVelocityInCurrentFrame = _bulletRigidbody2D.velocity;
    }
    

    private IEnumerator CollisionDebugRoutine(Collision2D collision2D)
    {
        var collisionDebugText = $"Before collision\n" +
                                 $"    Bullet Velocity:\n" +
                                 $"        X axis: <color=#142ec4>{_bulletVelocityInPreviousFrame.x:0.00}</color>\n" +
                                 $"        Y axis: <color=#142ec4>{_bulletVelocityInPreviousFrame.y:0.00}</color>\n" +
                                 $"    Additional:\n" +
                                 $"        Magnitude: <color=#142ec4>{_bulletVelocityInPreviousFrame.magnitude:0.00}</color>\n" +
                                 $"        Collision with: {collision2D.gameObject.name}\n"+
                                 $"        Collision position: <color=#142ec4>{collision2D.transform.position}</color>\n";
        Debug.Log(collisionDebugText);
        yield return new WaitForFixedUpdate();
        var bulletVelocityAfter = _bulletRigidbody2D.velocity;
        var afterCollisionDebugText = $"After collision\n" +
                              $"    Bullet Velocity:\n" +
                              $"        X axis: <color=#142ec4>{bulletVelocityAfter.x:0.00}</color>\n" +
                              $"        Y axis: <color=#142ec4>{bulletVelocityAfter.y:0.00}</color>\n" +
                              $"    Additional:\n" +
                              $"        Magnitude: <color=#142ec4>{bulletVelocityAfter.magnitude:0.00}</color>\n"+
                              $"        Collision with: {collision2D.gameObject.name}";
        Debug.Log(afterCollisionDebugText);
    }

    private void OnValidate()
    {
        if (!showDebug)
        {
            debugText.text = "";
        }
    }
}
