using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Steamworks.Data;
using System.Threading.Tasks;
using UnityEngine.Events;
using TMPro;
using FishNet;
using FishNet.Transporting;

public class SteamLobbyManager : MonoBehaviour
{
    public static Lobby currentLobby;
    public static bool inLobby;

    [SerializeField] private UnityEvent OnLobbyCreated;
    [SerializeField] private UnityEvent OnLobbyJoined;
    [SerializeField] private UnityEvent OnLobbyLeave;

    [SerializeField] private GameObject playerItem;
    [SerializeField] private Transform playerContent;

    [SerializeField] private GameObject lobbyItem;
    [SerializeField] private Transform lobbyContent;

    private const string HostAddressKey = "HostAddress";
    private const string GameIdentifier = "GameIdentifier";

    private Dictionary<SteamId, GameObject> players = new Dictionary<SteamId, GameObject>();

    private List<GameObject> lobbyItems = new List<GameObject>();

    private bool isHost = false;

    private void Start()
    {
        //DontDestroyOnLoad(this);

        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberDisconnected;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        SteamMatchmaking.OnChatMessage += OnChatMessage;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;

        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequest;

        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }

    private void Update()
    {
        SteamClient.RunCallbacks();
    }

    private void OnLobbyInvite(Friend _friend, Lobby _lobby)
    {
        Debug.Log($"{_friend.Name} invited you to his lobby");
    }

    private void OnLobbyGameCreated(Lobby _lobby, uint _ip, ushort _port, SteamId _id)
    {

    }

    private void OnLobbyMemberJoined(Lobby _lobby, Friend _friend)
    {
        Debug.Log($"{_friend.Name} joined the lobby");

        CreatePlayerItem(_friend.Id, _friend.Name);
    }

    private void OnLobbyMemberDisconnected(Lobby _lobby, Friend _friend)
    {
        Debug.Log($"{_friend.Name} left the lobby");
        Debug.Log($"New lobby owner is {currentLobby.Owner}");

        if (currentLobby.Owner.Id == SteamClient.SteamId)
        {
            OnLobbyCreated?.Invoke();

            _lobby.SetData("Owner", SteamClient.Name);

            isHost = true;
        }

        if (players.ContainsKey(_friend.Id))
        {
            Destroy(players[_friend.Id]);
            players.Remove(_friend.Id);
        }
    }

    private void OnChatMessage(Lobby _lobby, Friend _friend, string _message)
    {
        Debug.Log($"incoming chat message from {_friend.Name} : {_message}");
    }

    private async void OnGameLobbyJoinRequest(Lobby _joinedLobby, SteamId _id)
    {
        RoomEnter joinedLobbySuccessfuly = await _joinedLobby.Join();

        if (joinedLobbySuccessfuly != RoomEnter.Success)
        {
            Debug.Log("failed to join lobby : " + joinedLobbySuccessfuly);
        }
        else
        {
            currentLobby = _joinedLobby;
        }
    }

    private void OnLobbyCreatedCallback(Result _result, Lobby _lobby)
    {
        if (_result != Result.OK)
        {
            Debug.Log("Lobby creation result not OK : " + _result);
        }
        else
        {
            isHost = true;

            OnLobbyCreated?.Invoke();

            _lobby.SetData(HostAddressKey, SteamClient.SteamId.ToString());
            _lobby.SetData(GameIdentifier, "Tommetje");
            _lobby.SetData("Owner", SteamClient.Name);

            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();

            Debug.Log("Lobby creation result OK");
        }
    }

    private void OnLobbyEntered(Lobby _lobby)
    {
        Debug.Log("Client joined the lobby");

        inLobby = true;

        foreach (var player in players.Values)
        {
            Destroy(player);
        }

        players.Clear();

        CreatePlayerItem(SteamClient.SteamId, SteamClient.Name);

        foreach (var player in currentLobby.Members)
        {
            if (player.Id != SteamClient.SteamId)
            {
                CreatePlayerItem(player.Id, player.Name);
            }
        }

        OnLobbyJoined?.Invoke();

        // FishNet
        string hostAddress = _lobby.GetData(HostAddressKey);

        if (!isHost)
        {
            InstanceFinder.ClientManager.StartConnection(hostAddress);
        }
    }

    public async void CreateLobbyAsync()
    {
        bool result = await CreateLobby();

        if (!result)
        {
            // Invoke an error message
        }
    }

    public static async Task<bool> CreateLobby()
    {
        try
        {
            var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync(5);

            if (!createLobbyOutput.HasValue)
            {
                Debug.Log("Lobby created but not correctly instantiated");

                return false;
            }

            currentLobby = createLobbyOutput.Value;
            currentLobby.SetPublic();
            currentLobby.SetJoinable(true);

            return true;
        }
        catch (System.Exception _exception)
        {
            Debug.Log("Failed to create lobby : " + _exception);

            return false;
        }
    }

    public void LeaveLobby()
    {
        try
        {
            if (isHost)
            {
                InstanceFinder.ServerManager.StopConnection(false);

                isHost = false;
            }

            InstanceFinder.ClientManager.StopConnection();

            inLobby = false;

            currentLobby.Leave();
            OnLobbyLeave?.Invoke();

            foreach (var player in players.Values)
            {
                Destroy(player);
            }

            players.Clear();
        }
        catch
        {

        }
    }

    public async void RefreshLobbies()
    {
        foreach (var lobbyItem in lobbyItems)
        {
            Destroy(lobbyItem);
        }

        lobbyItems.Clear();

        Lobby[] lobbies = await SteamMatchmaking.LobbyList.RequestAsync();

        if (lobbies.Length > 0)
        {
            foreach (var lobby in lobbies)
            {
                if (lobby.GetData(GameIdentifier) == "Tommetje1")
                {
                    GameObject item = Instantiate(lobbyItem, lobbyContent);

                    LobbyDataEntry lobbyEntry = item.GetComponent<LobbyDataEntry>();
                    lobbyEntry.SetLobbyData(lobby);

                    lobbyItems.Add(item);
                }
            }
        }
    }

    private async void CreatePlayerItem(SteamId _steamId, string _name)
    {
        GameObject item = Instantiate(playerItem, playerContent);
        var img = await SteamFriends.GetLargeAvatarAsync(_steamId);
        item.GetComponent<FriendItem>().Setup(SteamFriendsManager.GetTextureFromImage(img.Value), _name, _steamId);

        players.Add(_steamId, item);
    }

    private void OnClientConnectionState(ClientConnectionStateArgs _args)
    {
        Debug.Log("State: " + _args.ConnectionState);
    }
}
