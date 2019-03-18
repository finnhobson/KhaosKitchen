using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class LobbyManager : NetworkLobbyManager {

    public GameObject Lobby;
    public GameObject Menu;
    public NetworkLobbyHook NetworkLobbyHook;
    
    //private int roundTime, playerCount, BaseInstructionNumber, InstructionNumberIncreasePerRound, BaseInstructionTime, InstructionTimeReductionPerRound, InstructionTimeIncreasePerPlayer, MinimumInstructionTime;

    public Text roundTimeText, playerCountText, BaseInstructionNumberText, InstructionNumberIncreasePerRoundText, BaseInstructionTimeText, InstructionTimeReductionPerRoundText, InstructionTimeIncreasePerPlayerText, MinimumInstructionTimeText;


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
        NetworkLobbyHook = GetComponent<NetworkLobbyHook>();
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
        //GameSettings.RoundTime = int.Parse(roundTimeText.text);
        //roundTime = GameSettings.RoundTime;
        //GameSettings.InstructionTime = int.Parse(instructionTimeText.text);
        //instructionTime = GameSettings.InstructionTime;
        //GameSettings.PointMultiplier = float.Parse(pointMultiplierText.text);
        //pointMultiplier = GameSettings.PointMultiplier;
        //GameSettings.PlayerCount = int.Parse(playerCountText.text);
        //playerCount = GameSettings.PlayerCount;

        GameSettings.RoundTime = int.Parse(roundTimeText.text);
        GameSettings.PlayerCount = int.Parse(playerCountText.text);
        GameSettings.BaseInstructionNumber = int.Parse(BaseInstructionNumberText.text);
        GameSettings.InstructionNumberIncreasePerRound = int.Parse(InstructionNumberIncreasePerRoundText.text);
        GameSettings.BaseInstructionTime = int.Parse(BaseInstructionTimeText.text);
        GameSettings.InstructionTimeReductionPerRound = int.Parse(InstructionTimeReductionPerRoundText.text);
        GameSettings.InstructionTimeIncreasePerPlayer = int.Parse(InstructionTimeIncreasePerPlayerText.text);
        GameSettings.MinimumInstructionTime = int.Parse(MinimumInstructionTimeText.text);
    }

    private void SetDefaultSettings()
    {
        GameSettings.RoundTime = 60;
        GameSettings.PlayerCount = 2;
        GameSettings.BaseInstructionNumber = 4;
        GameSettings.InstructionNumberIncreasePerRound = 4;
        GameSettings.BaseInstructionTime = 15;
        GameSettings.InstructionTimeReductionPerRound = 2;
        GameSettings.InstructionTimeIncreasePerPlayer = 2;
        GameSettings.MinimumInstructionTime = 5;
    }
    
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        //This hook allows you to apply state data from the lobby-player to the game-player
        //just subclass "LobbyHook" and add it to the lobby object.

        if (NetworkLobbyHook)
            NetworkLobbyHook.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

        return true;
    }

}
