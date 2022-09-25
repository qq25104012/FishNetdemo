//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Photon.Pun;
//using Photon.Realtime;
//using ExitGames.Client.Photon;

//public class RoomManager : MonoBehaviourPunCallbacks
//{
//    [Header("Setup")]
//    [SerializeField] string eventReceiverPrefabName;

//    [SerializeField] string mainMenuScene;

//    bool hasLeft = false;

//    public override void OnEnable()
//    {
//        base.OnEnable();
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    public override void OnDisable()
//    {
//        base.OnDisable();
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }

//    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
//    {
//        if (scene.buildIndex == 1)
//        {
//            PhotonNetwork.Instantiate(eventReceiverPrefabName, Vector3.zero, Quaternion.identity);

//            Cursor.lockState = CursorLockMode.Locked;
//            Cursor.visible = false;

//            if (GameManager.Instance.isVR)
//            {
//                EventSystemNew.RaiseEvent(Event_Type.SPAWN_PLAYER);
//            }
//            else
//            {
//                EventSystemNew.RaiseEvent(Event_Type.SPAWN_SPIDER);
//            }
//        }
//    }

//    public void LeaveRoom()
//    {
//        if (!hasLeft)
//        {
//            hasLeft = true;

//            PhotonNetwork.LocalPlayer.CustomProperties.Clear();
//            PhotonNetwork.LeaveRoom();
//        }
//    }

//    public override void OnLeftRoom()
//    {
//        SceneManager.LoadScene(mainMenuScene);
//    }

//    public void RestartLevel(string _levelName)
//    {
//        object[] content = new object[] { _levelName };

//        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

//        PhotonNetwork.RaiseEvent((int)Event_Code.GameRestarted, content, raiseEventOptions, SendOptions.SendReliable);
//    }
//}