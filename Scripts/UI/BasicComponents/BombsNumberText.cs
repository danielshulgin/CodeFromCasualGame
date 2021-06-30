using System;
using TMPro;
using UnityEngine;
using Zenject;

public class BombsNumberText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    
    [Inject] private PlayerCannonballs _playerBombs;
    

    private void Awake()
    {
        _playerBombs.OnUpdateNumberInStack += HandleUpdaterNumber;
    }

    private void HandleUpdaterNumber(CannonballScriptableObject bombScriptableObject, int number)
    {
        text.text = number.ToString();
    }
}
