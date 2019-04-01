using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public LobbyManager lobbyManager;
    public GameObject lobby, mainMenu;
    public MyNetworkDiscovery networkDiscovery;
    private bool host = false;
    private bool client = false;

    public string ipAddress;

    private void Start()
    {
        mainMenu.SetActive(true);
        NetworkServer.Reset();
        networkDiscovery.Initialize();
        networkDiscovery.StartAsClient();
        client = true;
        Debug.Log("Listening for IP Addresses...");
    }

    public void OnClickHost()
    {
        networkDiscovery.StopBroadcast();
        Debug.Log("Stopped Listening.");
        NetworkServer.Reset();
        networkDiscovery.Initialize();
        networkDiscovery.StartAsServer();
        Debug.Log("Broadcasting IP Address...");
        lobbyManager.StartServer();
        client = false;
        host = true;
    }

    public void OnClickJoin()
    {

        //lobbyManager.networkAddress = "172.20.10.4";
        lobbyManager.networkAddress = "127.0.0.1";
        //lobbyManager.networkAddress = "192.168.0.100";
        Debug.Log("Joining Game using IP Address: " + lobbyManager.networkAddress);
        lobbyManager.StartClient();
        lobby.SetActive(true);
    }

    public void OnClickBack()
    {
        if (host)
        {
            networkDiscovery.StopBroadcast();
            NetworkServer.Reset();
            networkDiscovery.Initialize();
            networkDiscovery.StartAsClient();
            lobbyManager.StopServer();
            host = false;
            Debug.Log("Server stopped. Back to Main Menu.");
        }
        else
        {
            networkDiscovery.StopBroadcast();
            lobbyManager.StopClient();
            Debug.Log("Client stopped. Back to Main Menu.");
        }
        lobby.SetActive(false);
    }
}
