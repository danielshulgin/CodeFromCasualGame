using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class BombsStoreUI : MonoBehaviour
{
    [SerializeField] private CannonballScriptableObject bombScriptableObject;
    
    [SerializeField] private TextMeshProUGUI numberInStackText;
    
    [SerializeField] private TextMeshProUGUI priceText;
    
    [SerializeField] private Button buyButton;
   
    [Inject] private PlayerCannonballs _playerBombs;


    private void OnEnable()
    {
        UpdateStoreState();
    }

    private void UpdateStoreState()
    {
        var canBuyOneBomb = _playerBombs.CanBuyBombsStack(bombScriptableObject);
        buyButton.interactable = canBuyOneBomb;
        numberInStackText.text = _playerBombs.GetNumberInStack(bombScriptableObject).ToString();
        priceText.text = bombScriptableObject.price.coins.ToString();
    }

    public void BuyButtonClick()
    {
        _playerBombs.BuyBombsStack(bombScriptableObject);
        UpdateStoreState();
    }   
    
    public void ExitButtonClick()
    {
        gameObject.SetActive(false);
    }  
}
