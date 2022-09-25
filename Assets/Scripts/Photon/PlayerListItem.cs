using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Color ownPlayerTextColor;
    [SerializeField] TextMeshProUGUI playerNameText;

    [SerializeField] GameObject[] hatCosmetics;

    Player player;

    int hatIndex = 0;

    public void SetUp(Player _player)
    {
        player = _player;
        playerNameText.text = _player.NickName;

        if (player == PhotonNetwork.LocalPlayer)
            playerNameText.color = ownPlayerTextColor;

        if (hatCosmetics.Length == 0)
            return;

        // Hats
        foreach (var item in hatCosmetics)
        {
            item.SetActive(false);
        }

        if (_player.CustomProperties.ContainsKey("Hat"))
        {
            hatIndex = (int)_player.CustomProperties["Hat"];
        }
        else
        {
            hatIndex = 0;
        }

        hatCosmetics[hatIndex].SetActive(true);
    }

    public void ChangeHat()
    {
        if (player == PhotonNetwork.LocalPlayer)
        {
            hatIndex++;

            if (hatIndex > hatCosmetics.Length - 1)
            {
                hatIndex = 0;
            }

            var hash = player.CustomProperties;

            hash["Hat"] = hatIndex;

            player.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Hat"))
        {
            if (hatCosmetics.Length == 0)
                return;

            if (targetPlayer == player)
            {
                hatIndex = (int)targetPlayer.CustomProperties["Hat"];

                foreach (var item in hatCosmetics)
                {
                    item.SetActive(false);
                }

                hatCosmetics[hatIndex].SetActive(true);
            }
        }
    }
}
