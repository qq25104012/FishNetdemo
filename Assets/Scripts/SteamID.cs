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

        if (!IsServer) return;

        Owner.CustomData = SteamClient.SteamId;
    }
}
