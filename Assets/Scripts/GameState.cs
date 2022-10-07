using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class GameState : NetworkBehaviour
{
    public static GameState Instance { get; private set; }

    public bool gameStarted { get { return _gameStarted; } }
    public bool gameEnded { get { return _gameEnded; } }

    [SyncVar(OnChange = nameof(SyncStartGame))]
    private bool _gameStarted = false;

    [SyncVar(OnChange = nameof(SyncEndGame))]
    private bool _gameEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (!IsServer) return;
            InstanceFinder.ServerManager.Despawn(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!_gameStarted && !_gameEnded)
            {
                Debug.Log("Start Game");

                RPC_StartGame();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RPC_StartGame()
    {
        _gameStarted = true;
    }

    public void GameEnded()
    {
        if (!IsServer) return;

        _gameStarted = false;

        _gameEnded = true;
    }

    private void SyncEndGame(bool prev, bool next, bool asServer)
    {
        if (asServer) return;

        _gameEnded = next;

        if (_gameEnded)
        {
            _gameStarted = false;

            EventSystemNew.RaiseEvent(Event_Type.GAME_ENDED);
        }
    }

    private void SyncStartGame(bool prev, bool next, bool asServer)
    {
        //if (asServer) return;

        Debug.Log("Game Started: " + next);

        _gameStarted = next;

        if (_gameStarted)
        {
            Debug.Log("Game Started");

            _gameEnded = false;

            EventSystemNew.RaiseEvent(Event_Type.GAME_STARTED);
        }
    }
}
