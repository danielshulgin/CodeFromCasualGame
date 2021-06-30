using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> bloodStainsPrefabs;
    
    [SerializeField] private GameObject bloodParticlesPrefab;
    
    [SerializeField] private GameObject obstacleBloodStainPrefab;
    
    [SerializeField] private GameObject debugStainPrefab;

    [SerializeField] private float enableBloodStainDelay = 0.2f;

    [SerializeField] private float scaleFromSize = 0.7f;
    
    [SerializeField] private float bloodStainScaleDuration = 1f;
    
    [SerializeField] private int stainCountThreshold = 300;
    
    [SerializeField] private int bloodStainsMin = 1;
    
    [SerializeField] private int bloodStainsMax = 3;
    
    [SerializeField] private float bloodStainsMinSize = 0.5f;
    
    [SerializeField] private float bloodStainsMaxSize = 2.5f;
    
    [SerializeField] private float bloodStainsMaxSpawnRadius = 3f;
    
    [SerializeField] private float maxBulletCollisionForce = 30f;
    
    [SerializeField] private float minCollisionForceToSpawnEffects = 4f;
    
    [Range(0f, 1f)] [SerializeField] private float minCollisionSizeK = 0.6f;

    [SerializeField] public bool debugMode = false;
    
    public bool enableEffects = true;
    
    private List<GameObject> _bloodStains;

    
    #region singletone
    
    public static BloodSpawner instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Multiple Blood Spawner instances");
        }
    }
    
    #endregion
    
    private void Start()
    {
        _bloodStains = new List<GameObject>();
        GameState.OnResetGame += Clean;
    }
    
    private void OnDestroy()
    {
        GameState.OnResetGame -= Clean;
    }

    public void HandleBulletCollision(Collision2D collision2D)
    {
        var clampedMagnitude = Mathf.Clamp(collision2D.relativeVelocity.magnitude, 0f, maxBulletCollisionForce);
        var size = HelperFunctions.RangeToRange(clampedMagnitude, 0f, maxBulletCollisionForce, minCollisionSizeK, 1f);

        if (debugMode)
        {
            SpawnDebugParticles(collision2D.GetContact(0).point);
            return;
        }

        if (!PlayerSettings.Instance.Blood)
        {
            return;
        }
        
        if (clampedMagnitude < minCollisionForceToSpawnEffects)
        {
            return;
        }
        SpawnStandardParticles(collision2D.GetContact(0).point, size);
    }
    
    private void SpawnStandardParticles(Vector3 position, float size)
    {
        if (enableEffects)
        {
            var bloodParticles = Instantiate(bloodParticlesPrefab, transform);
            bloodParticles.transform.position = new Vector3(position.x, position.y, position.z - 1f);
            if (bloodStainsPrefabs.Count >= 0)
            {
                if (_bloodStains.Count > stainCountThreshold)
                {
                    var oldestStain = _bloodStains.First();
                    _bloodStains.Remove(oldestStain);
                    Destroy(oldestStain);
                }
                StartCoroutine(EnableBloodStain(obstacleBloodStainPrefab, bloodParticles.transform.position,
                    Vector3.one * size));
                
                var stainPrefab = bloodStainsPrefabs[UnityEngine.Random.Range(0, bloodStainsPrefabs.Count)];

                var bloodStainsNumber = Random.Range(bloodStainsMin, bloodStainsMax + 1);
                for (int i = 0; i < bloodStainsNumber; i++)
                {
                    var bloodStainsOffset = new Vector3(Random.Range(-bloodStainsMaxSpawnRadius, bloodStainsMaxSpawnRadius),
                        Random.Range(-bloodStainsMaxSpawnRadius, bloodStainsMaxSpawnRadius),0f);
                    var bloodStainsSize = Random.Range(bloodStainsMinSize, bloodStainsMaxSize) * size;
                    
                    StartCoroutine(EnableBloodStain(stainPrefab, bloodParticles.transform.position + bloodStainsOffset,
                        new Vector3(bloodStainsSize, bloodStainsSize, 1f)));
                }
            }
        }
    }

    private void SpawnDebugParticles(Vector3 position)
    {
        var debugBloodStain = Instantiate(debugStainPrefab, transform);
        debugBloodStain.transform.position = position;
        _bloodStains.Add(debugBloodStain);
    }

    public void Clean()
    {
        StopAllCoroutines();
        foreach (var bloodStain in _bloodStains) {
            GameObject.Destroy(bloodStain);
        }
        _bloodStains = new List<GameObject>();
    }
    
    IEnumerator EnableBloodStain(GameObject stainPrefab, Vector3 position, Vector3 prefabScale)
    {
        yield return new WaitForSeconds(enableBloodStainDelay);
        var bloodStain = Instantiate(stainPrefab, transform);
        _bloodStains.Add(bloodStain);
        
        bloodStain.transform.localScale = prefabScale * scaleFromSize;
        
        bloodStain.transform.position = new Vector3(position.x, position.y);
        
        StartCoroutine(ScaleBloodStain(bloodStain.transform, prefabScale, bloodStainScaleDuration));
    }

    IEnumerator ScaleBloodStain(Transform targetTransform, Vector3 finalScale, float duration)
    {
        var startScale = targetTransform.localScale;
        var scaleDelta = finalScale - startScale;
        
        for (var timeFromStart = 0f; timeFromStart < duration; timeFromStart += Time.deltaTime)
        {
            if (targetTransform != null)
            {
                targetTransform.localScale = startScale + scaleDelta * timeFromStart / duration;
            }
            yield return null;
        }
    }
}
