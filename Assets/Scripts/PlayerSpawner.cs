using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public class PlayerSpawner : NetworkBehaviour
{
    [SyncVar]
    public GameObject spawnedObject;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject deadHUD;
    [SerializeField] private GameObject deathCam;

    [SerializeField] private bool spawnOnStart = false;

    private void OnEnable()
    {
        EventSystemNew<NetworkConnection>.Subscribe(Event_Type.PlayerDied, PlayerDied);
    }

    private void OnDisable()
    {
        EventSystemNew<NetworkConnection>.Unsubscribe(Event_Type.PlayerDied, PlayerDied);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        Debug.Log("Is Owner: " + IsOwner);

        if (spawnOnStart && IsOwner)
        {
            SpawnPlayer();
        }

        if (!IsOwner)
        {
            Destroy(deadHUD);
            Destroy(deathCam);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayer(NetworkConnection _client = null)
    {
        GameObject go = Instantiate(playerPrefab);

        Spawn(go, _client);

        spawnedObject = go;
    }

    public void RespawnPlayer()
    {
        if (spawnedObject != null) return;

        deadHUD.SetActive(false);
        deathCam.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SpawnPlayer();
    }

    private void PlayerDied(NetworkConnection connection)
    {
        if (IsOwner)
        {
            deadHUD.SetActive(true);
            deathCam.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
