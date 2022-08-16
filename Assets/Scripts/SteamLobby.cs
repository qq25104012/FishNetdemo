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

    //public void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        DontDestroyOnLoad(gameObject);
    //        Instance = this;

    //        try
    //        {
    //            // Create client
    //            SteamClient.Init(gameAppId, true);

    //            if (!SteamClient.IsValid)
    //            {
    //                Debug.Log("Steam client not valid");
    //                throw new Exception();
    //            }

    //            Debug.Log("Connected");
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log("Error connecting to Steam");
    //            Debug.Log(e);
    //        }
    //    }
    //    else if (Instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobbyAsync();
    }
}
