using System.Collections.Generic;
using Malee;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Level", menuName = "MyScriptableObjects/Level", order = 6)] 
public class LevelScriptableObject : ItemScriptableObject
{
    public int levelNumber;
    
    public List<LevelPartScriptableObject> levelPartScriptableObjects;

    public GameObject levelPrefab;

    public override string StoreName => Localization.Instance.GetTranslation("level_store_name") + levelNumber;
}