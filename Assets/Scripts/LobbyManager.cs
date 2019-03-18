using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyManager : NetworkLobbyManager {

    public GameObject Lobby;
    public GameObject Menu;
    
    //private int roundTime, playerCount, BaseInstructionNumber, InstructionNumberIncreasePerRound, BaseInstructionTime, InstructionTimeReductionPerRound, InstructionTimeIncreasePerPlayer, MinimumInstructionTime;

    public Text roundTimeText, BaseInstructionNumberText, InstructionNumberIncreasePerRoundText, BaseInstructionTimeText, InstructionTimeReductionPerRoundText, InstructionTimeIncreasePerPlayerText, MinimumInstructionTimeText;

    public Text playerCountText;
    public Slider playerCountSlider;

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
        //GameSettings.RoundTime = int.Parse(roundTimeText.text);
        //roundTime = GameSettings.RoundTime;
        //GameSettings.InstructionTime = int.Parse(instructionTimeText.text);
        //instructionTime = GameSettings.InstructionTime;
        //GameSettings.PointMultiplier = float.Parse(pointMultiplierText.text);
        //pointMultiplier = GameSettings.PointMultiplier;
        //GameSettings.PlayerCount = int.Parse(playerCountText.text);
        //playerCount = GameSettings.PlayerCount;

        //GameSettings.RoundTime = int.Parse(roundTimeText.text);
        //GameSettings.PlayerCount = int.Parse(playerCountText.text);
        //GameSettings.BaseInstructionNumber = int.Parse(BaseInstructionNumberText.text);
        //GameSettings.InstructionNumberIncreasePerRound = int.Parse(InstructionNumberIncreasePerRoundText.text);
        //GameSettings.BaseInstructionTime = int.Parse(BaseInstructionTimeText.text);
        //GameSettings.InstructionTimeReductionPerRound = int.Parse(InstructionTimeReductionPerRoundText.text);
        //GameSettings.InstructionTimeIncreasePerPlayer = int.Parse(InstructionTimeIncreasePerPlayerText.text);
        //GameSettings.MinimumInstructionTime = int.Parse(MinimumInstructionTimeText.text);

        GameSettings.RoundTime = string.IsNullOrEmpty(roundTimeText.text) ? 60 : int.Parse(roundTimeText.text);
        GameSettings.BaseInstructionNumber = string.IsNullOrEmpty(BaseInstructionNumberText.text) ? 16 : int.Parse(BaseInstructionNumberText.text);
        GameSettings.InstructionNumberIncreasePerRound = string.IsNullOrEmpty(InstructionNumberIncreasePerRoundText.text) ? 16 : int.Parse(InstructionNumberIncreasePerRoundText.text);
        GameSettings.BaseInstructionTime = string.IsNullOrEmpty(BaseInstructionTimeText.text) ? 15 : int.Parse(BaseInstructionTimeText.text);
        GameSettings.InstructionTimeReductionPerRound = string.IsNullOrEmpty(InstructionTimeReductionPerRoundText.text) ? 2 : int.Parse(InstructionTimeReductionPerRoundText.text);
        GameSettings.InstructionTimeIncreasePerPlayer = string.IsNullOrEmpty(InstructionTimeIncreasePerPlayerText.text) ? 2 : int.Parse(InstructionTimeIncreasePerPlayerText.text);
        GameSettings.MinimumInstructionTime = string.IsNullOrEmpty(MinimumInstructionTimeText.text) ? 5 : int.Parse(MinimumInstructionTimeText.text);

        //GameSettings.PlayerCount = string.IsNullOrEmpty(playerCountText.text) ? 2 : int.Parse(playerCountText.text);
        GameSettings.PlayerCount = (int)playerCountSlider.value;

    }

    private void SetDefaultSettings()
    {
        GameSettings.RoundTime = 60;
        GameSettings.PlayerCount = 2;
        GameSettings.BaseInstructionNumber = 16;
        GameSettings.InstructionNumberIncreasePerRound = 16;
        GameSettings.BaseInstructionTime = 15;
        GameSettings.InstructionTimeReductionPerRound = 2;
        GameSettings.InstructionTimeIncreasePerPlayer = 2;
        GameSettings.MinimumInstructionTime = 5;
    }

    public void UpdatePlayerCountText()
    {
        playerCountText.text = "Player Count: " + playerCountSlider.value;
    }

}
