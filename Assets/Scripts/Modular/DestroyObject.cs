using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : NetworkBehaviour
{
    public void DestroyGObject(GameObject _gObj)
    {
        if (!IsServer) return;

        InstanceFinder.ServerManager.Despawn(_gObj);
    }

    public void DestroySpider(GameObject _gObj)
    {
        if (!IsServer) return;

        RPC_PlayerDied(Owner);

        InstanceFinder.ServerManager.Despawn(_gObj);
    }

    [TargetRpc]
    private void RPC_PlayerDied(NetworkConnection conn)
    {
        EventSystemNew.RaiseEvent(Event_Type.Player_Died);
    }
}
