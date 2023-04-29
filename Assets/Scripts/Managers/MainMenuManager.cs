using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject StartPanel;
    public Button StartButton, SettingsButton, QuitButton;

    public GameObject SettingsPanel;
    public Button BackButton1;

    public GameObject CharacterSelectPanel;
    public List<Sprite> spriteList;
    public GameObject sprite;
    private int current;
    public Button PlayButton, LeftButton, RightButton, BackButton2;

    private void Awake()
    {
        StartPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        CharacterSelectPanel.SetActive(false);

        StartButton.onClick.AddListener(CharacterSelect);
        SettingsButton.onClick.AddListener(Settings);
        QuitButton.onClick.AddListener(QuitGame);

        BackButton1.onClick.AddListener(Back);
        BackButton2.onClick.AddListener(Back);

        LeftButton.onClick.AddListener(Left);
        RightButton.onClick.AddListener(Right);
        PlayButton.onClick.AddListener(StartGame);
    }

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

    private void Back()
    {
        StartPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        CharacterSelectPanel.SetActive(false);
    }

    private void StartGame()
    {
        GameManager.Instance.ChangeGameState(GameState.StartGame);
    }

    private void Left()
    {
        if (current == 0)
            current = spriteList.Count-1;
        else
            current--;
        sprite.GetComponent<SpriteRenderer>().sprite = spriteList[current];
    }

    private void Right()
    {
        if (current == spriteList.Count-1)
            current = 0;
        else
            current++;
        sprite.GetComponent<SpriteRenderer>().sprite = spriteList[current];
    }
}
