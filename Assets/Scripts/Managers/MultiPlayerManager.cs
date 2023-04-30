using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiPlayerManager : Singleton<MultiPlayerManager>
{
    private List<PlayerScript> players = new List<PlayerScript>();
    public List<PlayerScript> Players { get { return players; } };
    private GameObject[] spawnPoints;

    [SerializeField] private PlayerScript playerPrefab;

    protected override void Awake()
    {
        base.Awake();
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    private void Start()
    {
        GameManager.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameState state)
    {
        Debug.Log("c'est bon pour vous ?");
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

    public void SpawnPlayer()
    {
        Debug.Log("oui le spawn");
        PlayerInputManager.instance.playerPrefab = this.playerPrefab.gameObject;
        for (int i = 0; i < PlayerInputManager.instance.maxPlayerCount; i++)
        {
            var playerInput = PlayerInputManager.instance.JoinPlayer(i);
            playerInput.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("player joined: " + playerInput.gameObject.name);
        players.Add(playerInput.gameObject.GetComponent<PlayerScript>());
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("player left");
        players.Remove(playerInput.gameObject.GetComponent<PlayerScript>());
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= GameManager_OnStateChanged;

    }
}
