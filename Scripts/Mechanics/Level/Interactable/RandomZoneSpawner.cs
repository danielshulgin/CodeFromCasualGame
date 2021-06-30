using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomZoneSpawner : MonoBehaviour
{
    //used in editor script
    public float radius = 2f;
    
    [SerializeField] private GameObject prefab;
    
    [SerializeField] private int num = 1;
    
    [Range(0f,1f)] [SerializeField] private float chance = 1f;

    private List<GameObject> _spawned = new List<GameObject>();

    private void Start()
    {
        ResetSpawn();
        GameState.OnEndGame += ResetSpawn;
    }

    private void OnDestroy()
    {
        GameState.OnEndGame -= ResetSpawn;
    }

    private void ResetSpawn()
    {
        foreach (var gameObject in _spawned)
        {
            Destroy(gameObject);
        }
        for (int i = 0; i < num; i++)
        {
            if (UnityEngine.Random.value <= chance)
            {
                var item = Instantiate(prefab, transform);
                item.transform.position = transform.position +
                    Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0, 360)) 
                    * Vector2.right 
                    * (radius * UnityEngine.Random.value);
                _spawned.Add(item);
            }
        }
    }
}

