using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header ("Start")]
    [Space]
    public GameObject StartPanel;
    public Button StartButton, SettingsButton, QuitButton;

    [Header("Settings")]
    [Space]
    public GameObject SettingsPanel;
    public Button BackButton1;
    public Slider masterVolume, musicVolume, sfxVolume;

    [Header("CharacterSelect")]
    [Space]
    public GameObject CharacterSelectPanel;
    public Button PlayButton,BackButton2;

    private int currentP1;
    private int currentP2;
    public List<Sprite> spriteListP1;
    public GameObject spriteP1;
    public List<Sprite> spriteListP2;
    public GameObject spriteP2;
    public Button LeftButtonP1, RightButtonP1;
    public Button LeftButtonP2, RightButtonP2;

    private void Awake()
    {
        StartPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        CharacterSelectPanel.SetActive(false);

        StartButton.onClick.AddListener(CharacterSelect);
        SettingsButton.onClick.AddListener(Settings);
        QuitButton.onClick.AddListener(QuitGame);

        BackButton1.onClick.AddListener(Back);
        masterVolume.onValueChanged.AddListener(setMasterVolume);
        musicVolume.onValueChanged.AddListener(setMusicVolume);
        sfxVolume.onValueChanged.AddListener(setSFXVolume);

        BackButton2.onClick.AddListener(Back);
        LeftButtonP1.onClick.AddListener(LeftP1);
        RightButtonP1.onClick.AddListener(RightP1);
        LeftButtonP2.onClick.AddListener(LeftP2);
        RightButtonP2.onClick.AddListener(RightP2);
        PlayButton.onClick.AddListener(StartGame);
    }

    private void Start()
    {
        masterVolume.value = SoundManager.Instance.masterVolume;
        musicVolume.value = SoundManager.Instance.musicVolume;
        sfxVolume.value = SoundManager.Instance.sfxVolume;
    }

    #region Start

    private void CharacterSelect()
    {
        StartPanel.SetActive(false);
        CharacterSelectPanel.SetActive(true);
    }

    private void Settings()
    {
        StartPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    #endregion

    #region Settings

    private void Back()
    {
        StartPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        CharacterSelectPanel.SetActive(false);
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

    #region CharacterSelect

    private void StartGame()
    {
        GameManager.Instance.ChangeGameState(GameState.StartGame);
    }

    private void LeftP1()
    {
        if (currentP1 == 0)
            currentP1 = spriteListP1.Count-1;
        else
            currentP1--;
        spriteP1.GetComponent<Image>().sprite = spriteListP1[currentP1];
    }

    private void RightP1()
    {
        if (currentP1 == spriteListP1.Count-1)
            currentP1 = 0;
        else
            currentP1++;
        spriteP1.GetComponent<Image>().sprite = spriteListP1[currentP1];
    }

    private void LeftP2()
    {
        if (currentP2 == 0)
            currentP2 = spriteListP2.Count - 1;
        else
            currentP2--;
        spriteP2.GetComponent<Image>().sprite = spriteListP2[currentP2];
    }

    private void RightP2()
    {
        if (currentP2 == spriteListP2.Count - 1)
            currentP2 = 0;
        else
            currentP2++;
        spriteP2.GetComponent<Image>().sprite = spriteListP2[currentP2];
    }

    #endregion

}
