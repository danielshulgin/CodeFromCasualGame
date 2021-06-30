using Malee;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

public class TutorialManager : MonoBehaviour
{
    [Reorderable] public TutorialPartReorderableArray tutorialParts;
    
    [SerializeField] private int _tutorialPartIndex;

    [Inject] private PlayerData _playerData;

    
    private void Start()
    {
        for (int i = 0; i < tutorialParts.Count; i++)
        {
            if (!_playerData.PassedTutorialParts.Contains(i))
            {
                _tutorialPartIndex = i;
                break;
            }
        }
        
        BeginNextTutorialPart();
    }

    [ButtonMethod]
    private void BeginNextTutorialPart()
    {
        if (_tutorialPartIndex < tutorialParts.Count && 
            !_playerData.PassedTutorialParts.Contains(_tutorialPartIndex))
        {
            var currentTutorialPart = tutorialParts[_tutorialPartIndex];
            currentTutorialPart.OnEnd += EndTutorialPart;
            currentTutorialPart.Begin();
        }
    }

    private void EndTutorialPart()
    {
        var passedTutorialPart = tutorialParts[_tutorialPartIndex];
        passedTutorialPart.OnEnd -= EndTutorialPart;
        _playerData.HandleTutorialPartPath(_tutorialPartIndex);
        if (passedTutorialPart.ReloadSceneAfterPass)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    [System.Serializable]
    public class TutorialPartReorderableArray : ReorderableArray<TutorialPart> {
    }
}
