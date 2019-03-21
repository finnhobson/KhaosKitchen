using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;


public class InstructionController : NetworkBehaviour
{
    public GameController GameController;
    public InstructionHandler InstructionHandler;
    
    //Store of the combinations of instructions possible
    private static List<String> verbList = new List<string>(new string[] { "Grab", "Fetch", "Grate", "Grill", "Melt", "Serve", "Stir", "Chop", "Cut", "Mash", "Season", "Flambé", "Bake", "Fry", "Taste", "Microwave", "Tendorise", "Roast", "Cry Into", "Sneeze On" });
    private static List<String> nounList = new List<string>(new string[] { "Minced Beef", "Steak", "Pork Loin", "Ice Cream", "Strawberry", "Bannana", "Bun", "Toast", "Chocolate", "Pasta", "Bacon", "Tomato", "Sugar", "Salt", "Lettuce", "Sauce", "Mustard", "Sausage", "Chicken", "Ice Cubes" });
    private static List<String> nfcInstructions = new List<string>(new string[] { "Darn! Run to the bin!\n\n\n(RUN TO NFC)", "Quickly! Get that rubbish out of here!\n\n\n(RUN TO NFC)" });
    private static List<String> micInstructions = new List<string>(new string[] { " Sous Chef made a deeply offensive comment!\n Shout some sense into him! It's 2019 ffs!\n\n (SHOUT INTO THE MIC)",
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

    //Booleans
    [SyncVar] public bool isRoundPaused = false;
    [SyncVar] public bool isLastActionOfRound = false;
    [SyncVar] public bool SetupFinished = false;

    //Phone interaction probability = 2/x
    [SyncVar] public int piProb = 21;

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
            InstructionHandler = new InstructionHandler();
            SelectButtonActions();  //Create synced list of executables, one for each button in the game.
            SetFirstInstructions(); //Select one instruction per player from Action Button List.
            SetupFinished = true;
        }

        while (!SetupFinished)
        {
            //Just in case there is a timing issue with the setup.
        }
        
        //Assign actions and instructions to each player.
        foreach (var player in Players)
        {
            for (int i = 0; i < NumberOfButtons; i++)
            {
                string action = activeButtonActions[player.PlayerId * numberOfButtons + i];
                player.SetActionButtons(action, i);

                if (!isServer) continue;
                InstructionHandler.SetButtonNumber(action, i);
                InstructionHandler.SetButtonPlayerID(action, player.PlayerId);
            }

            string instruction = ActiveInstructions[player.PlayerId];
            player.SetInstruction(instruction);
            if(isServer) InstructionHandler.SetInstructionPlayerID(instruction, player.PlayerId);
        }
                
    }

    /*
     * Reset the active instructions and buttons.
     */
    [Server]
    public void ResetIC()
    {
        ClearInstructionHandler();
        SelectButtonActions();  //Create synced list of executables, one for each button in the game
        SetFirstInstructions(); //Select one instruction per player from Action Button List
    }

    [ClientRpc]
    public void RpcResetPlayers()
    {
        //Assign actions and instructions to each player.
        foreach (var player in Players)
        {
            for (int i = 0; i < NumberOfButtons; i++)
            {
                string action = activeButtonActions[player.PlayerId * NumberOfButtons + i];

                player.SetActionButtons(action, i);
            }
            string instruction = ActiveInstructions[player.PlayerId];
            player.SetInstruction(instruction);
        }
        
//        if(isServer) InstructionHandler.PrintInstructions();

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
                nounNo = UnityEngine.Random.Range(0, verbList.Count-1); 
                text = verbList[verbNo] + " " + nounList[nounNo];

                if (ActiveButtonActions.Contains(text)) continue;
                ActiveButtonActions.Add(text);
                InstructionHandler.AddValue(text, new Instruction(){IsActive = false, ButtonNumber = 69, InstructionPlayerID = 69, ButtonPlayerID = 69, InstructionTimer = 69f} );
                duplicate = false;
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
            InstructionHandler.SetIsActive(ActiveButtonActions[randomIndex]);
        }
    }
    
    /*
     * Function called from player when action attempted.
     * If action is a current instruction then generate new instruction and order GC to handle score.
     */
    [Server]
    public void CheckAction(string action)
    {
        if (isRoundPaused) return; //Do nothing when round paused.
        bool match = false;

        //When an action button is pressed by a player-client, check if action matches an active instruction.
        for (int i = 0; i < ActiveInstructions.Count; i++)
        {
            //If match, increment score.
            if (action != ActiveInstructions[i]) continue;

            match = true;
            DeactivateInstruction(action);
            GameController.CheckAction(action, i);

            PickNewInstruction(i, action);
            if (!isLastActionOfRound)
            {
                RpcUpdateInstruction(ActiveInstructions[i], i);
                RpcStartInstTimer(i);
            }

            //Update player score
            Players[i].PlayerScore++;
            
            //Only do a panel action if there are still instructions left in the round.
            if (isLastActionOfRound) return;
//            PrintInstructionHandler();

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
            else if (rand == 3)
            {
                rand = UnityEngine.Random.Range(0, nfcInstructions.Count);
                RpcSetNfcPanel(i, nfcInstructions[rand]);
            }
        }
        if (!match) GameController.fireCount++;
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
                
                PickNewInstruction(i, action);
                RpcUpdateInstruction(ActiveInstructions[i], i);
                RpcSetNfcPanel(i, "You missed an order! " + System.Environment.NewLine
                                                           + "Run to the bin!" + System.Environment.NewLine + System.Environment.NewLine
                                                           + "(TAP ON BIN NFC)");
            }
        }
    }

    /*
     * New Instruction selected from set of ActiveButtonActions, with no duplicate ensured.
     */
    public void PickNewInstruction(int index, string action)
    {
        InstructionHandler.InstructionCompleted(action);

        int randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);
        
        while (ActiveInstructions.Contains(ActiveButtonActions[randomIndex]))
        {
            randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);
        }
        
        ActiveInstructions[index] = ActiveButtonActions[randomIndex];
        
        InstructionHandler.SetIsActive(ActiveButtonActions[randomIndex]);
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
    public void RpcSetActionButton(int playerID, string action, int buttonNumber)
    {
        foreach (var player in Players)
        {
            if (player.PlayerId != playerID) return;
            player.SetActionButtons(action, buttonNumber);
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

    public void PauseIC()
    {
        isRoundPaused = true;
    }
    
    public void UnPauseIC()
    {
        isRoundPaused = false;
    }

    public void PenultimateAction(bool fromGC)
    {
        isLastActionOfRound = fromGC;
    }
   
    public void PlayerUpdateButton(int buttonNumber, string action, int playerID)
    {
        InstructionHandler.SetButtonPlayerID(action, playerID);
        InstructionHandler.SetButtonNumber(action, buttonNumber);
//        InstructionHandler.AddValue(action, new Instruction() {IsActive = false, ButtonNumber = buttonNumber, ButtonPlayerID = playerID});
    }

    public void PlayerUpdateInstruction(string instruction, int playerID)
    {
        InstructionHandler.SetInstructionPlayerID(instruction, playerID);
        InstructionHandler.SetIsActive(instruction);
    }

    public void PrintInstructionHandler()
    {
        InstructionHandler.PrintInstructions();
    }

    private void ClearInstructionHandler()
    {
        InstructionHandler.ClearInstructions();
    }

    private void DeactivateInstruction(string instruction)
    {
        InstructionHandler.SetNotActive(instruction);
    }
}
