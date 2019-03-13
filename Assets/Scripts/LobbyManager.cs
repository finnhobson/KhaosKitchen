using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyManager : NetworkLobbyManager {

    public GameObject Lobby;
    public GameObject Menu;

    private int instructionTime, roundTime, playerCount;
    private float pointMultiplier;
    public Text instructionTimeText, roundTimeText, pointMultiplierText, playerCountText;

    private void Start()
    {
        Lobby.SetActive(false);
        SetDefaultSettings();
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

    public void SetSettings()
    {
        GameSettings.RoundTime = int.Parse(roundTimeText.text);
        roundTime = GameSettings.RoundTime;
        GameSettings.InstructionTime = int.Parse(instructionTimeText.text);
        instructionTime = GameSettings.InstructionTime;
        GameSettings.PointMultiplier = float.Parse(pointMultiplierText.text);
        pointMultiplier = GameSettings.PointMultiplier;
        GameSettings.PlayerCount = int.Parse(playerCountText.text);
        playerCount = GameSettings.PlayerCount;
    }

    private void SetDefaultSettings()
    {
        GameSettings.RoundTime = 90;
        GameSettings.InstructionTime = 15;
        GameSettings.PointMultiplier = 1;
        GameSettings.PlayerCount = 2;
    }

    public void ResetSettingsToLast()
    {
        roundTimeText.text = roundTime.ToString();
        instructionTimeText.text = instructionTime.ToString();
        pointMultiplierText.text = pointMultiplier.ToString();
    }
}
