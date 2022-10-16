using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject spectateCamera;

    private void OnEnable()
    {
        EventSystemNew.Subscribe(Event_Type.Player_Died, PlayerDied);
        EventSystemNew.Subscribe(Event_Type.Player_Respawned, PlayerRespawned);
    }

    private void OnDisable()
    {
        EventSystemNew.Unsubscribe(Event_Type.Player_Died, PlayerDied);
        EventSystemNew.Unsubscribe(Event_Type.Player_Respawned, PlayerRespawned);
    }

    private void PlayerDied()
    {
        EventSystemNew.RaiseEvent(Event_Type.SPIDER_DESTROY_CAMERA);

        spectateCamera.SetActive(true);
    }

    private void PlayerRespawned()
    {
        spectateCamera.SetActive(false);
    }
}
