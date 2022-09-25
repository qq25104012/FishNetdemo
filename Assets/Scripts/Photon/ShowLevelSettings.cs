using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;

public class ShowLevelSettings : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI minutesText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        minutesText.text = PersistentLevelSettings.Instance.matchStartTime.ToString();
        scoreText.text = PersistentLevelSettings.Instance.scoreNeeded.ToString();

        PersistentLevelSettings.Instance.onMatchStartTimeUpdate += UpdateMatchStartTime;
        PersistentLevelSettings.Instance.onScoreNeededUpdate += UpdateScoreNeeded;
    }

    private void UpdateMatchStartTime(int _matchStartTime)
    {
        minutesText.text = _matchStartTime.ToString();
    }

    private void UpdateScoreNeeded(int _scoreNeeded)
    {
        scoreText.text = _scoreNeeded.ToString();
    }
}
