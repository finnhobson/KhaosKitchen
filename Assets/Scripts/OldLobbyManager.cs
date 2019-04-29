using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class OldLobbyManager : NetworkLobbyManager {

    public GameObject Lobby;
    public GameObject Menu;
    public NetworkLobbyHook NetworkLobbyHook;
    
    //private int roundTime, playerCount, BaseInstructionNumber, InstructionNumberIncreasePerRound, BaseInstructionTime, InstructionTimeReductionPerRound,
    //InstructionTimeIncreasePerPlayer, MinimumInstructionTime;

    public Text roundTimeText, BaseInstructionNumberText, InstructionNumberIncreasePerRoundText, BaseInstructionTimeText, InstructionTimeReductionPerRoundText, InstructionTimeIncreasePerPlayerText, MinimumInstructionTimeText;

    public Text playerCountText;
    public Slider playerCountSlider;
    public Text toggleText;
    public Toggle easyPhoneInteractions;
    public Text phoneInteractionText;

    private void Start()
    {
        Lobby.SetActive(false);
        SetSettings();
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

        //GameSettings.RoundTime = int.Parse(roundTimeText.text);
        //GameSettings.PlayerCount = int.Parse(playerCountText.text);
        //GameSettings.BaseInstructionNumber = int.Parse(BaseInstructionNumberText.text);
        //GameSettings.InstructionNumberIncreasePerRound = int.Parse(InstructionNumberIncreasePerRoundText.text);
        //GameSettings.BaseInstructionTime = int.Parse(BaseInstructionTimeText.text);
        //GameSettings.InstructionTimeReductionPerRound = int.Parse(InstructionTimeReductionPerRoundText.text);
        //GameSettings.InstructionTimeIncreasePerPlayer = int.Parse(InstructionTimeIncreasePerPlayerText.text);
        //GameSettings.MinimumInstructionTime = int.Parse(MinimumInstructionTimeText.text);

        GameSettings.RoundTime = string.IsNullOrEmpty(roundTimeText.text) ? 90 : int.Parse(roundTimeText.text);
        GameSettings.BaseInstructionNumber = string.IsNullOrEmpty(BaseInstructionNumberText.text) ? 10 : int.Parse(BaseInstructionNumberText.text);
        GameSettings.InstructionNumberIncreasePerRound = string.IsNullOrEmpty(InstructionNumberIncreasePerRoundText.text) ? 5 : int.Parse(InstructionNumberIncreasePerRoundText.text);
        GameSettings.BaseInstructionTime = string.IsNullOrEmpty(BaseInstructionTimeText.text) ? 15 : int.Parse(BaseInstructionTimeText.text);
        GameSettings.InstructionTimeReductionPerRound = string.IsNullOrEmpty(InstructionTimeReductionPerRoundText.text) ? 2 : int.Parse(InstructionTimeReductionPerRoundText.text);
        GameSettings.InstructionTimeIncreasePerPlayer = string.IsNullOrEmpty(InstructionTimeIncreasePerPlayerText.text) ? 2 : int.Parse(InstructionTimeIncreasePerPlayerText.text);
        GameSettings.MinimumInstructionTime = string.IsNullOrEmpty(MinimumInstructionTimeText.text) ? 5 : int.Parse(MinimumInstructionTimeText.text);

        //GameSettings.PlayerCount = string.IsNullOrEmpty(playerCountText.text) ? 2 : int.Parse(playerCountText.text);
        GameSettings.PlayerCount = (int)playerCountSlider.value;
        GameSettings.EasyPhoneInteractions = easyPhoneInteractions.isOn;
        GameSettings.PhoneInteractionProbability = string.IsNullOrEmpty(phoneInteractionText.text) ? 42 : 2*int.Parse(phoneInteractionText.text);

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
        GameSettings.EasyPhoneInteractions = true;
        GameSettings.PhoneInteractionProbability = 14;
    }
    
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        //This hook allows you to apply state data from the lobby-player to the game-player
        //just subclass "LobbyHook" and add it to the lobby object.

        if (NetworkLobbyHook)
            NetworkLobbyHook.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

        return true;
    }

    public void UpdatePlayerCountText()
    {
        playerCountText.text = "Player Count: " + playerCountSlider.value;
    }

    public void UpdateToggleText()
    {
        if(easyPhoneInteractions.isOn){
            toggleText.text = "On";
        }
        else{
            toggleText.text = "Off";
        }
    }
}
