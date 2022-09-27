using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class SpiderCosmetics : NetworkBehaviour
{
    [SerializeField] GameObject[] hatCosmetics;

    [SyncVar(OnChange = nameof(SyncHat))]
    private int hatInt = 0;

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

        if (PlayerPrefs.HasKey("Hat"))
        {
            hatInt = PlayerPrefs.GetInt("Hat");
        }

        RPC_SetHat(hatInt);
    }

    [ServerRpc(RequireOwnership = true)]
    public void RPC_SetHat(int _index)
    {
        hatInt = _index;

        hatCosmetics[hatInt].SetActive(true);
    }

    private void SyncHat(int prev, int next, bool asServer)
    {
        Debug.Log("Sync Hat");

        hatCosmetics[hatInt].SetActive(true);
    }
}
