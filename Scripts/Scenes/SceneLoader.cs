using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private int mainSceneIndex = 1;

    [SerializeField] private TextMeshProUGUI percentageText;
    
    [SerializeField] private RectTransform sliderRectTransform;
    
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            Debug.Log("Multiple SceneLoader instances");
        }
    }

    public void LoadMainSceneAsync()
    {
        StartCoroutine(LoadAsync(mainSceneIndex));
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        yield return null;
        var loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!loadOperation.isDone)
        {
            var progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            
            percentageText.text = Mathf.CeilToInt(progress * 100f) + "%";
            
            sliderRectTransform.localScale = new Vector3(progress, 1f, 1f);
            Debug.Log(progress);
            yield return null;
        }
    }
}
