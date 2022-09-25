using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class SpiderCosmetics : NetworkBehaviour
{
    [SerializeField] GameObject[] hatCosmetics;

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

    [ObserversRpc]
    public void RPC_SetHat(int _index)
    {
        hatInt = _index;

        hatCosmetics[hatInt].SetActive(true);
    }
}
