using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class InstructionController : NetworkBehaviour
{
    public GameController GameController;
    
    //Store of the combinations of instructions possible
    private static List<String> verbList = new List<string>(new string[] { "Grab", "Fetch", "Grate", "Grill", "Melt", "Serve", "Stir", "Chop", "Cut", "Mash", "Season", "Flambé", "Bake", "Fry", "Taste", "Microwave" });
    private static List<String> nounList = new List<string>(new string[] { "Minced Beef", "Steak", "Pork Loin", "Ice Cream", "Strawberry", "Bannana", "Bun", "Toast", "Chocolate", "Pasta", "Bacon", "Tomato", "Sugar", "Salt", "Lettuce", "Sauce", "Mustard" });
   
    private static List<String> nfcInstructions = new List<string>(new string[] { "", "" });
    private static List<String> micInstructions = new List<string>(new string[] { " Sous Chef made a deeply offensive comment!\n Shout some sense into him! It's 2018 ffs!\n\n (SHOUT INTO THE MIC)",
        " Waiters won't take the food out fast enough!\n Shout at them to work harder!\n\n (SHOUT INTO THE MIC)",
        " Your team are being incompetent!\n Shout some hurtful words at them!\n\n (SHOUT INTO THE MIC)"});
    private static List<String> shakeInstructions = new List<string>(new string[] { " Chef underseasoned the dish!\n Shake to salt the food!\n\n (SHAKE YOUR PHONE)",
        " Food runner dropped the dish!\n Shake some sense into the boy!\n\n (SHAKE YOUR PHONE)",
        " Pan set on fire!\n Shake to put it out!\n\n (SHAKE YOUR PHONE)"});
    
    
    private SyncListString activeButtonActions = new SyncListString();
    private SyncListString activeInstructions = new SyncListString();
    
    public SyncListString ActiveButtonActions
    {
        get { return activeButtonActions; }
        set { activeButtonActions = value; }
    }
    public SyncListString ActiveInstructions
    {
        get { return activeInstructions; }
        set { activeInstructions = value; }
    }

    //Variables that are generated in GC
    public List<Player> Players { get; set; }
    public int NumberOfButtons { get; set; }
    public int PlayerCount { get; set; }


    //Phone interaction probability = 2/x
    private int piProb = 15;

    /*
     * Called from GC, this is where the IC is setup. 
    */
    public void ICStart(int playerCount, int numberOfButtons, List<Player> players, GameController gameController)
    {
        //Assignment ensures that GC has generated values
        PlayerCount = playerCount;
        NumberOfButtons = numberOfButtons;
        Players = players;

        if (isServer)
        {
            SelectButtonActions();  //Create synced list of executables, one for each button in the game
            SetFirstInstructions(); //Select one instruction per player from Action Button List
        }
        
        //Assign actions and instructions to each player.
        foreach (var player in Players)
        {
            for (int i = 0; i < NumberOfButtons; i++)
            {
                player.SetActionButtons(activeButtonActions[player.PlayerId*numberOfButtons + i], i); 
            }
            player.SetInstruction(ActiveInstructions[player.PlayerId]);
        }
    }

    /*
     * Reset the active instructions and buttons.
     */
    [Server]
    public void ResetIC()
    {
        ActiveInstructions.Clear();
        ActiveButtonActions.Clear();
        int k = 0;
        foreach (var player in Players)
        {
            ActiveInstructions.Add(player.PlayerUserName);
            for (int j = 0; j < NumberOfButtons; j++)
            {
                ActiveButtonActions.Add(k.ToString());
                k++;
            }   
        }
    }

    [ClientRpc]
    public void RpcResetPlayers()
    {
        //Assign actions and instructions to each player.
        foreach (var player in Players)
        {
            for (int i = 0; i < NumberOfButtons; i++)
            {
                player.SetActionButtons(activeButtonActions[player.PlayerId*NumberOfButtons + i], i); 
            }
            player.SetInstruction(ActiveInstructions[player.PlayerId]);
        }
    }

    /*
     * Creates synced list of executable strings, one for each button.
     */
    public void SelectButtonActions()
    {
        ActiveButtonActions.Clear(); 
        
        int randomIndex;
        bool duplicate;
        int verbNo, nounNo;
        string text;
        
        for (int i = 0; i < PlayerCount*NumberOfButtons; i++)
        {
            duplicate = true;
            while (duplicate)
            {
                verbNo = UnityEngine.Random.Range(0, nounList.Count-1);
                nounNo = UnityEngine.Random.Range(0, verbList.Count-1); ;
                text = verbList[verbNo] + " " + nounList[nounNo];

                if (ActiveButtonActions.Contains(text) == false)
                {
                    ActiveButtonActions.Add(text);
                    duplicate = false;
                }
            }
        }
    }

    /*
     * Randomly select an instruction per player from ActiveButtonActions and add to ActiveInstruction.
     * Update same lists on GC.
     */
    private void SetFirstInstructions()
    {
        ActiveInstructions.Clear();
        for (int i = 0; i < PlayerCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);  /* How do we ensure we do not get a duplicate here? */
            while (ActiveInstructions.Contains(ActiveButtonActions[randomIndex]))
            {
                randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);  /* How do we ensure we do not get a duplicate here? */
            }
            ActiveInstructions.Add(ActiveButtonActions[randomIndex]);
        }
    }
    
    /*
     * Function called from player when action attempted.
     * If action is a current instruction then generate new instruction and order GC to handle score.
     */
    [Server]
    public void CheckAction(string action)
    {
        //When an action button is pressed by a player-client, check if action matches an active instruction.
        for (int i = 0; i < ActiveInstructions.Count; i++)
        {
            //If match, increment score.
            if (action == ActiveInstructions[i])
            {
                GameController.CheckAction(action, i);
                PickNewInstruction(i);      
                RpcUpdateInstruction(ActiveInstructions[i], i);
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
    
    /*
     * Checks if failed action is a current active instruction, and if so pick new instruction and orders NFC bin.
     */
    public void FailAction(string action)
    {
        //When an action button is pressed by a player-client, check if action matches an active instruction.
        for (int i = 0; i < ActiveInstructions.Count; i++)
        {
            //If match, increment score.
            if (action == ActiveInstructions[i])
            {
                GameController.RpcResetScoreSteak(i);
                PickNewInstruction(i);
                RpcUpdateInstruction(ActiveInstructions[i], i);
                RpcSetNfcPanel(i, "You f*cked the order! " + System.Environment.NewLine
                                                           + "Run to the bin!" + System.Environment.NewLine + System.Environment.NewLine
                                                           + "(TAP ON BIN NFC)");
            }
        }
    }

    /*
     * New Instruction selected from set of ActiveButtonActions, with no duplicate ensured.
     */
    public void PickNewInstruction(int index)
    {
        int randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);
        
        while (ActiveInstructions.Contains(ActiveButtonActions[randomIndex]))
        {
            randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);
        }
        
        ActiveInstructions[index] = ActiveButtonActions[randomIndex];

    }
    
    [ClientRpc]
    public void RpcUpdateInstruction(String newInstruction, int playerID)
    {
        Players[playerID].SetInstruction(newInstruction);
    }

    [ClientRpc]
    public void RpcUpdateButtons(int playerID)
    {
        for (int i = 0; i < NumberOfButtons; i++)
        {
//            Players[i].SetActionButtons(activeButtonActions[Players[i].PlayerId*NumberOfButtons + i], i);
            Players[i].SetActionButtons(playerID.ToString(), i);
        }
    }

    [ClientRpc]
    public void RpcStartInstTimer(int playerID)
    {
        Players[playerID].StartInstTimer();
    }
    
    [ClientRpc]
    public void RpcSetNfcPanel(int playerID, string text)
    {
        Players[playerID].SetNfcPanel(text);
    }

    [ClientRpc]
    public void RpcSetMicPanel(int playerID, string text)
    {
        Players[playerID].SetMicPanel(text);
    }

    [ClientRpc]
    public void RpcSetShakePanel(int playerID, string text)
    {
        Players[playerID].SetShakePanel(text);
    }
    
    [ClientRpc]
    public void RpcShowPaused()
    {
        foreach (var player in Players)
        {
            player.SetInstruction(player.PlayerUserName + " is paused");
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
