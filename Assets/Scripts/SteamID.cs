using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamID : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner) return;

        RPC_SetSteamID(SteamClient.SteamId);
    }

    [ServerRpc]
    public void RPC_SetSteamID(SteamId _steamID)
    {
        Owner.CustomData = _steamID;

        Debug.Log("Custom Data: " + Owner.CustomData);
    }
}
