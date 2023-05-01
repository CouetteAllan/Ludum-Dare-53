using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    [Header("UI")]
    [Space]
    public TMP_Text timer;

    public Button ResumeButton;
    public Button SettingsButton, MenuButton;

    public GameObject PausePanel, SettingsPanel, UIPanel;

    [Header("Settings")]
    [Space]

    public Button backButton;
    public Slider masterVolume, musicVolume, sfxVolume;

    [Header("End")]
    [Space]

    public GameObject EndPanel;
    public TMP_Text ScoreP1, ScoreP2, whowonText;
    public Button MenuButton2;

    [Header("Tuto")]
    [Space]

    public GameObject TutoPanel;
    bool tutoFlag = false;

    [Header("TargetIndicator")]
    [Space]
    public Canvas canvas;
    public List<TargetIndicator> targetIndicators = new List<TargetIndicator>();
    public Camera MainCamera;
    public GameObject TargetIndicatorPrefab;

    protected override void Awake()
    {
        base.Awake();

        GameManager.OnStateChanged += GameManager_OnStateChanged;

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

        MenuButton2.onClick.AddListener(MainMenu);
    }

    private void Start()
    {
        UIPanel.SetActive(false);
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        TutoPanel.SetActive(false);
        EndPanel.SetActive(false);
    }

    private void GameManager_OnStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                DisplayUI(false);
                break;
            case GameState.InGame:
                DisplayUI(true);
                DisplayTuto(false);
                break;
            case GameState.Pause:
                DisplayPause(true);
                break;
            case GameState.Win:
                Win();
                break;
            case GameState.StartGame:
                break;
            case GameState.DebutGame:
                DisplayUI(true);
                DisplayTuto(true);
                MainCamera = Camera.main;
                break;
        }
    }

    #region display

    private void DisplayPause(bool state)
    {
        PausePanel.SetActive(state);
        EventSystem.current.firstSelectedGameObject = ResumeButton.gameObject;
        ResumeButton.Select();
    }

    private void DisplayUI(bool state)
    {
        UIPanel.SetActive(state);
    }

    private void DisplayTuto(bool state)
    {
        TutoPanel.SetActive(state);
        tutoFlag = true;
    }

    #endregion

    public void Resume()
    {
        //Resume the game when button Resume is clicked
        GameManager.Instance.ChangeGameState(GameState.InGame);
        DisplayPause(false);
    }


    public void MainMenu()
    {
        //Ouvrir la BootScene
        UIPanel.SetActive(false);
        EndPanel.SetActive(false);
        PausePanel.SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.MainMenu);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void Update()
    {
        timer.text = ((int)GameManager.Instance.GlobalTimer).ToString();
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.ChangeGameState(GameState.Pause);
        }

        if (tutoFlag)
        {
            if(Input.anyKeyDown)
            {
                GameManager.Instance.ChangeGameState(GameState.InGame);
                tutoFlag = false;
            }
        }


        if(targetIndicators.Count > 0)
        {
            for(int i = 0; i < targetIndicators.Count; i++)
            {
                targetIndicators[i].UpdateTargetIndicator();
            }
        }
    }

    public void AddTargetIndicator(GameObject target)
    {
        TargetIndicator indicator = GameObject.Instantiate(TargetIndicatorPrefab, canvas.transform).GetComponent<TargetIndicator>();
        indicator.InitialiseTargetIndicator(target, MainCamera, canvas);
        targetIndicators.Add(indicator);
    }

    #region Settings

    public void Settings()
    {
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(true);
        EventSystem.current.firstSelectedGameObject = backButton.gameObject;
        backButton.Select();
    }

    public void Back()
    {
        PausePanel.SetActive(true);
        SettingsPanel.SetActive(false);
        EventSystem.current.firstSelectedGameObject = ResumeButton.gameObject;
        ResumeButton.Select();
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

    public void Win()
    {
        EndPanel.SetActive(true);
        ScoreP1.text = GameManager.Instance.ScoreP1.ToString();
        ScoreP2.text = GameManager.Instance.ScoreP2.ToString();

        EventSystem.current.firstSelectedGameObject = MenuButton2.gameObject;
        MenuButton2.Select();

        if (GameManager.Instance.ScoreP1 > GameManager.Instance.ScoreP2)
        {
            whowonText.text = "Player 1 Wins";
        }
        else if (GameManager.Instance.ScoreP1 < GameManager.Instance.ScoreP2)
        {
            whowonText.text = "Player 2 Wins";
        }
        else
        {
            whowonText.text = "Draw";
        }
    }
}
