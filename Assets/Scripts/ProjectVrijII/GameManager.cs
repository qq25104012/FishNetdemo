using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public enum PlayerTypes { Human, Spiders }

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject[] objectsToDisableForVR;
    [SerializeField] GameObject[] objectsToDisableForNonVR;

    public Collider leftHandPalmCollider;
    public Collider rightHandPalmCollider;

    public bool isVR { get { return _isVR; } }
    [SerializeField] private bool _isVR = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (!IsServer) return;
            InstanceFinder.ServerManager.Despawn(gameObject);
            return;
        }

        Instance = this;

        if (isVR)
        {
            foreach (var item in objectsToDisableForVR)
            {
                Destroy(item);
            }
        }
        else
        {
            foreach (var item in objectsToDisableForNonVR)
            {
                Destroy(item);
            }
        }
    }
}
