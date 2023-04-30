using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiPlayerManager : Singleton<MultiPlayerManager>
{
    private PlayerScript[] players;
    private GameObject[] spawnPoints;

    [SerializeField] private PlayerScript playerPrefab;

    protected override void Awake()
    {
        base.Awake();
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        GameManager.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                break;
            case GameState.InGame:
                SpawnPlayer();
                break;
            case GameState.Pause:
                break;
            case GameState.Win:
                break;
            case GameState.StartGame:
                break;
        }
    }

    private void SpawnPlayer()
    {
        PlayerInputManager.instance.playerPrefab = this.playerPrefab.gameObject;
        for (int i = 0; i < players.Length; i++)
        {
            var playerInput = PlayerInputManager.instance.JoinPlayer(i);
            playerInput.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("player joined: " + playerInput.gameObject.name);
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("player left");
    }
}
