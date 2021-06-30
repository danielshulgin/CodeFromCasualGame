using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class TutorialPart : MonoBehaviour
{
    public Action OnEnd;

    [SerializeField] private bool reloadSceneAfterPass = true;

    public bool ReloadSceneAfterPass => reloadSceneAfterPass;
    
    
    public abstract void Begin();

    protected void SetButtonPosition(Button button, RectTransform targetPosition)
    {
        button.transform.SetParent(targetPosition);
        button.GetComponent<RectTransform>().transform.position = (Vector2)targetPosition.position;
    }
}
