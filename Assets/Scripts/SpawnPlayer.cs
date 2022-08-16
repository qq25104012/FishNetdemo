using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class SpawnPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerSpawn(NetworkConnection _client = null)
    {
        GameObject go = Instantiate(playerPrefab);

        Spawn(go, _client);
    }
}
