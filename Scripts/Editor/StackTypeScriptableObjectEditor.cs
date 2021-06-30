﻿using System;
using System.Diagnostics;
using UnityEditor;
using Unity;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CustomEditor(typeof(StackTypeScriptableObject), true)]
public class StackTypeScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var targetStackTypeScriptableObject = target as StackTypeScriptableObject;
        GUILayout.BeginHorizontal();
        var path = "Assets/Resources/ProjectContext.prefab";
        if(GUILayout.Button("Add to db"))
        {
            var projectContext = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            AssetDatabase.StartAssetEditing();
            var scriptableObjectDataBase = projectContext
                .GetComponent<ScriptableObjectDataBase>();
            scriptableObjectDataBase.AddStackTypeScriptableObject(targetStackTypeScriptableObject);
            EditorUtility.SetDirty(scriptableObjectDataBase);
            AssetDatabase.StopAssetEditing();
        }
        if(GUILayout.Button("Remove from db"))
        {
            var projectContext = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            AssetDatabase.StartAssetEditing();
            var scriptableObjectDataBase = projectContext
                .GetComponent<ScriptableObjectDataBase>();
            scriptableObjectDataBase.RemoveStackTypeScriptableObject(targetStackTypeScriptableObject);
            AssetDatabase.StopAssetEditing();
        }
        GUILayout.EndHorizontal();
    }
    
}