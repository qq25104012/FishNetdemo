using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Cinemachine;
using UnityEngine.InputSystem;

public class Spectate : MonoBehaviour
{
    [Header("Camera's")]
    [SerializeField] CinemachineFreeLook spectateCamera;
    [SerializeField] CinemachineVirtualCamera noSpectatorsCamera;
    [SerializeField] Camera cam;

    [Header("UI")]
    [SerializeField] GameObject spectateHUD;
    [SerializeField] TextMeshProUGUI spiderNameText;

    RopeGenerator[] spiders;

    int spectateID = 0;

    bool isSpectating = false;

    private void OnEnable()
    {
        EventSystemNew<bool, bool>.Subscribe(Event_Type.SPIDER_DIED, SpiderDied);

        EventSystemNew<bool>.Subscribe(Event_Type.SPIDER_RESPAWNED, SpiderRespawned);

        // Input Events
        EventSystemNew<int>.Subscribe(Event_Type.ChangeSpectator, ChangeSpectator);
    }

    private void OnDisable()
    {
        EventSystemNew<bool, bool>.Unsubscribe(Event_Type.SPIDER_DIED, SpiderDied);

        EventSystemNew<bool>.Unsubscribe(Event_Type.SPIDER_RESPAWNED, SpiderRespawned);

        // Input Events
        EventSystemNew<int>.Unsubscribe(Event_Type.ChangeSpectator, ChangeSpectator);
    }

    private void SetSpectating()
    {
        if (!isSpectating)
        {
            return;
        }

        RopeGenerator[] tempSpiders = FindObjectsOfType<RopeGenerator>();

        if (tempSpiders.Length == 0)
        {
            NoSpidersAlive();

            return;
        }

        if (spiders[spectateID] == null)
        {
            spectateID = 0;

            spiders = tempSpiders;

            SetSpectateTarget();
        }
        else
        {
            SetSpectateTarget();
        }
    }

    private void SetSpectateTarget()
    {
        noSpectatorsCamera.gameObject.SetActive(false);

        spectateCamera.Follow = spiders[spectateID].transform;
        spectateCamera.LookAt = spiders[spectateID].transform;

        spectateCamera.gameObject.SetActive(true);
        cam.gameObject.SetActive(true);

        spiderNameText.text = spiders[spectateID].GetComponent<PhotonView>().Controller.NickName;
    }

    private void ChangeSpectator(int _previousOrNext)
    {
        if (isSpectating)
        {
            spectateID += _previousOrNext;

            if (spectateID < 0)
            {
                spectateID = FindObjectsOfType<RopeGenerator>().Length - 1;
            }
            else if (spectateID > FindObjectsOfType<RopeGenerator>().Length - 1)
            {
                spectateID = 0;
            }

            SpiderDied(false, false);
        }
    }

    private void SpiderDied(bool _ownDeath, bool _increaseRespawnTime)
    {
        if (_ownDeath)
        {
            spectateID = 0;

            spectateHUD.SetActive(true);

            isSpectating = true;

            spiders = FindObjectsOfType<RopeGenerator>();
        }

        SetSpectating();
    }

    private void NoSpidersAlive()
    {
        spectateCamera.gameObject.SetActive(false);
        noSpectatorsCamera.gameObject.SetActive(true);
        cam.gameObject.SetActive(true);

        spiderNameText.text = "Nobody";
    }

    private void SpiderRespawned(bool _ownRespawn)
    {
        if (_ownRespawn)
        {
            if (spectateCamera != null)
            {
                isSpectating = false;

                spectateID = 0;

                spectateCamera.gameObject.SetActive(false);
                noSpectatorsCamera.gameObject.SetActive(false);
                cam.gameObject.SetActive(false);
            }

            spectateHUD.SetActive(false);
        }
        else
        {
            SpiderDied(false, false);
        }
    }
}
