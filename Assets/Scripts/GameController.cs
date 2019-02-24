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
    
    public Text scoreText, instruction1, instruction2, instruction3, instruction4;
    public Text roundTimerText;
    public Text scoreBarText;

    public GameObject roundTimerBar;
    
    public List<Player> playerList = new List<Player>();

    [SyncVar] public int score = 0;
    [SyncVar] public float roundTimeLeft;
    
    [SyncVar] 
    public bool isRoundPaused = false;
    
    private static int numberOfButtons = 4;
    public int playerCount;
    public float roundStartTime = 90;
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

        if ( isRoundComplete())
        {
            onRoundComplete();
        }
        
        else if(roundTimeLeft<0){
            SetTimerText("0");
            foreach(Player p in playerList){
                p.GameOver();
            }
        }
        else{
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
        ShowPauseIC();
        ResetIC();

        //Store round info
        gameStateHandler.onRoundComplete(score);
        foreach (var player in playerList)
        {
            gameStateHandler.updatePlayerScore(player.PlayerUserName, player.PlayerScore);
        }

        var time = 5;
        StartCoroutine(pausePlayersForXSeconds(time));

//        while (time>0)
//        {
//            //Wait
//            time -= Time.deltaTime;
//        }
//        RpcUnpausePlayers();
    }

    /*
     * Function that halts at yield line for x seconds.
     * After x seconds, players and server are reset and next round started.
     */
    private IEnumerator pausePlayersForXSeconds(int x)
    {
        print(Time.time);
        yield return new WaitForSecondsRealtime(x);
        print(Time.time);
        ResetPlayers();
        resetServer();
        RpcUnpausePlayers();
        isRoundPaused = false;
    }

    private void resetServer()
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

    private void ResetIC()
    {
        InstructionController.ResetIC();
    }

    private void ResetPlayers()
    {
        InstructionController.RpcResetPlayers();
    }

    private void ShowPauseIC()
    {
        InstructionController.RpcShowPaused();
    }

}