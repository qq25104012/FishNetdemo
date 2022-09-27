using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using FishNet.Connection;
using FishNet;
using Steamworks;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private TextMeshProUGUI healthText;

    [SyncVar(OnChange = nameof(UpdateHealth))]
    private float health;

    public override void OnStartClient()
    {
        base.OnStartClient();

        health = startHealth;
    }

    public void TakeDamage(float _damage, string _address)
    {
        if (!IsServer) return;

        health -= _damage;

        if (health <= 0f)
        {
            health = 0f;

            Debug.Log("Connection: " + _address);
            EventSystemNew<string, int>.RaiseEvent(Event_Type.UPDATE_SCORE, _address, 1);

            RPC_PlayerDied(Owner);

            Despawn();
        }
    }

    private void UpdateHealth(float _prevHealth, float _curHealth, bool asServer)
    {
        health = _curHealth;

        healthText.text = health + " +";
    }

    [TargetRpc]
    private void RPC_PlayerDied(NetworkConnection conn)
    {
        EventSystemNew.RaiseEvent(Event_Type.Player_Died);
    }
}
