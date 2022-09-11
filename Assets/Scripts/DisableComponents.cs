using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class DisableComponents : NetworkBehaviour
{
    [SerializeField] private GameObject[] objectsToDisableForOthers;
    [SerializeField] private Component[] componentsToDisableForOthers;

    [SerializeField] private GameObject[] objectsToDisableForOwn;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            foreach (var item in objectsToDisableForOthers)
            {
                item.SetActive(false);
            }

            foreach (var item in componentsToDisableForOthers)
            {
                Destroy(item);
            }

            enabled = false;
        }
        else
        {
            foreach (var item in objectsToDisableForOwn)
            {
                item.SetActive(false);
            }
        }
    }
}
