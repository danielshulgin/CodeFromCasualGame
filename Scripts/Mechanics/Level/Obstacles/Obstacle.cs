using System.Collections;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleSettings obstacleSettings;

    public virtual ObstacleSettings Settings => obstacleSettings;

    public bool Locked { get; private set; }
    

    public void Lock()
    {
        if (!Locked)
        {
            Locked = true;
            StartCoroutine(UnlockRoutine());
        }
    }

    private void OnDisable()
    {
        Locked = false;
    }

    IEnumerator UnlockRoutine()
    {
        yield return new WaitForSeconds(Settings.lockTime);
        Locked = false;
    }
}
