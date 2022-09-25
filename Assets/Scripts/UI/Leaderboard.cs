using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;

public class Leaderboard : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI scoreNeededText;

    [SerializeField] Transform leaderboardContainer;

    [SerializeField] GameObject leaderboardItemPrefab;

    Dictionary<Player, LeaderboardItem> leaderboardItems = new Dictionary<Player, LeaderboardItem>();

    int maxScore = 0;

    public override void OnEnable()
    {
        base.OnEnable();

        EventSystemNew<Player, int>.Subscribe(Event_Type.UPDATE_SCORE, UpdateScore);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        EventSystemNew<Player, int>.Unsubscribe(Event_Type.UPDATE_SCORE, UpdateScore);
    }

    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("isVR"))
            {
                AddLeaderboardItem(player);
            }
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("ScoreNeeded"))
        {
            maxScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["ScoreNeeded"];

            scoreNeededText.text = string.Format("<color=#FF9F00>First to</color> <color=#18FF00>{0}</color> <color=#FF9F00>points</color>", maxScore);
        }
    }

    private void AddLeaderboardItem(Player _player)
    {
        LeaderboardItem item = Instantiate(leaderboardItemPrefab, leaderboardContainer).GetComponent<LeaderboardItem>();

        item.Initialize(_player);

        leaderboardItems.Add(_player, item);
    }

    private void RemoveLeaderboardItem(Player _player)
    {
        if (leaderboardItems.ContainsKey(_player))
        {
            Destroy(leaderboardItems[_player].gameObject);

            leaderboardItems.Remove(_player);
        }
    }

    private void UpdateScore(Player _player, int _score)
    {
        leaderboardItems[_player].UpdateScore(_score);

        if (_score >= maxScore)
        {
            object[] content = new object[] { _player.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent((int)Event_Code.GameWon, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!newPlayer.CustomProperties.ContainsKey("isVR"))
        {
            AddLeaderboardItem(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveLeaderboardItem(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Score"))
        {
            UpdateScore(targetPlayer, (int)changedProps["Score"]);
        }
    }
}
