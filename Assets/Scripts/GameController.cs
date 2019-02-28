using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameController : NetworkBehaviour
{
    public InstructionController InstructionController;
    private GameStateHandler gameStateHandler;

    public Text scoreText;
    public Text roundTimerText;
    public Text scoreBarText;

    public GameObject roundTimerBar;
    
    public List<Player> playerList = new List<Player>();

    [SyncVar] public int score = 0;
    [SyncVar] public float roundTimeLeft;
    
    [SyncVar] 
    public bool isRoundPaused = false;

    private const int numberOfButtons = 4;
    public int playerCount;
    private float roundStartTime = 1000;
    public int roundStartScore;
    public int roundMaxScore;
    public GameObject scoreBar;
    
    List<string> userNames = new List<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }); /* Just here so in future they can set their own usernames from the lobby */
    private List<String> activeUserNames = new List<string>();

    //Phone interaction probability = 2/x
    private int piProb = 15;
    
    private void Start()
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

            //New attributes for players to add to gameplayer, thoughts?
            p.PlayerScore = 0;
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

        StartRoundTimer();
        UpdateScoreBar();
        
    }

    private void Update()
    {
        //Show score and active instructions on server display.
        scoreText.text = score.ToString();
        UpdateRoundTimeLeft();

        if (roundMaxScore - score <= 1)
        {
            PenultimateAction(true);
        }
        
        if ( isRoundComplete())
        { 
            onRoundComplete();
        }
        
        else if(roundTimeLeft<0)
        {
            SetTimerText("0");
            foreach(Player p in playerList){
                p.GameOver();
            }
        }
        
        else
        {
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
        if (isRoundPaused) return; //Do not check if round paused.
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

    private bool isRoundComplete()
    {
        return (score >= roundMaxScore);
    }

    /*
     * Updates gamestatehandler object with current data, as well as updating individual player scores.
     */
    private void onRoundComplete()
    {
        if (!isServer || isRoundPaused) return; //Only need to access this function once per round completion.
        isRoundPaused = true;

        RpcPausePlayers();
        ReadyInstructionController();
        UpdateGamestate();

        var time = 5;
        StartCoroutine(RestartNewRoundAfterXSeconds(time));
    }

    /*
     * Function that halts at yield line for x seconds.
     * After x seconds, players and server are reset and next round starts.
     */
    private IEnumerator RestartNewRoundAfterXSeconds(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        ResetPlayers();
        ResetServer();
        RpcUnpausePlayers();
        isRoundPaused = false;
        PenultimateAction(false);
        PrintInstructionHandler();
    }

    private void ResetServer()
    {
        Debug.Log("RESET SERVER");
        score = 0; //This has to be called to break out the loop in Update
        UpdateScoreBar();
        StartRoundTimer();
    }

    [ClientRpc]
    public void RpcPausePlayers()
    {
        foreach (var player in playerList)
        {
            player.PausePlayer();
        }
    }

    [ClientRpc]
    public void RpcUnpausePlayers()
    {
        foreach (var player in playerList)
        {
            player.UnpausePlayer();
        }
    }

    /*
     * Pauses IC to prevent actions from working while paused.
     * Generate pause message on players.
     * Generate new active button actions and corresponding active instructions.
     */
    private void ReadyInstructionController()
    {
        InstructionController.PauseIC();
        InstructionController.RpcShowPaused();
        InstructionController.ResetIC();
    }

    private void ResetPlayers()
    {
        InstructionController.RpcResetPlayers();
        InstructionController.UnpauseIC();
    }

    private void PenultimateAction(bool action)
    {
        InstructionController.PenultimateAction(action);
    }

    private void UpdateGamestate()
    {
        //Store round info
        gameStateHandler.onRoundComplete(score);
        foreach (var player in playerList)
        {
            gameStateHandler.updatePlayerScore(player.PlayerUserName, player.PlayerScore);
            player.PlayerScore = 0;
        }
        gameStateHandler.printGameData();

        
    }

    private void PrintInstructionHandler()
    {
        InstructionController.PrintInstructionHandler();
    }

    public void PrintOut(int buttonNumber)
    {
        Debug.Log(buttonNumber);
    }
}