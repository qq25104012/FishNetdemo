using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using TMPro;
using FishNet;
using FishNet.Object;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI scoreNeededText;

    [SerializeField] Transform leaderboardContainer;

    [SerializeField] GameObject leaderboardItemPrefab;

    Dictionary<Friend, LeaderboardItem> leaderboardItems = new Dictionary<Friend, LeaderboardItem>();

    int maxScore = 0;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        InstanceFinder.NetworkManager.SceneManager.AddOwnerToDefaultScene(NetworkObject);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //maxScore = PersistentLevelSettings.Instance.scoreNeeded;
        scoreNeededText.text = "First to " + maxScore.ToString() + " points";

        if (!IsServer) return;

        foreach (var player in SteamLobbyManager.currentLobby.Members)
        {
            AddLeaderboardItem(player);
        }

        // SteamID is the same as LocalConnection.GetAddress()
    }

    public void OnEnable()
    {
        EventSystemNew<string, int>.Subscribe(Event_Type.UPDATE_SCORE, UpdateScore);

        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberDisconnected;
    }

    public void OnDisable()
    {
        EventSystemNew<string, int>.Unsubscribe(Event_Type.UPDATE_SCORE, UpdateScore);

        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberDisconnected;
    }

    private void AddLeaderboardItem(Friend _player)
    {
        if (!IsServer) return;

        GameObject item = Instantiate(leaderboardItemPrefab, leaderboardContainer);
        Spawn(item);

        item.GetComponent<LeaderboardItem>().Initialize(_player);

        leaderboardItems.Add(_player, item.GetComponent<LeaderboardItem>());
    }

    private void RemoveLeaderboardItem(Friend _player)
    {
        if (!IsServer) return;

        if (leaderboardItems.ContainsKey(_player))
        {
            InstanceFinder.ServerManager.Despawn(leaderboardItems[_player].gameObject);

            leaderboardItems.Remove(_player);
        }
    }

    private void UpdateScore(string _steamID, int _score)
    {
        foreach (var player in SteamLobbyManager.currentLobby.Members)
        {
            if (player.Id.Value == ulong.Parse(_steamID))
            {
                leaderboardItems[player].ChangeScore(_score);

                if (leaderboardItems[player].score >= maxScore)
                {
                    // Game Won by Player Name
                    Debug.Log("Game Won by " + player.Name);
                }
            }
        }
    }

    private void OnLobbyMemberJoined(Lobby _lobby, Friend _player)
    {
        if (!IsServer) return;

        AddLeaderboardItem(_player);
    }

    private void OnLobbyMemberDisconnected(Lobby _lobby, Friend _player)
    {
        if (!IsServer) return;

        RemoveLeaderboardItem(_player);
    }
}
