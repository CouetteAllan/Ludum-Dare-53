using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiPlayerManager : Singleton<MultiPlayerManager>
{
    private List<PlayerScript> players = new List<PlayerScript>();
    public List<PlayerScript> Players { get { return players; } }
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
        switch (state)
        {
            case GameState.MainMenu:
                break;
            case GameState.InGame:
                break;
            case GameState.Pause:
                break;
            case GameState.Win:
                break;
            case GameState.StartGame:
                break;
            case GameState.DebutGame:
                SpawnPlayer();
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
            var playerRef = playerInput.gameObject.GetComponent<PlayerScript>();
            if(i == 0)
                playerRef.Init(GameManager.Instance.Player1data);
            else if (i == 1)
            {
                playerRef.Init(GameManager.Instance.Player2data);
            }
            players.Add(playerRef);
        }
        GameManager.Instance.ChangeGameState(GameState.InGame);
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("player joined: " + playerInput.gameObject.name);
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
