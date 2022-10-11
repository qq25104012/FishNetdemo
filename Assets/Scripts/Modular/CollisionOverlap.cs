using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionOverlap : NetworkBehaviour
{
    [SerializeField] private UnityEvent onCollisionEnter = new UnityEvent();
    [SerializeField] private UnityEvent onCollisionExit = new UnityEvent();

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        Debug.Log("Steam ID: " + Owner.CustomData);

        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            onCollisionEnter?.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!IsServer) return;

        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            onCollisionExit?.Invoke();
        }
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}
