using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;

public class PersistentLevelSettings : MonoBehaviour
{
    public static PersistentLevelSettings Instance { get; private set; }

    public System.Action<int> onMatchStartTimeUpdate;
    public System.Action<int> onScoreNeededUpdate;

    [SyncVar(OnChange = nameof(UpdateMatchStartTime))]
    public int matchStartTime;

    [SyncVar(OnChange = nameof(UpdateScoreNeeded))]
    public int scoreNeeded;

    [Header("Settings")]
    [SerializeField] private int minMinutes, maxMinutes;
    [SerializeField] private int minScoreNeeded, maxScoreNeeded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject.GetComponent(Instance.GetType()));
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        matchStartTime = minMinutes;
        scoreNeeded = minScoreNeeded;
    }

    public void ChangeMinutes(int _upDown)
    {
        matchStartTime += _upDown;
        matchStartTime = Mathf.Clamp(matchStartTime, minMinutes, maxMinutes);
    }

    public void ChangeScoreNeeded(int _upDown)
    {
        scoreNeeded += _upDown;
        scoreNeeded = Mathf.Clamp(scoreNeeded, minScoreNeeded, maxScoreNeeded);
    }

    private void UpdateMatchStartTime(int prev, int next, bool asServer)
    {
        matchStartTime = next;
        onMatchStartTimeUpdate?.Invoke(matchStartTime);
    }

    private void UpdateScoreNeeded(int prev, int next, bool asServer)
    {
        scoreNeeded = next;
        onScoreNeededUpdate?.Invoke(scoreNeeded);
    }
}
