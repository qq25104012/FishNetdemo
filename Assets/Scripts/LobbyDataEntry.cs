using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;
using Steamworks.Data;

public class LobbyDataEntry : MonoBehaviour
{
    [SerializeField] private RawImage profilePicture;

    [SerializeField] private TextMeshProUGUI lobbyNameText;

    private Lobby lobby;

    public void SetLobbyData(Lobby _lobby, Texture2D _profilePicture)
    {
        lobby = _lobby;
        profilePicture.texture = _profilePicture;

        if (_lobby.GetData("Owner") == string.Empty)
        {
            lobbyNameText.text = "Unknown";
        }
        else
        {
            lobbyNameText.text = _lobby.GetData("Owner");
        }
    }

    public async void JoinLobby()
    {
        RoomEnter joinedLobbySuccessfuly = await lobby.Join();

        if (joinedLobbySuccessfuly != RoomEnter.Success)
        {
            Debug.Log("failed to join lobby : " + joinedLobbySuccessfuly);
        }
        else
        {
            SteamLobbyManager.currentLobby = lobby;
        }
    }
}
