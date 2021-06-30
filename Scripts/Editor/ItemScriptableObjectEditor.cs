using System;
using System.Diagnostics;
using UnityEditor;
using Unity;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CustomEditor(typeof(ItemScriptableObject), true)]
public class ItemScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var targetItemScriptableObject = target as ItemScriptableObject;
        GUILayout.BeginHorizontal();
        var path = "Assets/Resources/ProjectContext.prefab";
        if(GUILayout.Button("Add to db"))
        {
            var projectContext = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            AssetDatabase.StartAssetEditing();
            var scriptableObjectDataBase = projectContext
                .GetComponent<ScriptableObjectDataBase>();
            scriptableObjectDataBase.AddScriptableObject(targetItemScriptableObject);
            EditorUtility.SetDirty(scriptableObjectDataBase);
            AssetDatabase.StopAssetEditing();
        }
        if(GUILayout.Button("Remove from db"))
        {
            var projectContext = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            AssetDatabase.StartAssetEditing();
            var scriptableObjectDataBase = projectContext
                .GetComponent<ScriptableObjectDataBase>();
            scriptableObjectDataBase.RemoveScriptableObject(targetItemScriptableObject);
            AssetDatabase.StopAssetEditing();
        }
        GUILayout.EndHorizontal();
    }
}
