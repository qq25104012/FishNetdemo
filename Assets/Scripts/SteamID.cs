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

        RPC_SetSteamID(SteamClient.SteamId, SteamClient.Name);
    }

    [ServerRpc]
    public void RPC_SetSteamID(SteamId _steamID, string _steamName)
    {
        SteamData steamData = new SteamData(_steamID, _steamName);
        Owner.CustomData = steamData;
    }
}

public class SteamData
{
    public SteamId steamID;
    public string steamName;

    public SteamData(SteamId _steamID, string _steamName)
    {
        steamID = _steamID;
        steamName = _steamName;
    }
}
