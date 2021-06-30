using System.Collections.Generic;
using UnityEngine;

public class LevelMask : MonoBehaviour
{
    [SerializeField] private List<GameObject> parts;
    
    [SerializeField] private GameObject fadePanel;

    
    public void SetFadeActive(bool active)
    {
        fadePanel.SetActive(active);
    }
    
    private void Awake()
    {
        LevelSwitcher.OnEndSwitch += TurnOffMask;
        LevelSwitcher.OnCancelSwitch += TurnOffMask;
    }

    private void OnDestroy()
    {
        LevelSwitcher.OnEndSwitch -= TurnOffMask;
        LevelSwitcher.OnCancelSwitch -= TurnOffMask;
    }

    private void TurnOffMask(LevelScriptableObject levelScriptableObject)
    {
        SwitchMask(SpriteMaskInteraction.None);
    }

    public void SwitchMask(SpriteMaskInteraction spriteMaskInteraction)
    {
        foreach (var backGroundPart in parts)
        {
            backGroundPart.GetComponent<SpriteRenderer>().maskInteraction = spriteMaskInteraction;
        }
    }
}
