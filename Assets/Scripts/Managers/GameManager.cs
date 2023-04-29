using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    InGame,
    Pause,
    Win,
    StartGame
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

    public int ScoreP1 { get; set; }
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
            case GameState.InGame:
                break;
            case GameState.Pause:
                break;
        }

        OnStateChanged?.Invoke(CurrentState);
    }

}
