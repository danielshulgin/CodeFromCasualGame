using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelComponent : MonoBehaviour
{
    public List<LevelPart> levelParts;
        
    public List<ObstacleType> obstacleTypes;

#if UNITY_EDITOR
    [ButtonMethod]
    public void AddObstaclesToList()
    {
        var obstacles = GetComponentsInChildren<ObstacleIdentifier>().ToList();
        foreach (var obstacleType in obstacleTypes)
        {
            obstacleType.gameObjectsToActivate = new List<GameObject>();
            obstacleType.gameObjectsToActivate
                .AddRange(obstacles
                    .FindAll(id => id.ObstacleScriptableObject == obstacleType.obstacleScriptableObject)
                .Select(id => id.gameObject));
        }
        
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }
#endif
    
    [System.Serializable]
    public class LevelPart
    {
        public LevelPartScriptableObject levelPartScriptableObject;
        
        [FormerlySerializedAs("Walls")] 
        public List<GameObject> walls;
    }
    
    [System.Serializable]
    public class ObstacleType
    {
        [FormerlySerializedAs("trapScriptableObject")] 
        public ObstacleScriptableObject obstacleScriptableObject;

        [FormerlySerializedAs("GameObjectsToActivate")] 
        public List<GameObject> gameObjectsToActivate;
    }
}
