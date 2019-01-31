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

    public int playerCount = 3;

    public List<Player> playerList = new List<Player>();

    private static List<String> instructions = new List<string>(new string[] { "Grab Meat", "Grab Salad", "Grab Buns", "Grab Cheese", "Grind Meat", "Chop Salad", "Cut Bun", "Wash Salad", "Mash Potato",
        "Grill Meat", "Flip Pancake", "Roast Chicken", "Grab Pasta", "Grab Salad", "Grab Sauce", "Grab Cheese", "Boil Pasta", "Chop Salad", "Mix-in Sauce", "Serve Pasta", "Grate Parmesan", "Grind Pepper",
        "Drain Pasta", "Cook Beef" });

    


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
    }


    private void Update()
    {
        //Show score and active instructions on server display.
        scoreText.text = score.ToString();
    }


    private void SelectActions()
    {
        activeActions.Clear();
        int randomIndex;
        bool duplicate;
        for (int i = 0; i < playerCount*4; i++)
        {
            duplicate = true;
            while (duplicate)
            {
                randomIndex = UnityEngine.Random.Range(0, instructions.Count);
                if (activeActions.Contains(instructions[randomIndex]) == false)
                {
                    activeActions.Add(instructions[randomIndex]);
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
                score++;
                PickNewInstruction(i);      
                RpcUpdateInstruction(activeInstructions[i], i);
            }
        }
    }

    [Server]
    public void IncreaseScore()
    {
        score++;
    }
}