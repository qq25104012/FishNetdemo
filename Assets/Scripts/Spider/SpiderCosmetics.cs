using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class SpiderCosmetics : NetworkBehaviour
{
    [SerializeField] GameObject[] hatCosmetics;

    int hatInt = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();

        foreach (var item in hatCosmetics)
        {
            item.SetActive(false);
        }

        if (!IsOwner) return;

        Dictionary<string, int> customData = (Dictionary<string, int>)Owner.CustomData;
        
        if (customData.ContainsKey("Hat"))
        {
            hatInt = customData["Hat"];

            Debug.Log("Has Hat");
        }
        else
        {
            customData.Add("Hat", 1);
            Owner.CustomData = customData;
            hatInt = 1;
        }

        if (customData.ContainsKey("Hat"))
        {
            hatInt = customData["Hat"];

            Debug.Log("Has Hat: " + customData["Hat"]);
        }

        hatCosmetics[hatInt].SetActive(true);
    }
}
