using UnityEngine;
using UnityEngine.Serialization;

public class ObstacleIdentifier : MonoBehaviour
{
    [SerializeField] private ObstacleScriptableObject obstacleScriptableObject;

    public ObstacleScriptableObject ObstacleScriptableObject => obstacleScriptableObject;
}
