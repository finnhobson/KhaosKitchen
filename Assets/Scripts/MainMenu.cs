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
        networkDiscovery.Initialize();
        networkDiscovery.StartAsClient();
    }

    public void OnClickHost()
    {
        networkDiscovery.StopBroadcast();
        networkDiscovery.Initialize();
        networkDiscovery.StartAsServer();
        lobbyManager.StartServer();
    }

    public void OnClickJoin()
    {
        //lobbyManager.networkAddress = "172.20.10.4";
        lobbyManager.StartClient();
        Lobby.SetActive(true);
    }
}
