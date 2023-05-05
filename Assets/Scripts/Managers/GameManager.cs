using System;
using System.Collections;
using System.Threading.Tasks;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    StartGame,
    DebutGame,
    InGame,
    Pause,
    Win,
    DrawState
    //...
}

public class GameManager : Singleton<GameManager>
{
    #region Static Events
    /// <summary>
    /// This event fires before the changing of the state. It takes in parameters the next game state.
    /// </summary>
    public static event Action<GameState> OnBeforeStateChange;
    /// <summary>
    /// This event fires after the game state has been changed. It takes as a parameters the new game state.
    /// </summary>
    public static event Action<GameState> OnStateChanged;
    #endregion

    #region Variables
    public GameState CurrentState { get; private set; }

    public float GlobalTimer { get; private set; }

    public SelectedCharacterData Player1data { get; private set; }
    public float ScoreP1 { get; set; }

    public  SelectedCharacterData Player2data { get; private set; }
    public float ScoreP2 { get; set; }

    public SelectedCharacterData Winner { get; private set; }
    public int WinnerIndex { get; private set; }

    public bool IsDraw { get; private set; } = false;

    #endregion

    /// <summary>
    /// Called this method when you want to change the game state of the game and fire event when the state is changing
    /// </summary>
    /// <param name="newState"> The state you want the current game state to be</param>
    public void ChangeGameState(GameState newState)
    {
        if (newState == CurrentState)
            return;
        OnBeforeStateChange?.Invoke(newState);
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.MainMenu:
                ToMenu();
                break;
            case GameState.StartGame:
                StartCoroutine(StartGame());
                break;
            case GameState.DebutGame:
                //Pour init les joueurs
                break;
            case GameState.InGame:
                InGame();
                break;
            case GameState.Pause:
                Pause();
                break;
            case GameState.Win:
                SetWinner();
                //Choisir le vainqueur du jeu
                break;
            case GameState.DrawState:
                IsDraw = true;
                break;
        }

        OnStateChanged?.Invoke(CurrentState);
    }

    #region States
    private IEnumerator StartGame()
    {
        //àchanger
        var loadScene = SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
        loadScene.allowSceneActivation = true;
        while (!loadScene.isDone)
        {
            yield return null;
        }
        GlobalTimer = 180.0f;
        ScoreP1 = 0f;
        ScoreP2 = 0f;
        ChangeGameState(GameState.DebutGame);
    }


    private void InGame()
    {
        //Resume legame
        Time.timeScale = 1.0f;
    }

    private void ToMenu()
    {
        SceneManager.LoadScene("BootScene");
        Time.timeScale = 1.0f;
    }

    private void Pause()
    {
        //maybe stop time ?
        Time.timeScale = 0.0f;

    }

    private void SetWinner()
    {
        if(ScoreP1 == ScoreP2)
        {
            ChangeGameState(GameState.DrawState);
            return;
        }

        if (ScoreP1 > ScoreP2)
        {
            Winner = Player1data;
            WinnerIndex = 0; //Joueur 1
        }
        else if (ScoreP1 < ScoreP2)
        {
            Winner = Player2data;
            WinnerIndex = 1; //Joueur 2
        }

        Time.timeScale = 0.5f;
    }
    #endregion

    public void Update()
    {
        if (IsDraw)
            return;
        if(CurrentState == GameState.InGame)
        {
            GlobalTimer -= Time.deltaTime;
            if (GlobalTimer <= 0)
                ChangeGameState(GameState.Win);
        }
    }

    public void SetFirstCharacter(SelectedCharacterData playerData)
    {
        Player1data = playerData;
    }

    public void SetSecondCharacter(SelectedCharacterData playerData)
    {
        Player2data = playerData;
    }

}