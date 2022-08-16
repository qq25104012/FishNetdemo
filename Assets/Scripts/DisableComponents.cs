using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class DisableComponents : NetworkBehaviour
{
    [SerializeField] private GameObject[] objectsToDisableForOthers;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            foreach (var item in objectsToDisableForOthers)
            {
                item.SetActive(false);
            }

            enabled = false;
        }
    }
}
