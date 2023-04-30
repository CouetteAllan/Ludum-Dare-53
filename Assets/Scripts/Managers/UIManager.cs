using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    public TMP_Text timer;

    public Button ResumeButton;
    public Button SettingsButton, MenuButton;

    public GameObject PausePanel, SettingsPanel;

    [Header("Settings")]
    [Space]

    public Button backButton;
    public Slider masterVolume, musicVolume, sfxVolume;

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnStateChanged += GameManager_OnStateChanged;

        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);

        ResumeButton.onClick.AddListener(Resume);
        SettingsButton.onClick.AddListener(Settings);
        MenuButton.onClick.AddListener(MainMenu);

        backButton.onClick.AddListener(Back);

        masterVolume.onValueChanged.AddListener(setMasterVolume);
        masterVolume.value = SoundManager.Instance.masterVolume;
        musicVolume.onValueChanged.AddListener(setMusicVolume);
        musicVolume.value = SoundManager.Instance.musicVolume;
        sfxVolume.onValueChanged.AddListener(setSFXVolume);
        sfxVolume.value = SoundManager.Instance.sfxVolume;
    }

    private void GameManager_OnStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.InGame:
                break;
            case GameState.Pause:
                DisplayPause(true);
                break;
        }
    }

    private void DisplayPause(bool state)
    {
        PausePanel.SetActive(state);
    }

    public void Resume()
    {
        //Resume the game when button Resume is clicked
        GameManager.Instance.ChangeGameState(GameState.InGame);
        DisplayPause(false);
    }


    public void MainMenu()
    {
        //Ouvrir la BootScene
        GameManager.Instance.ChangeGameState(GameState.MainMenu);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void Update()
    {
        timer.text = ((int)GameManager.Instance.GlobalTimer).ToString();
    }

    #region Settings

    public void Settings()
    {
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void Back()
    {
        PausePanel.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    private void setMasterVolume(float i)
    {
        SoundManager.Instance.ModifyAllVolume(i);
    }

    private void setMusicVolume(float i)
    {
        SoundManager.Instance.ModifyMusicVolume(i);
    }

    private void setSFXVolume(float i)
    {
        SoundManager.Instance.ModifySFXVolume(i);
    }

    #endregion
}
