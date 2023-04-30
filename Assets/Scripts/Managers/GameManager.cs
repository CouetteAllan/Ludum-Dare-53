using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    StartGame,
    DebutGame,
    InGame,
    Pause,
    Win
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

    public PlayerCharacter Player1data { get; private set; }
    public float ScoreP1 { get; set; }

    public  PlayerCharacter Player2data { get; private set; }
    public float ScoreP2 { get; set; }

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
                break;
        }

        OnStateChanged?.Invoke(CurrentState);
    }

    #region States
    private IEnumerator StartGame()
    {
        //àchanger
        var loadScene = SceneManager.LoadSceneAsync("SceneAllan", LoadSceneMode.Single);
        loadScene.allowSceneActivation = true;
        while (!loadScene.isDone)
        {
            yield return null;
        }
        GlobalTimer = 60.0f;
        ChangeGameState(GameState.DebutGame);
    }


    public void InGame()
    {
        //Resume legame

    }

    public void ToMenu()
    {
        SceneManager.LoadScene("BootScene");
    }

    public void Pause()
    {
        //maybe stop time ?
    }
    #endregion

    public void Update()
    {
        if(CurrentState == GameState.InGame)
        {
            GlobalTimer -= Time.deltaTime;
            if (GlobalTimer <= 0)
                ChangeGameState(GameState.Win);
        }
    }

    public void SetFirstCharacter(PlayerCharacter playerData)
    {
        Player1data = playerData;
    }

    public void SetSecondCharacter(PlayerCharacter playerData)
    {
        Player2data = playerData;
    }

}