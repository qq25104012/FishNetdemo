using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using FishNet.Managing.Scened;

public class PlayerSpawner : NetworkBehaviour
{
    [SyncVar]
    public GameObject spawnedObject;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private bool spawnOnStart = false;

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

        Debug.Log("Is Owner: " + IsOwner);

        if (spawnOnStart && IsOwner)
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
