using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class SettingsUIState : UIState
{
    [SerializeField] private GameObject blockPanel;
    
    [SerializeField] private Toggle UISoundsToggle;
    
    [SerializeField] private Toggle musicToggle;
        
    [SerializeField] private Toggle gameAudioToggle;
    
    [SerializeField] private Toggle physicVibrationToggle;
    
    [SerializeField] private Toggle bloodToggle;
    
    [SerializeField] private Toggle sightToggle;
    
    [SerializeField] private TMP_Dropdown languageDropDown;
    
    [SerializeField] private int defaultLanguageIndex;

    [SerializeField] private Button signInButton;
    
    [SerializeField] private Button signOutButton;

    [SerializeField] private GameObject infoPanel;
    
    [Inject] private PlayerData _playerData;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void Start()
    {
        base.Start();
        InitializeLanguageDropDawn();
        UpdateToggles();
    }

    private void UpdateToggles()
    {
        gameAudioToggle.SetToggle(PlayerSettings.Instance.GameAudio);
        
        UISoundsToggle.SetToggle(PlayerSettings.Instance.UISounds);
        
        musicToggle.SetToggle(PlayerSettings.Instance.MusicSounds);
        
        physicVibrationToggle.SetToggle(PlayerSettings.Instance.PhysicVibration);
        
        bloodToggle.SetToggle(PlayerSettings.Instance.Blood);
        
        sightToggle.SetToggle(PlayerSettings.Instance.Sight);
    }

    private void InitializeLanguageDropDawn()
    {
        languageDropDown.options = new List<TMP_Dropdown.OptionData>();
        foreach (var language in Localization.Instance.NativeLanguagesNames)
        {
            languageDropDown.options.Add(new TMP_Dropdown.OptionData() {text=language});
        }

        defaultLanguageIndex = 0;
        if (Localization.Instance.Languages.Contains(Application.systemLanguage.ToString()))
        {
            defaultLanguageIndex = Array.IndexOf(Localization.Instance.Languages, 
                Application.systemLanguage.ToString());
        }
        
        var currentLanguageIndex = PlayerPrefs.GetInt("Language", defaultLanguageIndex);
        Localization.Instance.ChangeLanguage(currentLanguageIndex);
        languageDropDown.value = currentLanguageIndex;
        languageDropDown.RefreshShownValue();
        languageDropDown.onValueChanged.AddListener(ChangeLanguage);
    }

    private void ChangeLanguage(int languageIndex)
    {
        Localization.Instance.ChangeLanguage(languageIndex);
        PlayerPrefs.SetInt("Language", languageIndex);
    }
    
    public override void Enter()
    {
        SetActiveCanvasGroup(_canvasGroup, true);
        InitializeSignInOutButton();
    }

    public override void Exit()
    {
        SetActiveCanvasGroup(_canvasGroup, false);
        Destroy(gameObject);//TODO
    }

    public override void ExitButtonClick()
    {
        _uiStateMachine.ShowMainMenu();
    }

    public void TurnUISounds(bool active)
    {
        PlayerSettings.Instance.UISounds = active;
    }
    
    public void TurnMusic(bool active)
    {
        PlayerSettings.Instance.MusicSounds = active;
    }
    
    public void TurnGameAudio(bool active)
    {
        PlayerSettings.Instance.GameAudio = active;
    }
    
    public void TurnPhysicVibration(bool active)
    {
        PlayerSettings.Instance.PhysicVibration = active;
    }
    
    public void TurnBlood(bool active)
    {
        PlayerSettings.Instance.Blood = active;
    }
    
    public void TurnSight(bool active)
    {
        PlayerSettings.Instance.Sight = active;
    }

    public void AboutButtonClick()
    {
        _uiStateMachine.ShowAboutMenu();
    }

    public void ContactUsButtonClick()
    {
        AndroidNativeCalls.ContactUs();
    }
    
    public void OpenInfoPanel()
    {
        infoPanel.SetActive(true);
    }
    
    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }
    
    private void InitializeSignInOutButton()
    {
        if (PlayerSettings.Instance.FirstDeviceFirebaseSaveHappened)
        {
            signInButton.gameObject.SetActive(false);
            signOutButton.gameObject.SetActive(true);
        }
        else
        {
            signInButton.gameObject.SetActive(true);
            signOutButton.gameObject.SetActive(false);
        }
    }

    public void OnSignInButtonClick()
    {
        LoginManager.Instance.StartLateSignInStrategy(HandleSignInResult);
        SetInteractableUI(false);
    }

    public void HandleSignInResult(LateSignInResult result, PlayerDAO player)
    {
        DebugConsole.Instance.Log("HandleSignInResult");
        DebugConsole.Instance.Log(((int)result).ToString());
        switch (result)
        {
            case LateSignInResult.UserHaveAccountWithData:
                InitializeConnectToAccountDialog(player);
                break;
            case LateSignInResult.UserHaveEmptyAccount:
                Invoke(nameof(TurnOnSignInButton), 0.1f);
                break;
            case LateSignInResult.Fault:
                break;
        }
        Invoke(nameof(SetInteractableUI), 0.1f);
    }

    private void InitializeConnectToAccountDialog(PlayerDAO player)
    {
        StartCoroutine(InitializeConnectToAccountDialogRoutine(player));
    }

    IEnumerator InitializeConnectToAccountDialogRoutine(PlayerDAO player)
    {
        yield return null;
        DebugConsole.Instance.Log("InitializeConnectToAccountDialog");
        _uiStateMachine.ConfirmPanel.Initialize( Localization.Instance.GetTranslation("confirm_remote_data"),
            () => HandleConfirmLoadRemoteData(player),
            HandleCancelLoadRemoteData);
    }
    
    private void HandleConfirmLoadRemoteData(PlayerDAO player)
    {
        LoginManager.Instance.RewriteLocalPlayerData(player);
    }

    [ButtonMethod]
    private void HandleCancelLoadRemoteData()
    {
        _uiStateMachine.ConfirmPanel.Initialize(Localization.Instance.GetTranslation("confirm_local_data"), 
            HandleConfirmRewriteRemoteData,
            HandleCancelRewriteRemoteData);
    }
    
    private void HandleConfirmRewriteRemoteData()
    {
        LoginManager.Instance.RewriteRemotePlayerData();
        SetInteractableUI(true);
        Invoke(nameof(TurnOnSignInButton), 0.1f);
    }
    
    private void HandleCancelRewriteRemoteData()
    {
        GooglePlayServices.Instance.SignOutUser();
        PlayerSettings.Instance.UserId = "";
        SetInteractableUI(true);
    }

    public void OnSignOutButtonClick()
    {
        PlayerSettings.Instance.FirstLogin = true;
        GooglePlayServices.Instance.SignOutUser();
        _playerData.LoadDefaults();
    }

    public void TurnOnSignInButton()
    {
        signInButton.gameObject.SetActive(false);
        signOutButton.gameObject.SetActive(true);
    }

    private void SetInteractableUI()
    {
        SetInteractableUI(true);
    }
    
    private void SetInteractableUI(bool interactable)
    {
        blockPanel.SetActive(!interactable);
    }
}
