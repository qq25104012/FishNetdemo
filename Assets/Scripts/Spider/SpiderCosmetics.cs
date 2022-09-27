using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class SpiderCosmetics : NetworkBehaviour
{
    [SerializeField] GameObject[] hatCosmetics;

    [SyncVar(OnChange = nameof(SyncHat))]
    private int hatInt = -1;

    private void Awake()
    {
        foreach (var item in hatCosmetics)
        {
            item.SetActive(false);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner) return;

        hatInt = PlayerPrefs.GetInt("Hat", 0);

        RPC_SetHat(hatInt);
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            RPC_SetHat(2);
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void RPC_SetHat(int _index)
    {
        hatInt = _index;
    }

    private void SyncHat(int prev, int next, bool asServer)
    {
        hatInt = next;

        hatCosmetics[hatInt].SetActive(true);
    }
}
