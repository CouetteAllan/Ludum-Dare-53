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
    public List<PlayerCharacter> characterDataList;
    public GameObject spriteP1;
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
        currentP2 = 1;
        currentP1 = 0;
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
        var gameManager = GameManager.Instance;
        //Set up les characters data séléctionnés

        gameManager.SetFirstCharacter(characterDataList[currentP1]);
        gameManager.SetSecondCharacter(characterDataList[currentP2]);
        gameManager.ChangeGameState(GameState.StartGame);
    }

    private void LeftP1()
    {
        if (currentP1 == 0)
            currentP1 = characterDataList.Count-1;
        else
            currentP1--;
        spriteP1.GetComponent<Image>().sprite = characterDataList[currentP1].art;
    }

    private void RightP1()
    {
        if (currentP1 == characterDataList.Count-1)
            currentP1 = 0;
        else
            currentP1++;
        spriteP1.GetComponent<Image>().sprite = characterDataList[currentP1].art;
    }

    private void LeftP2()
    {
        if (currentP2 == 0)
            currentP2 = characterDataList.Count - 1;
        else
            currentP2--;
        spriteP2.GetComponent<Image>().sprite = characterDataList[currentP2].art;
    }

    private void RightP2()
    {
        if (currentP2 == characterDataList.Count - 1)
            currentP2 = 0;
        else
            currentP2++;
        spriteP2.GetComponent<Image>().sprite = characterDataList[currentP2].art;
    }

    #endregion
}
