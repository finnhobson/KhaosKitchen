using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public LobbyManager lobbyManager;
    public GameObject Lobby;
    public MyNetworkDiscovery networkDiscovery;

    public string ipAddress;

    private void Start()
    {
        //NetworkServer.Reset();
        networkDiscovery.Initialize();
        networkDiscovery.StartAsClient();
        Debug.Log("Listening for IP Addresses...");
    }

    public void OnClickHost()
    {
        networkDiscovery.StopBroadcast();
        Debug.Log("Stopped Listening.");
        networkDiscovery.Initialize();
        networkDiscovery.StartAsServer();
        Debug.Log("Broadcasting IP Address...");
        lobbyManager.StartServer();
    }

    public void OnClickJoin()
    {
        //lobbyManager.networkAddress = "172.20.10.4";
        Debug.Log("Joining Game using IP Address: " + lobbyManager.networkAddress);
        lobbyManager.StartClient();
        Lobby.SetActive(true);
    }
}
