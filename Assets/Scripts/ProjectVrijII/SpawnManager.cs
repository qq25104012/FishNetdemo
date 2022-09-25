using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] TextMeshProUGUI respawnTimeText;
    [SerializeField] int respawnTime = 5;
    [SerializeField] int respawnTimeIncrease = 5;
    [SerializeField] int maxRespawnTime = 15;

    [SerializeField] string playerPrefab;
    [SerializeField] string spiderPrefab;

    [SerializeField] GameObject[] startHUDSVR;

    [SerializeField] Transform[] playerSpawnPoints;
    [SerializeField] Transform[] spiderSpawnPoints;

    GameObject spawnedPlayer;

    int currentTime = 0;

    int startRespawnTime;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.SPAWN_PLAYER, SpawnPlayer);
        EventSystemNew.Subscribe(Event_Type.SPAWN_SPIDER, SpawnSpider);
        EventSystemNew<bool, bool>.Subscribe(Event_Type.SPIDER_DIED, SpiderDied);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.SPAWN_PLAYER, SpawnPlayer);
        EventSystemNew.Unsubscribe(Event_Type.SPAWN_SPIDER, SpawnSpider);
        EventSystemNew<bool, bool>.Unsubscribe(Event_Type.SPIDER_DIED, SpiderDied);
    }

    private void Awake()
    {
        currentTime = respawnTime;

        startRespawnTime = respawnTime;

        foreach (var item in startHUDSVR)
        {
            item.SetActive(false);
        }
    }

    private void SpawnPlayer()
    {
        int randomPlayerSpawnPoint = Random.Range(0, playerSpawnPoints.Length);

        startHUDSVR[randomPlayerSpawnPoint].SetActive(true);

        spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab, playerSpawnPoints[randomPlayerSpawnPoint].position, Quaternion.identity);
    }

    private void SpawnSpider()
    {
        int randomSpiderSpawnPoint = Random.Range(0, spiderSpawnPoints.Length);

        spawnedPlayer = PhotonNetwork.Instantiate(spiderPrefab, spiderSpawnPoints[randomSpiderSpawnPoint].position, Quaternion.identity);
    }

    private void SpiderDied(bool _ownDeath, bool _increaseRespawnTime)
    {
        if (_ownDeath)
        {
            StartCoroutine(RespawnSpiderTimer(_increaseRespawnTime));
        }
    }

    private IEnumerator RespawnSpiderTimer(bool _increaseRespawnTime)
    {
        if (!_increaseRespawnTime)
        {
            respawnTime = startRespawnTime;

            currentTime = respawnTime;
        }

        respawnTimeText.text = string.Format("{0} Seconds", currentTime);

        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1);

            currentTime -= 1;

            respawnTimeText.text = string.Format("{0} Seconds", currentTime);
        }

        RespawnSpider();

        if (_increaseRespawnTime)
        {
            if (respawnTime + respawnTimeIncrease <= maxRespawnTime)
            {
                respawnTime += respawnTimeIncrease;
            }
        }

        currentTime = respawnTime;

        yield break;
    }

    private void RespawnSpider()
    {
        int randomSpiderSpawnPoint = Random.Range(0, spiderSpawnPoints.Length);

        spawnedPlayer = PhotonNetwork.Instantiate(spiderPrefab, spiderSpawnPoints[randomSpiderSpawnPoint].position, Quaternion.identity);

        object[] content = new object[] { spawnedPlayer.GetComponent<PhotonView>().ViewID, true };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent((int)Event_Code.RespawnSpider, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
