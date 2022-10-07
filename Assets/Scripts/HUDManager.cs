using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using TMPro;

public class HUDManager : NetworkBehaviour
{
    [SerializeField] private GameObject spectate_HUD;
    [SerializeField] private GameObject playerWon_HUD_NonVR;
    [SerializeField] private GameObject playerWon_HUD_VR;

    [SerializeField] private TextMeshProUGUI playerWon_Text_NonVR;
    [SerializeField] private TextMeshProUGUI[] playerWon_Text_VR;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.Player_Died, PlayerDied);
        EventSystemNew.Subscribe(Event_Type.Player_Respawned, PlayerRespawned);
        EventSystemNew<string>.Subscribe(Event_Type.GAME_WON, GameWon);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.Player_Died, PlayerDied);
        EventSystemNew.Unsubscribe(Event_Type.Player_Respawned, PlayerRespawned);
        EventSystemNew<string>.Unsubscribe(Event_Type.GAME_WON, GameWon);
    }

    private void PlayerDied()
    {
        spectate_HUD.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlayerRespawned()
    {
        spectate_HUD.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RespawnPlayer()
    {
        EventSystemNew.RaiseEvent(Event_Type.Respawn_Player);
    }

    private void GameWon(string _playerName)
    {
        if (GameManager.Instance.isVR)
        {
            playerWon_HUD_VR.SetActive(true);

            foreach (var item in playerWon_Text_VR)
            {
                item.text = _playerName;
            }
        }
        else
        {
            playerWon_HUD_NonVR.SetActive(true);
            playerWon_Text_NonVR.text = _playerName;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
