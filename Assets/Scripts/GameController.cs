using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using Random = System.Random;


public class GameController : NetworkBehaviour
{
    //Custom GameObjects
    public InstructionController InstructionController;
    public AnimationController animationController;
    private GameStateHandler gameStateHandler;
    public MusicPlayer MusicPlayer;

    //Unity GameObjects
    public Text scoreText, roundTimerText, scoreBarText, roundNumberText;
    public GameObject roundTimerBar, gameOverText, backButton, lobbyTopPanel;
    public Image stars;

    public List<Player> playerList = new List<Player>();
    public List<Text> playerNames = new List<Text>();

    public List<string> raceWinnersList = new List<string>();

    [SyncVar] public int score = 0;

    public int Score
    {
        get
        {
            return score;
        }
    }

    [SyncVar] private int roundScore = 0;
    [SyncVar] public int roundNumber = 1;

    [SyncVar] public bool isRoundPaused;
    [SyncVar] public bool isGameStarted;
    private bool isGameOver;
    
    //Group activity
    private bool isGroupActiviy = true;
    [SyncVar] public bool isGroupDone;

    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }

    public int RoundNumber
    {
        get
        {
            return roundNumber;
        }
    }

    [SyncVar] public int fireCount = 0;
    int stationCount = 4;

    public int FireCount
    {
        get
        {
            return fireCount;
        }
    }

    public bool IsRoundPaused
    {
        get
        {
            return isRoundPaused;
        }
    }

    public bool IsGameStarted
    {
        get
        {
            return isGameStarted;
        }
    }

    [SyncVar] public float roundTimeLeft;

    public float RoundTimeLeft
    {
        get
        {
            return roundTimeLeft;
        }
    }

    [SyncVar] public int roundStartTime;

    public int RoundStartTime
    {
        get
        {
            return roundStartTime;
        }
    }

    [SyncVar] public int instructionStartTime;
    [SyncVar] public int BaseInstructionNumber;
    [SyncVar] public int InstructionNumberIncreasePerRound;
    [SyncVar] public int BaseInstructionTime;
    [SyncVar] public int InstructionTimeReductionPerRound;
    [SyncVar] public int InstructionTimeIncreasePerPlayer;
    [SyncVar] public int MinimumInstructionTime;
    [SyncVar] public int playerCount;


    public int PlayerCount
    {
        get
        {
            return playerCount;
        }
    }

    [SyncVar] public bool easyPhoneInteractions;

    //[SyncVar(hook = "SetTopChef")] public string currentTopChef;
    [SyncVar] public string currentTopChef;

    [SyncVar] public float customerSatisfaction = 50;

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
    [FormerlySerializedAs("startGroupActivity")] [SyncVar] public bool groupActivityStarted;
    public int numberOfGroupActivities = 2;
    [SyncVar] public int activityNumber = 0;

    List<string> UserNames = new List<string>(); /* Just here so in future they can set their own usernames from the lobby */

    //Indicator variables for the animation controller
    public bool playersInitialised;

    public bool PlayersInitialised
    {
        get
        {
            return playersInitialised;
        }
    }

    //Functions-----------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------------------
    // FUNCTIONS ---------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        StartCoroutine(SetupGame(2));

        //Show server display only on the server
        if (isServer)
        {
            GetComponentInChildren<Canvas>().enabled = true;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animation>().Play();
            GameObject.FindGameObjectWithTag("LobbyTopPanel").SetActive(false);
            
            Debug.Log("Start");
        }
    }

    private IEnumerator SetupGame(int x)
    {
        yield return new WaitForSecondsRealtime(x);

        if (isServer)
        {
            LoadSettings();
        }

        StartCoroutine(Setup(5));
    }
    
    private IEnumerator Setup(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        
        //Find players
        var players = FindObjectsOfType<Player>();

        //Loop sets up playerList, links players to the GC and IC and sets player id
        int playerIndex = 0;
        foreach (Player p in players)
        {
            playerList.Add(p);
            if (isServer) p.SetGameController(this);
            p.SetInstructionController(InstructionController);
            p.SetPlayerId(playerIndex);
            p.instStartTime = CalculateInstructionTime();
            p.playerCount = playerCount;
            p.PlayerScore = 0;
            if (!easyPhoneInteractions) p.DisableOkayButtonsOnPanels();

            playerIndex++;
        }
        
        if (isServer)
        {
            GetComponentInChildren<Canvas>().enabled = true; //Show server display only on the server.
            playerIndex = 0;
            foreach (var p in players)
            {
                UserNames.Add(p.PlayerUserName);
                playerNames[playerIndex].text = p.PlayerUserName;
                playerNames[playerIndex].color = p.PlayerColour;
                playerIndex++;
            }
        }

        PlayersInitialisedFromStart();
        

        InstructionController.ICStart(playerCount, numberOfButtons, playerList, this);
        InstructionController.piProb = piProb;

        if (isServer)
        {
//            int j = 0;
//            foreach (var p in playerList)
//            {
////                UserNames.Add(p.PlayerUserName);
//                UserNames.Add("PLayer" + j);
//                j++;
//            }
            gameStateHandler = new GameStateHandler(UserNames); //Instantiate single gameStateHandler object on the server to hold gamestate data 
            
            StartCoroutine(RoundCountdown(10, "3"));
            StartCoroutine(RoundCountdown(11, "2"));            
            StartCoroutine(RoundCountdown(12, "1"));
            StartCoroutine(StartRound(13));
            StartCoroutine(StartGame(13));
        }

    }

    private IEnumerator StartGame(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        

        StartRoundTimer();
        UpdateScoreBar();
        isGameStarted = true;
        Debug.Log("StartGame");
    }

    private void Update()
    {
        if (isGameStarted)
        {
            
            //Show score and active instructions on server display.
            scoreText.text = score.ToString();
            stars.fillAmount = customerSatisfaction / 100;
            roundNumberText.text = roundNumber.ToString();
            UpdateRoundTimeLeft();

            if (isServer)
            {
                if ((score % 50 == 10) && isGroupActiviy) //Needs to be changed.
                {
                    Debug.Log("Call1");
                    InitiateGroupActivity();
                }

                else if (groupActivityStarted)
                {
                    Debug.Log("call2");

                    CheckGroupActivity();
                }
                else
                {
                    ResetGroup();
                }

            }

            if (roundMaxScore - roundScore <= 1)
            {
                PenultimateAction(true);
            }

            if (IsRoundComplete())
            {
                OnRoundComplete();
            }
            else if (roundTimeLeft < 0 || customerSatisfaction == 0)
            {
                SetTimerText("0");
                if (isServer) RpcGameOver();
                gameOverText.transform.SetAsLastSibling();
                gameOverText.SetActive(true);
                backButton.SetActive(true);

                if (!isGameOver)
                {
                    PlayGameOver();
                    GameOver();
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

    public void OnClickBack()
    {
        RpcQuitGame();
        Application.Quit();
    }

    [ClientRpc]
    public void RpcQuitGame()
    {
        foreach (Player p in playerList)
        {
            p.BackToMainMenu();
        }
    }

    [ClientRpc]
    public void RpcIncreaseScoreStreak(int playerID)
    {
        playerList[playerID].IncreaseScoreStreak();
    }

    [ClientRpc]
    public void RpcResetScoreSteak(int playerID)
    {
        playerList[playerID].ResetScoreStreak();
    }

    [ClientRpc]
    public void RpcScoreStreakCheck(int playerID)
    {
        playerList[playerID].ScoreStreakCheck();
    }

    [Server]
    public void CheckAction(int i)
    {
        if (isRoundPaused) return; //Do not check if round paused.
        IncreaseScore();
        RpcIncreaseScoreStreak(i);
        RpcScoreStreakCheck(i);
    }

    [Server]
    public void IncreaseFireCount()
    {
        fireCount++;
    }

    [Server]
    public void IncreaseScore()
    {
        score += 10;
        roundScore++;
        customerSatisfaction += 2.0f;
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

    public void PlayersInitialisedFromStart()
    {
        playersInitialised = true;
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
        UpdateGamestate();
        isRoundPaused = true;

        RoundPaused();
        fireCount = 0;
        CancelInvoke();
        PauseMusic();
        PlayRoundBreakMusic();
        RpcPausePlayers();

        foreach (Player p in playerList)
        {
            p.instStartTime = CalculateInstructionTime();
        }

        ReadyInstructionController();

        StartCoroutine(PlayXCountAfterNSeconds(8, 2));
        StartCoroutine(StartNewRoundAfterXSeconds(8));
        StartCoroutine(RoundCountdown(9, "2"));
        StartCoroutine(RoundCountdown(10, "1"));
        StartCoroutine(StartRound(11));
    }

    public void RoundPaused()
    {
        isRoundPaused = true;
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
            p.shopPanel.SetActive(false);
        }
    }

    private IEnumerator StartRound(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        Debug.Log("StartRound");
        var players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            playerNames[i].text = players[i].PlayerUserName;
            playerNames[i].color = players[i].PlayerColour;
            players[i].SetPlayerId(i);
        }
        PlayRoundMusic();
        ResetPlayers();
        ResetServer();
        RpcUnpausePlayers();
        isRoundPaused = false;
        PenultimateAction(false);
        roundMaxScore = CalculateInstructionNumber();
        customerSatisfaction = 50;
//        InvokeRepeating("DecreaseCustomerSatisfaction", 1.0f, 1.0f);
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
            player.roundCompletePanel.SetActive(true);
            if (player.topChefText.text == "YOU!!")
            {
                player.shopPanel.SetActive(true);
                player.roundCompletePanel.SetActive(false);
            }
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
        int topScore = 0;
        string topChef = null;
        var players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            Debug.Log("Player: " + player.PlayerUserName + " :: " + player.PlayerScore);
            if (player.PlayerScore > topScore)
            {
                topScore = player.PlayerScore;
                topChef = player.PlayerUserName;
            }
            //gameStateHandler.UpdatePlayerScore(player.PlayerUserName, player.PlayerScore);
            //player.PlayerScore = 0;
        }
        currentTopChef = topChef;
        RpcSetTopChef(topChef);        
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
                    - (roundNumber - 1) * InstructionTimeReductionPerRound
                    + (playerCount - 2) * InstructionTimeIncreasePerPlayer;

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
    
    private void DecreaseCustomerSatisfaction()
    {
        customerSatisfaction -= 1;
        if (customerSatisfaction > 100) customerSatisfaction = 100;
        if (customerSatisfaction < 0) customerSatisfaction = 0;
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
    
    public void GameOver()
    {
        isGameOver = true;
    }
    
    [ClientRpc]
    private void RpcSetTopChef(string topChef)
    {
        Debug.Log("TOP CHEF = " + topChef);
        foreach (var player in playerList)
        {
            player.roundScoreText.text = player.PlayerScore.ToString();
            if (topChef != null)
            {
                if (topChef == player.PlayerUserName)
                {
                    player.topChefText.text = "YOU!!";
                }
                else player.topChefText.text = topChef;
            }
        }
    }

    [ClientRpc]
    public void RpcSetGroupActivity(bool active)
    {
        Debug.Log("set group active");

        foreach (var player in playerList)
        {
            player.isGroupActive = active;
        }
    }

    private void CheckShake()
    {

        bool allReady = true;
        foreach (var player in playerList)
        {
            allReady &= player.isShaking;
        }

        if (allReady)
        {
            AllAreReady();
        }
    }

    private void CheckNFC()
    {
//        Debug.Log("list A");
//        foreach (var VARIABLE in raceWinnersList)
//        {
//            Debug.Log(VARIABLE);
//        }

        foreach (var player in playerList)
        {
            Debug.Log("is race started: " + player.isNFCRaceStarted);
            if (player.IsNFCRaceCompleted && !raceWinnersList.Contains(player.PlayerUserName))
            {
                raceWinnersList.Add(player.PlayerUserName);
            }
        }

//        Debug.Log("list B");
//        foreach (var VARIABLE in raceWinnersList)
//        {
//            Debug.Log(VARIABLE);
//        }

        if (raceWinnersList.Count == playerCount)
        {
            //TODO: Get finn to put this list on the board.
            foreach (var player in playerList)
            {
                for (int i = 0; i < raceWinnersList.Count; i++)
                {
                    if (player.PlayerUserName == raceWinnersList[i])
                    {
                        int scoreAdjustment = ( 10 * ( playerCount - i ) );
                        player.PlayerScore += scoreAdjustment;
                    }
                    Debug.Log(raceWinnersList[i]);
                }
            }
            
            AllAreReady();
        }
    }

    private void IncrementGroupActivity()
    {
        activityNumber = (activityNumber + 1) % numberOfGroupActivities;
//        activityNumber = 1;
    }

    private void RpcUpdateActivityNumber(int number)
    {
        foreach (var player in playerList)
        {
            if (player != null) player.activityNumber = number;
            else Debug.Log("not player at UpdateActivityNumber");
        }
    }

    [Server]
    private void InitiateGroupActivity()
    {
        
        Debug.Log("initiate");
        RpcNfcRaceAssignStation();
        groupActivityStarted = true;
        RpcSetGroupActivity(true);
        //TODO: HERE
        isGroupActiviy = false;     

    }
    

    [Server]
    private void CheckGroupActivity()
    {

        switch (activityNumber)
        {
            case 0: 
                CheckShake();
                break;
                    
            case 1:
                //NFC group race
                CheckNFC();
                break;
                    
            default:
                
                score = 9999;
                break;
        }
    }

    [ClientRpc]
    private void RpcNfcRaceAssignStation()
    {
        Random rand = new Random();
        int i = rand.Next(0, stationCount);
        
        foreach (var player in playerList)
        {
            player.nfcStation = i;
            i = (i + 1) % playerCount;
        }
    }
    
    [Server]
    private void ResetGroup()
    {
        RpcResetGroupActivity();
    }

    [Server]
    private void AllAreReady()
    {
        Debug.Log("All");

        score += 10;
        groupActivityStarted = false;
        RpcResetGroupActivity();
        isGroupActiviy = true;
        raceWinnersList = new List<string>();
        raceWinnersList.Clear();
        RpcNfcRaceAssignStation();
        Debug.Log("... Ready");
        IncrementGroupActivity();
    }
    
        
    [ClientRpc]
    public void RpcResetGroupActivity()
    {
        foreach (var player in playerList)
        {
            player.instTime = instructionStartTime;
            player.isGroupActive = false;
            player.isNFCRaceStarted = false;
            player.IsNFCRaceCompleted = false;
            player.wait = false;
            player.activityNumber = activityNumber;
        }
    }

}
