using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(RandomZoneSpawner))]
public class RandomZoneSpawnerEditor: Editor 
{
    public void OnSceneGUI () 
    {
        var targetRandomZoneSpawner = target as RandomZoneSpawner;
        Handles.color = Color.red;
        Handles.DrawWireDisc(targetRandomZoneSpawner.transform.position,
    targetRandomZoneSpawner.transform.forward, 
           targetRandomZoneSpawner.radius);
        
        targetRandomZoneSpawner.radius = Handles.ScaleValueHandle(targetRandomZoneSpawner.radius,
             targetRandomZoneSpawner.transform.position + new Vector3(targetRandomZoneSpawner.radius,0f,0f),
            Quaternion.identity,
            6.0f,
            Handles.CubeHandleCap,
            1.0f);
    }
}
