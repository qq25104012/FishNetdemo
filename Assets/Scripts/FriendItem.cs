using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class FriendItem : MonoBehaviour
{
    [SerializeField] private RawImage profilePicture;
    [SerializeField] TextMeshProUGUI playerName;

    private SteamId steamId;

    public void Setup(Texture2D _profilePicture, string _playerName, SteamId _steamId)
    {
        profilePicture.texture = _profilePicture;
        playerName.text = _playerName;
        steamId = _steamId;
    }

    public async void Invite()
    {
        if (SteamLobbyManager.inLobby)
        {
            SteamLobbyManager.currentLobby.InviteFriend(steamId);
        }
        else
        {
            bool result = await SteamLobbyManager.CreateLobby();

            if (result)
            {
                SteamLobbyManager.currentLobby.InviteFriend(steamId);
            }
        }
    }
}
