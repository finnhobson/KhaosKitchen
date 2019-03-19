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
    //Custom GameObjects
    public InstructionController InstructionController;
    public AnimationController animationController;
    private GameStateHandler gameStateHandler;
    public MusicPlayer MusicPlayer;

    //Unity GameObjects
    public Text scoreText, roundTimerText, scoreBarText, roundNumberText;
    public GameObject roundTimerBar;

    public List<Player> playerList = new List<Player>();
    
    [SyncVar] public int score = 0;
    [SyncVar] private int roundScore = 0;
    [SyncVar] public int roundNumber = 1;

    [SyncVar] public bool isRoundPaused = false;
    [SyncVar] public bool isGameStarted = false;

    [SyncVar] public float roundTimeLeft;
    [SyncVar] public int roundStartTime;
    [SyncVar] public int instructionStartTime;

    [SyncVar] public int BaseInstructionNumber;
    [SyncVar] public int InstructionNumberIncreasePerRound;
    [SyncVar] public int BaseInstructionTime;
    [SyncVar] public int InstructionTimeReductionPerRound;
    [SyncVar] public int InstructionTimeIncreasePerPlayer;
    [SyncVar] public int MinimumInstructionTime;

    [SyncVar] public int playerCount;
    [SyncVar] public bool easyPhoneInteractions;

    [SyncVar] public float customerSatisfaction = 100;

    //Phone interaction probability = 2/x
    [SyncVar] public int piProb = 21;

    public float pointMultiplier;

    private static int numberOfButtons = 4;
    //[SyncVar] public int playerCount = GameSettings.PlayerCount;
    //public float roundStartTime = 90;
    public int roundStartScore;
    public int roundMaxScore;
    public GameObject scoreBar;

    //Booleans
    private bool isGameOver = false;

    List<string> userNames = new List<string>(); /* Just here so in future they can set their own usernames from the lobby */



    //Indicator variables for the animation controller
    public bool playersInitialised = false;
    
    //Functions-----------------------------------------------------------------------------------------------------

    private void Start()
    {
        if (isServer)
        {
            LoadSettings();
        }

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
            p.instStartTime = CalculateInstructionTime();
            p.playerCount = playerCount;

            if(!easyPhoneInteractions){
                p.DisableOkayButtonsOnPanels();
            }

            //New attributes for players to add to gameplayer, thoughts?
            p.PlayerScore = 0;
            
            playerIndex++;
        }
        
        if (isServer)
        {
            GetComponentInChildren<Canvas>().enabled = true; //Show server display only on the server.
            foreach (var p in players)
            {
                userNames.Add(p.PlayerUserName);
                Debug.Log(p.PlayerId + " = " + p.PlayerUserName);
            }
            gameStateHandler = new GameStateHandler(userNames); //Instantiate single gameStateHandler object on the server to hold gamestate data 
        }

        playersInitialised = true;

        InstructionController.ICStart(playerCount, numberOfButtons, playerList, this);
        InstructionController.piProb = piProb;
        
        

        if (isServer)
        {
            GetComponentInChildren<Canvas>().enabled = true; //Show server display only on the server.
            gameStateHandler = new GameStateHandler(userNames); //Instantiate single gameStateHandler object on the server to hold gamestate data 


        }
        
        StartCoroutine(RoundCountdown(5, "3"));
        StartCoroutine(RoundCountdown(6, "2"));
        StartCoroutine(RoundCountdown(7, "1"));
        StartCoroutine(StartRound(8));
        StartCoroutine(StartGame(8));

//        StartCoroutine(PlayXCountAfterNSeconds(2, 2));
//        StartCoroutine(RoundCountdown(3, "2"));
//        StartCoroutine(RoundCountdown(4, "1"));
//        StartCoroutine(StartRound(5));
//        StartCoroutine(StartGame(5));
        
//        Debug.Log(PlayerCount);

//        StartCoroutine(StartRound(0));
//        StartCoroutine(StartGame(0));
        
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
                RpcGameOver();
                foreach (Player p in playerList)
                {
                    p.GameOver();
                }

                if (!isGameOver)
                {
                    PlayGameOver();
                    isGameOver = true;
                }
            }
            else
            {
                SetTimerText(roundTimeLeft.ToString("F2"));
            }
        }
    }

    [ClientRpc]
    private void RpcGameOver()
    {
        foreach (Player p in playerList) p.GameOver();
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
        //roundStartTime = 90;
        roundTimeLeft = roundStartTime;
    }

    private void UpdateRoundTimeLeft()
    {
        roundTimeLeft -= Time.deltaTime;
        if (roundTimeLeft >= 0) roundTimerBar.GetComponent<RectTransform>().localScale = new Vector3(roundTimeLeft / roundStartTime, 1, 1);
        //SetTimerText(roundTimeLeft.ToString("F2"));
    }

    private void SetTimerText(string text)
    {
        roundTimerText.text = text;
    }

    private void UpdateScoreBar()
    {
        scoreBarText.text = (roundScore - roundStartScore).ToString() + " / " + roundMaxScore.ToString();
        scoreBar.GetComponent<RectTransform>().localScale = new Vector3((float)(roundScore - roundStartScore) / roundMaxScore, 1, 1);
    }

    private bool IsRoundComplete()
    {
        return (roundScore >= roundMaxScore);
    }

    private void OnRoundComplete()
    {
        if (!isServer || isRoundPaused) return; //Only need to access this function once per round completion.
        roundNumber++;
        isRoundPaused = true;
        PauseMusic();
        PlayRoundBreakMusic();

        RpcPausePlayers();
        foreach (Player p in playerList)
        {
            p.instStartTime = CalculateInstructionTime();
        }
        ReadyInstructionController();
        UpdateGamestate();
        
        

        StartCoroutine(PlayXCountAfterNSeconds(5, 2));
        StartCoroutine(StartNewRoundAfterXSeconds(5));
        StartCoroutine(RoundCountdown(6, "2"));
        StartCoroutine(RoundCountdown(7, "1"));
        StartCoroutine(StartRound(8));
        //StartCoroutine(StartRound(0));

    }
    
    private IEnumerator Wait(float n)
    {
        yield return new WaitForSecondsRealtime(n);
    }

    private IEnumerator RoundCountdown(int n, string x)
    {
        yield return new WaitForSecondsRealtime(n);
        int count = 1;
        Int32.TryParse(x, out count);
        PlayCountDown(count - 1);
        RpcCountdown(x);
    }

    [ClientRpc]
    private void RpcCountdown(string x)
    {
        foreach (Player p in playerList) p.countdownText.text = x;
    }

    private IEnumerator StartNewRoundAfterXSeconds(int x)
    {
        yield return new WaitForSecondsRealtime(x);
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
        PlayRoundMusic();
        ResetPlayers();
        ResetServer();
        RpcUnpausePlayers();
        isRoundPaused = false;
        PenultimateAction(false);
//        PrintInstructionHandler();
        roundMaxScore = CalculateInstructionNumber();
        customerSatisfaction = 100;
        UpdateScoreBar();
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
            player.roundScoreText.text = player.PlayerUserName;
            player.roundScoreText.color = player.PlayerColour;
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
    }

    private void PrintInstructionHandler()
    {
        InstructionController.PrintInstructionHandler();
    }

    public void PrintOut(int buttonNumber)
    {
        Debug.Log(buttonNumber);
    }

    //50% reduction in Number with 8 players
    private int CalculateInstructionNumber()
    {
        float temp = ((BaseInstructionNumber + InstructionNumberIncreasePerRound * (roundNumber - 1))
                      * ((12f - (playerCount - 2f)) / 12f));

        int round = (int)Math.Ceiling(temp);

        return round > 0 ? round : 1; ;

        //return 40;
    }

    public int CalculateInstructionTime()
    {
        int temp = BaseInstructionTime
                    - (roundNumber-1) * InstructionTimeReductionPerRound
                    + (playerCount-2) * InstructionTimeIncreasePerPlayer;

        return temp > MinimumInstructionTime ? temp : MinimumInstructionTime;

        //return 40;
    }

    private void LoadSettings()
    {

        roundStartTime = GameSettings.RoundTime;
        playerCount = GameSettings.PlayerCount;
        easyPhoneInteractions = GameSettings.EasyPhoneInteractions;

        BaseInstructionNumber = GameSettings.BaseInstructionNumber;
        InstructionNumberIncreasePerRound = GameSettings.InstructionNumberIncreasePerRound;
        BaseInstructionTime = GameSettings.BaseInstructionTime;
        InstructionTimeReductionPerRound = GameSettings.InstructionTimeReductionPerRound;
        InstructionTimeIncreasePerPlayer = GameSettings.InstructionTimeIncreasePerPlayer;
        MinimumInstructionTime = GameSettings.MinimumInstructionTime;

        piProb = GameSettings.PhoneInteractionProbability;
    }

    private IEnumerator PlayXCountAfterNSeconds(int n, int x)
    {
        yield return new WaitForSecondsRealtime(n);
        PauseMusic();
        PlayCountDown(x);
    }
    
    [Server]
    private void PlayRoundBreakMusic()
    {
        MusicPlayer.PlayRoundBreak();
    }

    [Server]
    private void PlayRoundMusic()
    {
        MusicPlayer.StartRoundMusic();
    }

    [Server]
    private void PlayCountDown(int x)
    {
        MusicPlayer.PlayCountDown(x);
    }

    [Server]
    private void PauseMusic()
    {
        MusicPlayer.PauseMusic();
    }

    [Server]
    private void PlayGameOver()
    {
        MusicPlayer.PlayGameOver();
    }
}
