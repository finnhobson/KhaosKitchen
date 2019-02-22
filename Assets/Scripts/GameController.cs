using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameController : NetworkBehaviour
{

    public InstructionController InstructionController;

    public Text scoreText, instruction1, instruction2, instruction3, instruction4;

    [SyncVar]
    public int score = 0;

    private static int numberOfButtons = 4;

    public int playerCount = 3;

    public List<Player> playerList = new List<Player>();

    public float roundStartTime = 90;

    [SyncVar]
    public float roundTimeLeft;

    public GameObject roundTimerBar;
    public Text roundTimerText;

    public int roundStartScore = 0;
    public int roundMaxScore = 5;
    public GameObject scoreBar;
    public Text scoreBarText;

    private GameStateHandler gameStateHandler;
    
    List<String> userNames = new List<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }); /* Just here so in future they can set their own usernames from the lobby*/
    private List<String> activeUserNames = new List<String>();

    //Phone interaction probability = 2/x
    private int piProb = 15;

    void Start()
    {
        //Find players
        var players = FindObjectsOfType<Player>();
        
        //Loop sets up playerList, links players to the GC and IC and sets player id
        int playerIndex = 0;
        foreach (var p in players)
        {
            playerList.Add(p);
            p.SetGameController(this);
            p.SetInstructionController(InstructionController);
            p.setPlayerId(playerIndex);
            p.PlayerUserName = userNames[playerIndex];
            activeUserNames.Add(p.PlayerUserName);
            playerIndex++;
        }
        
        //Setup the instruction controller
        InstructionController.ICStart(playerCount, numberOfButtons, playerList, this);

        if (isServer)
        {
            GetComponentInChildren<Canvas>().enabled = true; //Show server display only on the server.
            gameStateHandler = new GameStateHandler(activeUserNames); //Instantiate single gameStateHandler object on the server to hold gamestate data
            
        }

        playerIndex = 0;
        //Assign actions to each player.
        foreach (Player p in players)
        {
            p.SetInstruction(InstructionController.ActiveInstructions[playerIndex]);
            playerIndex++;
        }
        StartRoundTimer();
        UpdateScoreBar();
        
    }

    private void Update()
    {
        //Show score and active instructions on server display.
        scoreText.text = score.ToString();
        UpdateRoundTimeLeft();
        if(roundTimeLeft<0){
            SetTimerText("0");
            foreach(Player p in playerList){
                p.GameOver();
            }
        }else{
            SetTimerText(roundTimeLeft.ToString("F2"));
        }
    }

    [ClientRpc]
    public void RpcIncreaseScoreSteak(int playerID)
    {
        playerList[playerID].IncreaseScoreStreak();
    }

    [ClientRpc]
    public void RpcResetScoreSteak(int playerID)
    {
        playerList[playerID].ResetScoreStreak();
    }

    [ClientRpc]
    public void RpcScoreSteakCheck(int playerID)
    {
        playerList[playerID].ScoreStreakCheck();
    }

    [Server]
    public void CheckAction(string action, int i)
    {
        IncreaseScore();
        RpcIncreaseScoreSteak(i);
        RpcScoreSteakCheck(i);
    }

    [Server]
    public void IncreaseScore()
    {
        score++;
        UpdateScoreBar();
    }

    public void StartRoundTimer()
    {
        roundStartTime = 90;
        roundTimeLeft = roundStartTime;
    }

    private void UpdateRoundTimeLeft()
    {
        roundTimeLeft -= Time.deltaTime;
        roundTimerBar.GetComponent<UnityEngine.UI.Image>().fillAmount = roundTimeLeft / roundStartTime;
        //SetTimerText(roundTimeLeft.ToString("F2"));
    }

    private void SetTimerText(string text)
    {
        roundTimerText.text = text;
    }

    private void UpdateScoreBar()
    {
        scoreBarText.text = (score - roundStartScore).ToString() + " / " + roundMaxScore.ToString();
        scoreBar.GetComponent<UnityEngine.UI.Image>().fillAmount = (float)(score - roundStartScore)/roundMaxScore;

    }
}