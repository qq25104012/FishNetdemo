using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class HUDManager : NetworkBehaviour
{
    [SerializeField] private GameObject spectateHUD;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.Player_Died, PlayerDied);
        EventSystemNew.Subscribe(Event_Type.Player_Respawned, PlayerRespawned);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.Player_Died, PlayerDied);
        EventSystemNew.Unsubscribe(Event_Type.Player_Respawned, PlayerRespawned);
    }

    private void PlayerDied()
    {
        spectateHUD.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlayerRespawned()
    {
        spectateHUD.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RespawnPlayer()
    {
        EventSystemNew.RaiseEvent(Event_Type.Respawn_Player);
    }
}
