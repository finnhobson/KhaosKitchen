using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public LobbyManager lobbyManager;
    public GameObject Lobby;

	public void OnClickHost()
    {
        lobbyManager.StartServer();
    }

    public void OnClickJoin()
    {
        lobbyManager.networkAddress = "192.168.43.50";

        lobbyManager.StartClient();
        Lobby.SetActive(true);
    }
}
