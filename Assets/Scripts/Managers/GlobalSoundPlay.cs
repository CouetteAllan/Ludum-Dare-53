using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSoundPlay : MonoBehaviour
{
    private SoundManager soundManager;
    private bool needCheck = true;
    private bool firstTime = true;
    public void Init()
    {
        soundManager = SoundManager.Instance;
        PlayerScript.OnPlayerScore += PlayerScript_OnPlayerScore;
        GameManager.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.DebutGame:
                soundManager.StopAllMusics();
                soundManager.PlayMusic("Game1");
                break;
            case GameState.Win:
                soundManager.StopAllMusics();
                soundManager.PlayMusic("Victory");
                break;
            case GameState.MainMenu:
                soundManager.StopAllMusics();
                soundManager.PlayMusic("MainMenuMusic");
                break;
        }
    }

    private void PlayerScript_OnPlayerScore(int playerIndex)
    {
        ChangeMusicLayer(BuildingManager.Instance.BuildingsColored.Count);
        soundManager.Play("Score");
    }

    private void ChangeMusicLayer(int coloredBuildingsCount)
    {
        if (!NeedCheck())
            return;

        if(coloredBuildingsCount >= BuildingManager.Instance.BuildingCount)
        {
            soundManager.StopAllMusics();
            soundManager.PlayMusic("Game4");
            soundManager.PlayMusic("Game3");
            needCheck = false;
            return;
        }


        
        if (coloredBuildingsCount == 4)
        {
            soundManager.StopAllMusics();
            soundManager.PlayMusic("Game3");
        }
        else
        {
            soundManager.StopAllMusics();
            soundManager.PlayMusic("Game2");
            firstTime = false;
        }
    }

    private bool NeedCheck() => firstTime ||
        BuildingManager.Instance.BuildingsColored.Count == 4 ||
        (BuildingManager.Instance.BuildingsColored.Count == BuildingManager.Instance.BuildingCount && needCheck);

    private void OnDisable()
    {
        PlayerScript.OnPlayerScore -= PlayerScript_OnPlayerScore;
        GameManager.OnStateChanged -= GameManager_OnStateChanged;
    }
}
