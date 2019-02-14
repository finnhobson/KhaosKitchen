using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameController : NetworkBehaviour
{

    public Text scoreText, instruction1, instruction2, instruction3, instruction4;

    [SyncVar]
    public int score = 0;

    SyncListString activeInstructions = new SyncListString();

    SyncListString activeActions = new SyncListString();

    private static int numberOfButtons = 4;

    public int playerCount = 2;

    public List<Player> playerList = new List<Player>();

    private static List<String> instructions = new List<string>(new string[] { "Grab Meat", "Grab Salad", "Grab Buns", "Grab Cheese", "Grind Meat", "Chop Salad", "Cut Bun", "Wash Salad", "Mash Potato",
        "Grill Meat", "Flip Pancake", "Roast Chicken", "Grab Pasta", "Grab Salad", "Grab Sauce", "Grab Cheese", "Boil Pasta", "Chop Salad", "Mix-in Sauce", "Serve Pasta", "Grate Parmesan", "Grind Pepper",
        "Drain Pasta", "Cook Beef" });

    private static List<String> nfcInstructions = new List<string>(new string[] { "", "" });
    private static List<String> micInstructions = new List<string>(new string[] { " Sous Chef made a deeply offensive comment!\n Shout some sense into him!\n\n (SHOUT INTO THE MIC)",
                                                                                  " Waiters won't take the food out fast enough!\n Shout at them to work harder!\n\n (SHOUT INTO THE MIC)",
                                                                                  " Your team are being incompetent!\n Shout some sense into them!\n\n (SHOUT INTO THE MIC)"});

    private static List<String> shakeInstructions = new List<string>(new string[] { " Chef underseasoned the dish!\n Shake to salt the food!\n\n (SHAKE YOUR PHONE)",
                                                                                    " Food runner dropped the dish!\n Shake some sense into the boy!\n\n (SHAKE YOUR PHONE)",
                                                                                    " Pan set on fire!\n Shake to put it out!\n\n (SHAKE YOUR PHONE)"});

    private static List<String> verbList = new List<string>(new string[] { "Grab", "Fetch", "Grate", "Grill", "Melt", "Serve", "Stir", "Chop", "Cut", "Mash", "Season", "Flambé", "Bake", "Fry", "Taste", "Microwave" });
    private static List<String> nounList = new List<string>(new string[] { "Minced Beef", "Steak", "Pork Loin", "Ice Cream", "Strawberry", "Bannana", "Bun", "Toast", "Chocolate", "Pasta", "Bacon", "Tomato", "Sugar", "Salt", "Lettuce", "Sauce", "Mustard" });

    public float roundStartTime = 90;

    [SyncVar]
    public float roundTimeLeft;

    public GameObject roundTimerBar;
    public Text roundTimerText;

    public int roundStartScore = 0;
    public int roundMaxScore = 20;
    public GameObject scoreBar;
    public Text scoreBarText;

    //Phone interaction probability = 2/x
    private int piProb = 10;

    void Start()
    {
        //Find players
        var players = FindObjectsOfType<Player>();
        //playerCount = players.Length;

        if (isServer)
        {
            //Show server display only on the server.
            GetComponentInChildren<Canvas>().enabled = true;
            SelectActions();
            SetFirstInstructions(); 
        }

        //Assign actions to each player.
        int playerIndex = 0;
        foreach (Player p in players)
        {
            playerList.Add(p);
            p.SetGameController(this);
            p.SetInstruction(activeInstructions[playerIndex]);
            p.setPlayerId(playerIndex);

            for (int i = 0; i < numberOfButtons; i++)
            {
                p.SetActionButtons(activeActions[playerIndex*numberOfButtons + i], i);
            }
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

    private void SelectActions()
    {
        activeActions.Clear();
        int randomIndex;
        bool duplicate;
        int verbNo, nounNo;
        string text;
        for (int i = 0; i < playerCount*4; i++)
        {
            duplicate = true;
            while (duplicate)
            {
                //randomIndex = UnityEngine.Random.Range(0, instructions.Count);
                //if (activeActions.Contains(instructions[randomIndex]) == false)
                //{
                //    activeActions.Add(instructions[randomIndex]);
                //    duplicate = false;
                //}

                verbNo = UnityEngine.Random.Range(0, nounList.Count-1);
                nounNo = UnityEngine.Random.Range(0, verbList.Count-1); ;
                text = verbList[verbNo] + " " + nounList[nounNo];

                if (activeActions.Contains(text) == false)
                {
                    activeActions.Add(text);
                    duplicate = false;
                }
            }
        }
    }

    private void SetFirstInstructions()
    {
        activeInstructions.Clear();
        for (int i = 0; i < playerCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, activeActions.Count);
            activeInstructions.Add(activeActions[randomIndex]);
        }
    }

    [ClientRpc]
    public void RpcUpdateInstruction(String newInstruction, int playerID)
    {
        playerList[playerID].SetInstruction(newInstruction);
    }
    
    [ClientRpc]
    public void RpcUpdateButtons(String newbutton, int playerID, int buttonNumber)
    {
        playerList[playerID].SetActionButtons(newbutton, buttonNumber);
    }

    [ClientRpc]
    public void RpcStartInstTimer(int playerID)
    {
        playerList[playerID].StartInstTimer();
    }

    [ClientRpc]
    public void RpcSetNfcPanel(int playerID, string text)
    {
        playerList[playerID].SetNfcPanel(text);
    }

    [ClientRpc]
    public void RpcSetMicPanel(int playerID, string text)
    {
        playerList[playerID].SetMicPanel(text);
    }

    [ClientRpc]
    public void RpcSetShakePanel(int playerID, string text)
    {
        playerList[playerID].SetShakePanel(text);
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

    private void PickNewInstruction(int index)
    {
        bool duplicate = true;

        while (duplicate)
        {
            int randomIndex = UnityEngine.Random.Range(0, activeActions.Count);
            if (activeInstructions.Contains(activeActions[randomIndex]) == false)
            {
                activeInstructions[index] = activeActions[randomIndex];
                duplicate = false;
            } 
        }
    }

    [Server]
    public void CheckAction(string action)
    {
        //When an action button is pressed by a player-client, check if action matches an active instruction.
        for (int i = 0; i < activeInstructions.Count; i++)
        {
            //If match, increment score.
            if (action == activeInstructions[i])
            {
                IncreaseScore();
                RpcIncreaseScoreSteak(i);
                RpcScoreSteakCheck(i);
                PickNewInstruction(i);      
                RpcUpdateInstruction(activeInstructions[i], i);
                RpcStartInstTimer(i);
                int rand = UnityEngine.Random.Range(1, piProb);
                if(rand==1){
                    rand = UnityEngine.Random.Range(0, micInstructions.Count);
                    RpcSetMicPanel(i, micInstructions[rand]);
                }
                else if (rand == 2)
                {
                    rand = UnityEngine.Random.Range(0, shakeInstructions.Count);
                    RpcSetShakePanel(i, shakeInstructions[rand]);

                }
            }
        }
    }

    [Server]
    public void FailAction(string action)
    {
        //When an action button is pressed by a player-client, check if action matches an active instruction.
        for (int i = 0; i < activeInstructions.Count; i++)
        {
            //If match, increment score.
            if (action == activeInstructions[i])
            {
                RpcResetScoreSteak(i);
                PickNewInstruction(i);
                RpcUpdateInstruction(activeInstructions[i], i);
                RpcSetNfcPanel(i, "You messed up the order! " + System.Environment.NewLine
                               + "Run to the bin!" + System.Environment.NewLine + System.Environment.NewLine
                               + "(TAP ON BIN NFC)");
            }
        }
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