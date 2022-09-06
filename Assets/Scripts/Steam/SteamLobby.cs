using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using FishyFacepunch.Server;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    [SerializeField] private uint gameAppId = 480;

    private void OnApplicationQuit()
    {
        try
        {
            SteamClient.Shutdown();

            Debug.Log("Shutdown Steam");
        }
        catch
        {
            Debug.Log("Failed to Shutdown Steam");
        }
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobbyAsync();
    }
}
