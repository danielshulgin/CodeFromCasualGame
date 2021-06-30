using System;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class SlidingPanel : MonoBehaviour
{
    [SerializeField] private RectTransform panelRect;

    [SerializeField] private float openOffset = 300f;

    [SerializeField] private float moveTime = 1f;
    
    [Inject] private UIStateMachine _uiStateMachine;

    private float _closedOffset;

    private bool _open = false;


    private void Awake()
    {
        _closedOffset = panelRect.anchoredPosition.x;
    }
    
    public void Open()
    {
        if (_open)
        {
            return;
        }
        panelRect.DOAnchorPosX(openOffset, moveTime);
        _open = true;
    }
    
    public void Close()
    {
        if (!_open)
        {
            return;
        }
        panelRect.DOAnchorPosX(_closedOffset, moveTime);
        _open = false;
    }

    public void TongueButtonClick()
    {
        if (_open)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void SettingsButtonClick()
    {
        _uiStateMachine.ShowSettingsUIState();
    }
    
    public void HighScoreButtonClick()
    {
        GooglePlayServices.ShowLeaderboardUI();
    }
}
