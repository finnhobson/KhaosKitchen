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
    
    //Store of the combinations of instructions possible
    private static List<String> verbList = new List<string>(new string[] { "Grab", "Fetch", "Grate", "Grill", "Melt", "Serve", "Stir", "Chop", "Cut", "Mash", "Season", "Flambé", "Bake", "Fry", "Taste", "Microwave", "Tendorise", "Roast", "Cry Into", "Mince", "Juice", "Freeze", "Purée", "Sneeze On", "Dice", "Cube", "Boil", "Brine", "Sous Vide", "Slice", "Poach",  "Deep Fry", "Lick", "Inhale", "Smell" });
    private static List<String> nounList = new List<string>(new string[] { "Minced Beef", "Steak", "Pork Loin", "Ice Cream", "Strawberry", "Bannana", "Toast", "Chocolate", "Pasta", "Bacon", "Tomato", "Sugar", "Salt", "Lettuce", "Sauce", "Mustard", "Sausage", "Apple", "Orange", "Chicken", "Ice Cubes", "Cheese", "Chicken Nuggets", "Brie", "Cheddar", "Camembert", "Wine", "Beer", "Whiskey", "Vodka", "Wasabi", "Salmon", "Tuna", "Mushroom", "Lard", "Bowling Ball", "Burger" });

    private static List<string> fridge = new List<string>( new string[] { "MILK", "CHEESE" } );
    private static List<string> cupboard = new List<string>( new string[] { "PASTA", "LENTILS" } );
    private static List<string> prep = new List<string>( new string[] { "WHISK", "CHOPPING BOARD" } );
    private static List<string> serve = new List<string>( new string[] { "SPOON", "PLATE" } );
    
    private static List<string> binA = new List<string>( new string[] { "GLASS", "FOOD WASTE" } );
    private static List<string> binB = new List<string>( new string[] { "INTERGALACTIC\nBLACK HOLE", "PLASTIC" } );
    
    private static List<string> WinnersList = new List<string>( new string[] {"WINNER!!","2nd","3rd","4th", "5th", "6th"});
    
    private static List<List<string>> GoodStations = new List<List<string>>{ fridge, cupboard, prep, serve };
    private static List<List<string>> BadStations = new List<List<string>>{ binA, binB };
    
    private static List<String> micInstructions = new List<string>(new string[] { " Waiters won't take the food out fast enough!\n Shout at them to work harder!\n\n (SHOUT INTO THE MIC)", " Your team are being useless!\n Shout some sense into them!\n\n (SHOUT INTO THE MIC)",
                                                                                  " Rats have been spotted in the kitchen!\n Scream to scare them away!\n\n (SHOUT INTO THE MIC)", " Whoops! You just set your chopping board on fire!\n Try to blow it out!\n\n (BLOW INTO THE MIC)", 
                                                                                  " The person you have a crush on just walked in!\n Shout at them to confess your love!\n\n (SHOUT INTO THE MIC)", " Whoopsie! You've just set yourself on fire!\n Better try and blow the fire out!\n\n (BLOW INTO THE MIC)",
                                                                                  " The human race is on the brink of extinction and no one seems to care!\n Scream in despair!\n\n (SHOUT, SCREAM OR CRY INTO THE MIC"});

    private static List<String> shakeInstructions = new List<string>(new string[] { " Chef underseasoned the dish!\n Shake to salt the food!\n\n(SHAKE YOUR PHONE)", " The Queen has decided to dine here for some reason!\n Better give her a wave!\n\n(SHAKE YOUR PHONE)",
                                                                                    " Food runner dropped the dish!\n Shake some sense into the boy!\n\n (SHAKE YOUR PHONE)", " It's Virgin PornStar Martini time!\n Better shake that cocktail and thank the deveopers!\n\n (SHAKE YOUR PHONE)",
                                                                                    " Pan set on fire!\n Shake to put it out!\n\n (SHAKE YOUR PHONE)", " We are ten years away from irreversible climate damage!\n This isn't really an instruction, just thought you needed to know, but shake the phone anyway\n\n (SHAKE YOUR PHONE)",
                                                                                    " Your arch nemisis just walked in!\n Shake your fist at them angrily!\n\n (SHAKE YOUR PHONE)"});

    private static List<String> cameraInstructionText = new List<string>(new string[] { "FIND THE TOMATO!", "FIND THE ORANGE!", "FIND THE BANANAS!", "FIND THE APPLE!", "FIND THE EU FLAG TO REVOKE ARTICLE 50!" });


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
    public List<Player> Players;
    public int NumberOfButtons { get; set; }
    public int PlayerCount { get; set; }

    //Booleans
    [SyncVar] public bool isRoundPaused = false;
    [SyncVar] public bool isLastActionOfRound = false;

    public bool IsLastActionOfRound
    {
        get
        {
            return isLastActionOfRound;
        }
    }

    [SyncVar] public bool SetupFinished = false;

    //Phone interaction probability = 2/x
    [SyncVar] public int piProb = 12;

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
            }
            
            player.GenerateGoodStation(GoodStations);
            player.GenerateBadStation(BadStations);

            string instruction = ActiveInstructions[player.PlayerId];
            player.SetInstruction(instruction);
        }          
    }

    /*
     * Reset the active instructions and buttons.
     */
    [Server]
    public void ResetIC()
    {
        SelectButtonActions();  //Create synced list of executables, one for each button in the game
        SetFirstInstructions(); //Select one instruction per player from Action Button List
    }

    public string getPositionWord(int i)
    {
        if (i >= PlayerCount) return "error";
        return WinnersList[i];
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
        
//        Debug.Log("PlayerCount :: " + PlayerCount + ", NoB :: "  //                  + NumberOfButtons);
        
        for (int i = 0; i < PlayerCount*NumberOfButtons; i++)
        {
            duplicate = true;
            while (duplicate)
            {
                verbNo = UnityEngine.Random.Range(0, verbList.Count-1);
                nounNo = UnityEngine.Random.Range(0, nounList.Count-1); 
                text = verbList[verbNo] + " " + nounList[nounNo];

                if (ActiveButtonActions.Contains(text)) continue;
                ActiveButtonActions.Add(text);
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

            GameController.CheckAction(i);
            Players[i].PlayerScore++;

            PickNewInstruction(i, action);
            if (!isLastActionOfRound)
            {
                RpcUpdateInstruction(ActiveInstructions[i], i);
                RpcStartInstTimer(i);

                int rand = UnityEngine.Random.Range(0, 5);
               /* if (rand == 0)
                {
                    rand = UnityEngine.Random.Range(0, micInstructions.Count);
                    RpcSetMicPanel(i, micInstructions[rand]);
                }*/
                if (rand == 1)
                {
                    rand = UnityEngine.Random.Range(0, shakeInstructions.Count);
                    RpcSetShakePanel(i, shakeInstructions[rand]);
                }
                /*
                else if (rand == 2)
                {
                    rand = UnityEngine.Random.Range(0, 5);
                    RpcSetCameraPanel(i, rand, cameraInstructionText[rand]);
                }*/
            }
        }

        if (!match)
        {
            GameController.fireCount++;
            GameController.customerSatisfaction -= 3;
        }
    }
    
    /*
     * Checks if failed action is a current active instruction, and if so pick new instruction and orders NFC bin.
     */
    public void FailAction(string action, string bin)
    {
        //When an action button is pressed by a player-client, check if action matches an active instruction.
        for (int i = 0; i < ActiveInstructions.Count; i++)
        {
            //If match, increment score.
            if (action == ActiveInstructions[i])
            {
                GameController.RpcResetScoreSteak(i);
                GameController.IncreaseFireCount();
                
                PickNewInstruction(i, action);
                RpcUpdateInstruction(ActiveInstructions[i], i);
                RpcSetNfcPanel(i, "You missed an order! " + System.Environment.NewLine
                                                           + "Run to the "+ bin +"!" + System.Environment.NewLine + System.Environment.NewLine
                                                           + "(TAP ON "+ bin +" NFC)");
            }
        }
    }

    /*
     * New Instruction selected from set of ActiveButtonActions, with no duplicate ensured.
     */
    public void PickNewInstruction(int index, string action)
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
    public void RpcSetCameraPanel(int playerID, int colour, string text)
    {
        Players[playerID].SetCameraPanel(colour, text);
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
}



