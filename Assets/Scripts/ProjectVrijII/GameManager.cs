//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;
//using Photon.Realtime;
//using ExitGames.Client.Photon;
//using TMPro;

//public enum PlayerTypes { Human, Spiders }

//public class GameManager : MonoBehaviourPunCallbacks
//{
//    public static GameManager Instance { get; private set; }

//    [SerializeField] bool _isVR = false;

//    [SerializeField] GameObject[] objectsToDisableForVR;
//    [SerializeField] GameObject[] objectsToDisableForNonVR;

//    [SerializeField] GameObject playerWonHUDNonVR;
//    [SerializeField] TextMeshProUGUI playerWonNameTextNonVR;

//    [SerializeField] GameObject playerWonHUDVR;
//    [SerializeField] TextMeshProUGUI[] playerWonNameTextsVR;

//    public Collider leftHandPalmCollider;
//    public Collider rightHandPalmCollider;

//    public bool isVR { get { return _isVR; } }

//    public bool preGame { private set; get; }

//    public bool gameStarted { private set; get; }

//    public bool gameEnded { private set; get; }

//    public override void OnEnable()
//    {
//        base.OnEnable();

//        EventSystemNew.Subscribe(Event_Type.PRE_GAME, PreGameStarted);

//        EventSystemNew<string>.Subscribe(Event_Type.GAME_WON, GameWon);

//        EventSystemNew.Subscribe(Event_Type.GAME_STARTED, GameStarted);

//        EventSystemNew.Subscribe(Event_Type.GAME_ENDED, GameEnded);
//    }

//    public override void OnDisable()
//    {
//        base.OnDisable();

//        EventSystemNew.Unsubscribe(Event_Type.PRE_GAME, PreGameStarted);

//        EventSystemNew<string>.Unsubscribe(Event_Type.GAME_WON, GameWon);

//        EventSystemNew.Unsubscribe(Event_Type.GAME_STARTED, GameStarted);

//        EventSystemNew.Unsubscribe(Event_Type.GAME_ENDED, GameEnded);
//    }

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }

//        if (isVR)
//        {
//            foreach (var item in objectsToDisableForVR)
//            {
//                Destroy(item);
//            }
//        }
//        else
//        {
//            foreach (var item in objectsToDisableForNonVR)
//            {
//                Destroy(item);
//            }
//        }
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.L))
//        {
//            if (!preGame)
//            {
//                StartPreGame();

//                return;
//            }

//            if (!gameStarted && !gameEnded && preGame)
//            {
//                StartGame();
//            }
//        }
//    }

//    private void LeaveRoom()
//    {
//        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
//        PhotonNetwork.LeaveRoom();
//    }

//    public void StartGame()
//    {
//        object[] content = new object[] { };

//        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

//        PhotonNetwork.RaiseEvent((int)Event_Code.GameStarted, content, raiseEventOptions, SendOptions.SendReliable);
//    }
    
//    public void StartPreGame()
//    {
//        object[] content = new object[] { };

//        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

//        PhotonNetwork.RaiseEvent((int)Event_Code.PreGame, content, raiseEventOptions, SendOptions.SendReliable);
//    }

//    private void PreGameStarted()
//    {
//        preGame = true;
//    }

//    private void GameStarted()
//    {
//        gameStarted = true;
//    }

//    private void GameWon(string _playerName)
//    {
//        if (isVR)
//        {
//            playerWonHUDVR.SetActive(true);

//            foreach (var item in playerWonNameTextsVR)
//            {
//                item.text = _playerName;
//            }
//        }
//        else
//        {
//            playerWonHUDNonVR.SetActive(true);
//            playerWonNameTextNonVR.text = _playerName;

//            Cursor.lockState = CursorLockMode.None;
//            Cursor.visible = true;
//        }
//    }

//    public void GameEnded()
//    {
//        gameStarted = false;

//        gameEnded = true;
//    }

//    public override void OnPlayerEnteredRoom(Player newPlayer)
//    {
//        if (PhotonNetwork.IsMasterClient)
//        {
//            if (gameStarted)
//            {
//                object[] content = new object[] { };

//                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

//                PhotonNetwork.RaiseEvent((int)Event_Code.GameStarted, content, raiseEventOptions, SendOptions.SendReliable);
//            }
//        }
//    }
//}
