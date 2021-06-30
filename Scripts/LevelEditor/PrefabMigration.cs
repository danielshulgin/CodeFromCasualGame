using System.Collections.Generic;
using MyBox;
using UnityEngine;
using System.Linq;
using UnityEditor;


public class PrefabMigration : MonoBehaviour
{
#if UNITY_EDITOR
    public List<Couple> prefabCouples;
    
    public float x;

    [ButtonMethod]
    public void Migrate()
    {
        var obstaclesIdentifiers = GetComponentsInChildren<ObstacleIdentifier>();
        foreach (var prefabCouple in prefabCouples)
        {
            var obstacleIdentifier = prefabCouple.oldPrefab.GetComponent<ObstacleIdentifier>();
            var oldPrefabs = obstaclesIdentifiers.Where(obstacle =>
                obstacle.GetComponent<ObstacleIdentifier>().ObstacleScriptableObject == obstacleIdentifier.ObstacleScriptableObject);
            foreach (var oldPrefab in oldPrefabs)
            {
                var newPrefabInstance = PrefabUtility.InstantiatePrefab(prefabCouple.newPrefab) as GameObject;
                newPrefabInstance.transform.parent = oldPrefab.gameObject.transform.parent;
                newPrefabInstance.transform.position = oldPrefab.gameObject.transform.position;
                newPrefabInstance.name = oldPrefab.gameObject.name;
                newPrefabInstance.transform.SetSiblingIndex(oldPrefab.transform.GetSiblingIndex());
                DestroyImmediate(oldPrefab.gameObject);
            }
        }
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }

    [ButtonMethod]
    public void ChangeX()
    {
        foreach (Transform chield in gameObject.transform)
        {
            chield.transform.position = chield.transform.position + new Vector3(x, 0f);
        }
    }

    [System.Serializable]
    public class Couple
    {
        public GameObject oldPrefab;

        public GameObject newPrefab;
    }
#endif
}
