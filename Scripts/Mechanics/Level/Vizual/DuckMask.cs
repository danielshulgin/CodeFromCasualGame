using UnityEngine;


public class DuckMask : MonoBehaviour
{
    private void Start()
    {
        LevelSwitcher.OnStartSwitch += Hide;
        LevelSwitcher.OnCancelSwitch += Show;
        LevelSwitcher.OnEndSwitch += Show;
    }

    private void OnDestroy()
    {
        LevelSwitcher.OnStartSwitch -= Hide;
        LevelSwitcher.OnCancelSwitch -= Show;
        LevelSwitcher.OnEndSwitch -= Show;
    }

    private void Hide(LevelScriptableObject levelScriptableObject)
    {
        gameObject.SetActive(false);
    }

    private void Show(LevelScriptableObject levelScriptableObject)
    {
        gameObject.SetActive(true);
    }
}
