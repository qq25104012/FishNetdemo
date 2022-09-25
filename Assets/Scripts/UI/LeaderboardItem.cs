using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI scoreText;

    public void Initialize(Player _player)
    {
        playerNameText.text = _player.NickName + " |";

        if (_player.CustomProperties.ContainsKey("Score"))
        {
            scoreText.text = ((int)_player.CustomProperties["Score"]).ToString();
        }
        else
        {
            scoreText.text = "0";
        }
    }

    public void UpdateScore(int _score)
    {
        scoreText.text = _score.ToString();
    }
}
