using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    public NGameController networkController;

    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkCowboy gamePlayerPrefab = null;
    [SerializeField] private GameObject[] spawnpoints;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        
        bool isLeader = RoomPlayers.Count == 0;

        NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

        roomPlayerInstance.IsLeader = isLeader;

        NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if (!IsReadyToStart()) { return; }

        List<NetworkCowboy> players = new List<NetworkCowboy>();

        for (int i = RoomPlayers.Count - 1; i >= 0; i--)
        {
            var conn = RoomPlayers[i].connectionToClient;
            var gameplayerInstance = Instantiate(gamePlayerPrefab);
            gameplayerInstance.name = RoomPlayers[i].DisplayName;
            gameplayerInstance.playerName = RoomPlayers[i].DisplayName;

            if (i == 0)
            {
                gameplayerInstance.transform.position = spawnpoints[0].transform.position;
                gameplayerInstance.transform.rotation = Quaternion.LookRotation(spawnpoints[1].transform.position - spawnpoints[0].transform.position);
            }
            else if (i == 1)
            {
                gameplayerInstance.transform.position = spawnpoints[1].transform.position;
                gameplayerInstance.transform.rotation = Quaternion.LookRotation(spawnpoints[0].transform.position - spawnpoints[1].transform.position);
            }
                
            NetworkServer.Destroy(conn.identity.gameObject);
            NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);



            players.Add(gameplayerInstance.GetComponent<NetworkCowboy>());
        }
        networkController.StartNetwork(players);
    }
    

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
