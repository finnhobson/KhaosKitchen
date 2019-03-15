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
    //Custom Objects
    public InstructionController InstructionController;
    public AnimationController animationController;
    private GameStateHandler gameStateHandler;
    public MusicPlayer MusicPlayer;

    public Text scoreText, roundTimerText, scoreBarText, roundNumberText;

    public GameObject roundTimerBar;
    public GameObject scoreBar;
    
    //Lists
    public List<Player> playerList = new List<Player>();
    List<string> userNames = new List<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }); /* Just here so in future they can set their own usernames from the lobby */
    private List<String> activeUserNames = new List<string>();
    
    //Synced primitives
    [SyncVar] public int score = 0;
    [SyncVar] private int roundScore = 0;
    [SyncVar] public float roundTimeLeft;
    [SyncVar] private int roundNumber = 1;
    
    //Booleans
    [SyncVar] public bool isRoundPaused = false;
    [SyncVar] public bool isGameStarted = false;
    private int piProb = 10;                       //Phone interaction probability = 2/x
    public bool playersInitialised = false;        //Indicator variables for the animation controller

    //Primitives
    private static int numberOfButtons = 4;
    public int playerCount;
    public float roundStartTime;
    public int roundStartScore;
    public int roundMaxScore;
    public float VolumeOfSoundEffects;

    private bool stopped = false;

    
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
            p.SetPlayerId(playerIndex);

            //New attributes for players to add to gameplayer, thoughts?
            p.PlayerScore = 0;
            p.PlayerUserName = userNames[playerIndex];
            activeUserNames.Add(p.PlayerUserName);
            p.VolumeOfSoundEffects = VolumeOfSoundEffects;
            
            playerIndex++;
            //playerCount = playerCount+1;
        }

        playersInitialised = true;
        
        //Setup the instruction controller
        //Debug.Log("player count = " + playerCount);
        //PlayerCount = playerCount;
        InstructionController.ICStart(playerCount, numberOfButtons, playerList, this);

        if (isServer)
        {
            GetComponentInChildren<Canvas>().enabled = true; //Show server display only on the server.
            gameStateHandler = new GameStateHandler(activeUserNames); //Instantiate single gameStateHandler object on the server to hold gamestate data 
        }

        StartCoroutine(PlayXCountAfterNSeconds(2, 2));
        StartCoroutine(RoundCountdown(3, "2"));
        StartCoroutine(RoundCountdown(4, "1"));
        StartCoroutine(StartRound(5));
        StartCoroutine(StartGame(5));
    }

    private IEnumerator StartGame(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        StartRoundTimer();
        UpdateScoreBar();
        isGameStarted = true;

    }

    private void Update()
    {
        if (isGameStarted)
        {
            //Show score and active instructions on server display.
            scoreText.text = score.ToString();
            roundNumberText.text = roundNumber.ToString();
            UpdateRoundTimeLeft();

            if (roundMaxScore - roundScore <= 1)
            {
                PenultimateAction(true);
            }

            if (IsRoundComplete())
            {
                OnRoundComplete();
            }

            else if (roundTimeLeft < 0)
            {
                SetTimerText("0");
                if (!stopped)
                {
                    PlayGameOver();
                    stopped = true;

                }

                foreach (Player p in playerList)
                {
                    p.GameOver();
                }
                
            }

            else
            {
                SetTimerText(roundTimeLeft.ToString("F2"));
            }
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
        roundScore++;
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
        roundTimerBar.GetComponent<RectTransform>().localScale = new Vector3(roundTimeLeft / roundStartTime, 1, 1);
        //SetTimerText(roundTimeLeft.ToString("F2"));
    }

    private void SetTimerText(string text)
    {
        roundTimerText.text = text;
    }

    private void UpdateScoreBar()
    {
        scoreBarText.text = (roundScore - roundStartScore).ToString() + " / " + roundMaxScore.ToString();
        scoreBar.GetComponent<RectTransform>().localScale = new Vector3((float)(roundScore - roundStartScore)/roundMaxScore, 1, 1);
    }

    private bool IsRoundComplete()
    {
        return (roundScore >= roundMaxScore);
    }

    /*
     * Updates gamestatehandler object with current data, as well as updating individual player scores.
     */
    private void OnRoundComplete()
    {
        if (!isServer || isRoundPaused) return; //Only need to access this function once per round completion.
        roundNumber++;
        isRoundPaused = true;
        
        PauseMusic();
        PlayRoundBreakMusic();

        RpcPausePlayers();
        ReadyInstructionController();
        UpdateGamestate();

        StartCoroutine(StartNewRoundAfterXSeconds(5));
        StartCoroutine(RoundCountdown(6, "2"));
        StartCoroutine(RoundCountdown(7, "1"));
        StartCoroutine(StartRound(8));

    }

    private IEnumerator PlayXCountAfterNSeconds(int n, int x)
    {
        yield return new WaitForSecondsRealtime(n);
        PlayCountDown(x);
    }
    
    private IEnumerator RoundCountdown(int n, string x)
    {
        int count = 1;
        Int32.TryParse(x, out count);
        yield return new WaitForSecondsRealtime(n);
        PlayCountDown(count - 1);
        RpcCountdown(x);
    }

    [ClientRpc]
    private void RpcCountdown(string x)
    {
        foreach (Player p in playerList) p.countdownText.text = x;
    }

    /*
     * Function that halts at yield line for x seconds.
     * After x seconds, players and server are reset and next round starts.
     */
    private IEnumerator StartNewRoundAfterXSeconds(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        PauseMusic();
        PlayCountDown(2);
        RpcStartNewRound();
    }

    [ClientRpc]
    private void RpcStartNewRound()
    {
        foreach (Player p in playerList)
        {
            p.roundNumberText.text = roundNumber.ToString();
            p.countdownText.text = "3";
            p.roundStartPanel.SetActive(true);
            p.roundCompletePanel.SetActive(false);
        }
    }

    private IEnumerator StartRound(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        ResetPlayers();
        ResetServer();
        RpcUnpausePlayers();
        isRoundPaused = false;
        PlayRoundMusic();
        PenultimateAction(false);
        PrintInstructionHandler();
    }

    private void ResetServer()
    {
        Debug.Log("RESET SERVER");
        roundScore = 0;
        UpdateScoreBar();
        StartRoundTimer();
    }

    [ClientRpc]
    public void RpcPausePlayers()
    {
        foreach (var player in playerList)
        {
            player.roundScoreText.text = roundScore.ToString();
            player.roundCompletePanel.SetActive(true);
            player.PausePlayer();
        }
    }

    [ClientRpc]
    public void RpcUnpausePlayers()
    {
        foreach (var player in playerList)
        {
            player.roundCompletePanel.SetActive(false);
            player.roundStartPanel.SetActive(false);
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
        InstructionController.UnPauseIC();
    }

    private void PenultimateAction(bool action)
    {
        InstructionController.PenultimateAction(action);
    }

    private void UpdateGamestate()
    {
        //Store round info
        gameStateHandler.OnRoundComplete(score);
        foreach (var player in playerList)
        {
            gameStateHandler.UpdatePlayerScore(player.PlayerUserName, player.PlayerScore);
            player.PlayerScore = 0;
        }
        gameStateHandler.PrintGameData();

        
    }

    private void PrintInstructionHandler()
    {
        InstructionController.PrintInstructionHandler();
    }

    public void PrintOut(int buttonNumber)
    {
        Debug.Log(buttonNumber);
    }
    
    private void PlayRoundBreakMusic()
    {
        MusicPlayer.PlayRoundBreak();
    }

    private void PlayRoundMusic()
    {
        MusicPlayer.StartRoundMusic();
    }

    private void PlayCountDown(int x)
    {
        MusicPlayer.PlayCountDown(x);
    }

    private void PauseMusic()
    {
        MusicPlayer.PauseMusic();
    }

    private void PlayGameOver()
    {
        MusicPlayer.PlayGameOver();
    }
}
