using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Obstacle", menuName = "MyScriptableObjects/Obstacle", order = 7)] 
public class ObstacleScriptableObject : ItemScriptableObject
{
    public List<GameObject> gameObjectsToEnable;

    public LevelScriptableObject level;

    public int skullsNumber = 1;

    public bool airObstacle;
}
