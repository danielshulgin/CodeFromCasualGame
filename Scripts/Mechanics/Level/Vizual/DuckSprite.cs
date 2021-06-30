using System;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;


public class DuckSprite : MonoBehaviour
{
    [FormerlySerializedAs("spriteRenderer")] 
    [SerializeField] private SpriteRenderer duckSpriteRenderer;

    [SerializeField] private Animator duckAnimator;

    private void Start()
    {
        PlayerLevels.OnBuyLevel += HandleBuyLevel;
    }

    private void OnDestroy()
    {
        PlayerLevels.OnBuyLevel -= HandleBuyLevel;
    }

    public void Initialize(LevelScriptableObject levelScriptableObject)
    {
        duckSpriteRenderer.gameObject.SetActive(PlayerLevels.Instance.LevelBought(levelScriptableObject));
    }

    private void HandleBuyLevel(LevelScriptableObject levelScriptableObject)
    {
        duckSpriteRenderer.gameObject.SetActive(true);
        duckAnimator.Play("duck_born");
    }
}
