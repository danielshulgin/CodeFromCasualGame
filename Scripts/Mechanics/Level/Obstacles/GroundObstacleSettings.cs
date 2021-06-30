using UnityEngine;


[CreateAssetMenu(fileName = "ObstacleSettings", menuName = "MyScriptableObjects/GroundObstacleSettings", order = 0)] 
public class GroundObstacleSettings : ObstacleSettings
{
    public float width = 5f;

    public float offset = 0f;
    
    public Vector2 velocityMultiplier = new Vector2(1f, 1f);

    public bool enableMinVelocityConstrain;    
    
    [MyBox.ConditionalField(nameof(enableMinVelocityConstrain))]
    public Vector2 minVelocity = Vector2.zero;

    [Header("Debug values")] 
    public Color debugLineColor;

    public float debugLineThickness;
}
