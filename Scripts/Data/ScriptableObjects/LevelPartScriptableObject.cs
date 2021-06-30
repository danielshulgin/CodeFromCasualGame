using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Part", menuName = "MyScriptableObjects/Level Part", order = 2)] 
public class LevelPartScriptableObject : ItemScriptableObject
{
    public GameObject mainMenuBackGroundPrefab;

    public Currency selfAdsRewardIncrease;
    
    public Currency obstacleAdsRewardIncrease;

    public List<ObstacleScriptableObject> increasingRewardObstacles;
    
    public List<ObstacleScriptableObject> obstaclesInStore;
}
