using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyManager : NetworkLobbyManager {

    public GameObject Lobby;
    public GameObject Menu;

    private void Start()
    {
        Lobby.SetActive(false);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("New game created at " + networkAddress);
        Lobby.SetActive(true);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        Menu.SetActive(false);
        Lobby.SetActive(false);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        Menu.SetActive(false);
        Lobby.SetActive(false);
    }
}
