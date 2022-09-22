using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using FishNet.Managing.Scened;
using FishNet;

public class PlayerSpawner : NetworkBehaviour
{
    [SyncVar]
    public GameObject spawnedObject;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private bool spawnOnStart = false;
    [SerializeField] private bool spawnWithOwnership = true;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.Respawn_Player, RespawnPlayer);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.Respawn_Player, RespawnPlayer);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (spawnOnStart && !spawnWithOwnership)
        {
            SpawnPlayer();
        }
        else if (spawnOnStart && spawnWithOwnership && IsOwner)
        {
            SpawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        if (spawnedObject != null) return;

        EventSystemNew.RaiseEvent(Event_Type.Player_Respawned);

        SpawnPlayer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayer(NetworkConnection _client = null)
    {
        GameObject go = Instantiate(playerPrefab);

        Spawn(go, _client);

        spawnedObject = go;
    }
}
