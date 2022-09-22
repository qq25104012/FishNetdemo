//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using Photon.Pun;
//using TMPro;

//public class WebShooter : MonoBehaviour
//{
//    [Header("References")]
//    [SerializeField] Camera spiderCamera;

//    [SerializeField] Collider spiderCollider;

//    [SerializeField] string webTrailPrefabName;
//    [SerializeField] string nestTrailPrefabName;

//    [SerializeField] Transform webTrailSpawnPoint;

//    [SerializeField] TextMeshProUGUI webShootCooldownText;

//    [Header("Settings")]
//    [SerializeField] float webTrailSpeed = 5f;

//    [SerializeField] int shootDelay = 1;
//    [SerializeField] int nestDelay = 1;

//    //SOUND

//    private FMOD.Studio.EventInstance spiderShootSound;

//    bool canShoot = true;

//    bool preGame = false;

//    bool gameStarted = false;

//    private void OnEnable()
//    {
//        EventSystemNew.Subscribe(Event_Type.Shoot, ShootWeb);

//        EventSystemNew.Subscribe(Event_Type.PRE_GAME, PreGame);
//        EventSystemNew.Subscribe(Event_Type.GAME_STARTED, GameStarted);
//    }

//    private void OnDisable()
//    {
//        EventSystemNew.Unsubscribe(Event_Type.Shoot, ShootWeb);

//        EventSystemNew.Unsubscribe(Event_Type.PRE_GAME, PreGame);
//        EventSystemNew.Unsubscribe(Event_Type.GAME_STARTED, GameStarted);
//    }

//    public void ShootWeb()
//    {
//        if (gameStarted && canShoot)
//        {
//            canShoot = false;

//            GameObject webTrail = PhotonNetwork.Instantiate(webTrailPrefabName, webTrailSpawnPoint.position, spiderCamera.transform.rotation);

//            webTrail.GetComponent<WebTrail>().Setup(spiderCollider);
//            webTrail.GetComponent<Rigidbody>().velocity = spiderCamera.transform.forward * webTrailSpeed;

//            StartCoroutine(DelayShooting(shootDelay));

//            // SOUND
//            spiderShootSound = FMODUnity.RuntimeManager.CreateInstance("event:/SpiderShoot");
//            spiderShootSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
//            spiderShootSound.start();
//        }
//        else if (preGame && canShoot)
//        {
//            canShoot = false;

//            GameObject nestTrail = PhotonNetwork.Instantiate(nestTrailPrefabName, webTrailSpawnPoint.position, spiderCamera.transform.rotation);

//            nestTrail.GetComponent<WebTrail>().Setup(spiderCollider);
//            nestTrail.GetComponent<Rigidbody>().velocity = spiderCamera.transform.forward * webTrailSpeed;

//            StartCoroutine(DelayShooting(nestDelay));

//            // SOUND
//            spiderShootSound = FMODUnity.RuntimeManager.CreateInstance("event:/SpiderShoot");
//            spiderShootSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
//            spiderShootSound.start();
//        }
//    }

//    IEnumerator DelayShooting(int _shootDelay)
//    {
//        int currentTime = _shootDelay;

//        webShootCooldownText.text = currentTime.ToString();

//        webShootCooldownText.gameObject.SetActive(true);

//        while (currentTime > 0)
//        {
//            yield return new WaitForSeconds(1);

//            currentTime -= 1;

//            webShootCooldownText.text = currentTime.ToString();
//        }

//        webShootCooldownText.gameObject.SetActive(false);

//        canShoot = true;
//    }

//    private void PreGame()
//    {
//        preGame = true;
//    }

//    private void GameStarted()
//    {
//        preGame = false;
//        gameStarted = true;

//        StopAllCoroutines();

//        StartCoroutine(DelayShooting(0));
//    }
//}
