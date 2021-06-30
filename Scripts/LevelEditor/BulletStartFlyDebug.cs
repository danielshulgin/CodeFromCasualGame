using System.Reflection;
using Generic;
using MyBox;
using UnityEngine;

public class BulletStartFlyDebug : MonoBehaviour
{
    [SerializeField] private float timeScale = 10;
    
    [SerializeField] [Range(0f, 1f)] private float relativeForce;
    
    [SerializeField] private float torqueAbsoluteValue;
    
    [SerializeField] [Range(-0.99f, 0.99f)] private float relativePosition;

    [SerializeField] private bool rightFlyDirection;

    [SerializeField] private GameState gameState;

    [SerializeField] private BulletFly bulletFly;
    
    [SerializeField] private GameForceAdjusterRealization forceAdjuster;
    
    [SerializeField] private RotateCarousel rotateCarousel;

    private float _defaultForceReduceSpeed;
    
    private float _defaultForceAddSpeed;
    
    
    private void Start()
    {
        //Save few clicks in non critical place(Debug!!!!) 
        if(gameState == null)
            gameState = FindObjectOfType<GameState>();
        if(forceAdjuster == null)
            forceAdjuster = FindObjectOfType<GameForceAdjusterRealization>();
        if(rotateCarousel == null)
            rotateCarousel = FindObjectOfType<RotateCarousel>();
        if(bulletFly == null)
            bulletFly = FindObjectOfType<BulletFly>();
        
        OnValidate();
    }

    private void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            SetPrivateFiled(forceAdjuster, "force", relativeForce);
            SetPrivateFiled(bulletFly, "torque", torqueAbsoluteValue);
            rotateCarousel.UpdateBulletPosition();

            Time.timeScale = timeScale;

            SetPrivateFiled(forceAdjuster, "forceReduceSpeed", 0f);
            SetPrivateFiled(forceAdjuster, "forceAddSpeed", 0f);
            SetPrivateFiled(rotateCarousel, "startSpeed", 0f);
        }
    }

    // Only for Debug!!!
    private void SetPrivateFiled(object target, string fieldName, object value)
    {
        target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(target, value);
    }
    
    // Only for Debug!!!
    private object GetPrivateFiled(object target, string fieldName)
    {
        return target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(target);
    }
    
    
    [ButtonMethod]
    public void StartBulletFly()
    {
        SetPrivateFiled(forceAdjuster, "forceReduceSpeed", 0f);
        SetPrivateFiled(forceAdjuster, "forceAddSpeed", 0f);
        SetPrivateFiled(rotateCarousel, "startSpeed", 0f);
        SetPrivateFiled(forceAdjuster, "force", relativeForce);
        SetCarouselRelativePosition();
        Invoke(nameof(SendStartEventsFroStartFly), 0.1f);
    }

    public void SendStartEventsFroStartFly()
    {
        rotateCarousel.UpdatePositions();
        gameState.SendStartGame();
        gameState.SendStartFly();
    }
    
    [ButtonMethod]
    public void EndBulletFly()
    {
        gameState.SendEndFly();
        gameState.SendEndGame();
        gameState.SendResetGame();
    }

    public void SetCarouselRelativePosition()
    {
        var clockWise = false;
        var animationRelativePosition = 0f;
        if (rightFlyDirection)
        {
            animationRelativePosition = HelperFunctions.
                RangeToRange(relativePosition, -1f, 1f, 0.25f, 0.75f); 
            clockWise = true;
        }
        else
        {
            animationRelativePosition = HelperFunctions.
                RangeToRange(relativePosition, -1f, 1f, 0.75f, 0.25f);
        }

        rotateCarousel.SetDebugValues(animationRelativePosition, clockWise);
        
    }
}
