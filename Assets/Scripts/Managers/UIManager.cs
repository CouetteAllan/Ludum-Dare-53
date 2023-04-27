using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.InGame:
                //Enlever la pause
                break;
            case GameState.Pause:
                //Afficher la pause
                break;
        }
    }

    private void DisplayPause(bool state)
    {
        //"pause panel" .SetActive(state);
    }

    public void ResumeButton()
    {
        //Resume the game when button Resume is clicked
        GameManager.Instance.ChangeGameState(GameState.InGame);
    }

    public void MainMenuButton()
    {
        //Ouvrir la BootScene
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
