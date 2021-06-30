using System;
using TMPro;
using UnityEngine;


public class GameVersionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;

    private void Awake()
    {
        versionText.text = $"{Application.version}";
    }
}
