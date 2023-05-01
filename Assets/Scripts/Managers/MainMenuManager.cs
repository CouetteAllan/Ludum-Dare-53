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

    private int currentP1, currentColP1;
    private int currentP2, currentColP2;
    public List<PlayerCharacter> characterDataList;
    public List<Chroma> chromaList;
    public GameObject spriteP1;
    public GameObject colorP1;
    public GameObject spriteP2;
    public GameObject colorP2;
    public Button LeftButtonP1, RightButtonP1, LeftColorButtonP1, RightColorButtonP1;
    public Button LeftButtonP2, RightButtonP2, LeftColorButtonP2, RightColorButtonP2;

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
        LeftColorButtonP1.onClick.AddListener(LeftColorP1);
        RightColorButtonP1.onClick.AddListener(RightColorP1);
        LeftColorButtonP2.onClick.AddListener(LeftColorP2);
        RightColorButtonP2.onClick.AddListener(RightColorP2);
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
        currentColP1 = 0;
        currentColP2 = 1;
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

        var p1Datas = new SelectedCharacterData
        {
            animController = characterDataList[currentP1].animController,
            characterType = characterDataList[currentP1].characterType,
            spriteColor = SetColor(chromaList[currentColP1]),
        };
        gameManager.SetFirstCharacter(p1Datas);

        var p2Datas = new SelectedCharacterData
        {
            animController = characterDataList[currentP2].animController,
            characterType = characterDataList[currentP2].characterType,
            spriteColor = SetColor(chromaList[currentColP2]),
        };
        gameManager.SetSecondCharacter(p2Datas);
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

    private void LeftColorP1()
    {
        if (currentColP1 == 0)
            currentColP1 = chromaList.Count - 1;
        else
            currentColP1--;
        colorP1.GetComponent<Image>().color = SetColor(chromaList[currentColP1]);
        PlayButton.interactable = IsPlayButtonEnabled();
    }

    private void RightColorP1()
    {
        if (currentColP1 == chromaList.Count - 1)
            currentColP1 = 0;
        else
            currentColP1++;
        colorP1.GetComponent<Image>().color = SetColor(chromaList[currentColP1]);
        PlayButton.interactable = IsPlayButtonEnabled();
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

    private void LeftColorP2()
    {
        if (currentColP2 == 0)
            currentColP2 = chromaList.Count - 1;
        else
            currentColP2--;
        colorP2.GetComponent<Image>().color = SetColor(chromaList[currentColP2]);
        PlayButton.interactable = IsPlayButtonEnabled();
    }
    private void RightColorP2()
    {
        if (currentColP2 == chromaList.Count - 1)
            currentColP2 = 0;
        else
            currentColP2++;
        colorP2.GetComponent<Image>().color = SetColor(chromaList[currentColP2]);
        PlayButton.interactable = IsPlayButtonEnabled();
    }


    private const float COLOR_RATIO = 255.0f;
    private Color SetColor(Chroma chroma) => chroma switch
    {
        Chroma.Blue => new Color(103.0f/ COLOR_RATIO, 165.0f/ COLOR_RATIO, 255.0f/ COLOR_RATIO, 1),
        Chroma.Orange => new Color(255/ COLOR_RATIO, 143/ COLOR_RATIO, 50/ COLOR_RATIO,1),
        Chroma.Yellow => new Color(251 / COLOR_RATIO, 255 / COLOR_RATIO, 119 / COLOR_RATIO,1),
        Chroma.Pink => new Color(240 / COLOR_RATIO, 62 / COLOR_RATIO, 255 / COLOR_RATIO, 1),
        Chroma.Green => new Color(78 / COLOR_RATIO, 250 / COLOR_RATIO, 115 / COLOR_RATIO, 1),
    };

    private bool IsPlayButtonEnabled() => currentColP1 != currentColP2;

    #endregion
}
