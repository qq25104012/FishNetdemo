using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public class Nickname : NetworkBehaviour
{
    [SyncVar(OnChange = nameof(SyncNickname))]
    public string nickname;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            RPC_SyncNickname(SteamClient.Name);
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void RPC_SyncNickname(string _nickname)
    {
        nickname = _nickname;
    }

    private void SyncNickname(string prev, string next, bool asServer)
    {
        nickname = next;
    }
}
