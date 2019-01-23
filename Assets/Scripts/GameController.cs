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

    [SyncVar] public int score = 0;
    SyncListString activeInstructions = new SyncListString();

    private List<String> burgerRecipe = new List<string>();
    private List<String> burgerRecipeRandom = new List<string>();
    
    private List<String> genericRecipe = new List<string>();
    private List<String> genericRecipeRandom = new List<string>();

    // private List<String> otherRecipe; 

    private Stack<String> random = new Stack<string>();
   // private Stack<String> ordered = new Stack<string>();
    private int numberOfButtons = 4;
    private int playerCount = 2;

    public List<Player> playerList = new List<Player>();

//    private void ChooseRecipe()
//    {
//        //This is where we choose recipe
//        burgerRecipe.Add("Grab Meat");
//        burgerRecipe.Add("Grab Salad");
//        burgerRecipe.Add("Grab Buns");
//        burgerRecipe.Add("Grab Cheese");
//        burgerRecipe.Add("Grind Meat");
//        burgerRecipe.Add("Chop Salad");
//        burgerRecipe.Add("Cut Bun");
//        burgerRecipe.Add("Wash Salad");
//    }

    //return random Stack as randomly ordered stack of instructions
    private void setRandom(List<String> list)
    {
        System.Random rnd = new System.Random();
        for (int i = 0; i < list.Count; i++)
        {
            int k = rnd.Next(0, i);
            String value = list[k];
            list[k] = list[i];
            list[i] = value;
        }
    }

    //Return
    private Stack<String> setOrdered(List<String> list)
    {
        return new Stack<String>(list);
    }

    [ClientRpc]
    void RpcResetRound()
    {
        burgerRecipe.Clear();
        burgerRecipeRandom.Clear();
        
        
        burgerRecipe.Add("1");
        burgerRecipe.Add("2");
        burgerRecipe.Add("3");
        burgerRecipe.Add("4");
        burgerRecipe.Add("5");
        burgerRecipe.Add("6");
        burgerRecipe.Add("7");
        burgerRecipe.Add("8");

        activeInstructions.Add(burgerRecipe[0]);
        activeInstructions.Add(burgerRecipe[1]);

        foreach (var item in burgerRecipe)
        {
            burgerRecipeRandom.Add(item);
        }
        
        setRandom(burgerRecipeRandom);
        
        for(int k = 0;k<playerCount;k++)
        {
            burgerRecipe.Add("Complete!");
        }
        
        var players = FindObjectsOfType<Player>();
        int j = 0;
        int b = 0;
        foreach (Player p in players)
        {
            Debug.Log("in");
            p.SetInstruction(burgerRecipe[p.getPlayerId()]);
            for (int i = 0; i < numberOfButtons; i++)
            {
                p.SetActionButtons(burgerRecipeRandom[b], i);
                b++;
            }

            j++;
        } 
        
        for (int k = 0; k < burgerRecipe.Count; k++)
        {
            Debug.Log(k+" "+burgerRecipe[k]);
        }
        

    }
    

    void Start()
    {
        //Show server display only on the server.
        if (isServer) GetComponentInChildren<Canvas>().enabled = true;
        
        burgerRecipe.Add("Grab Meat");
        burgerRecipe.Add("Grab Salad");
        burgerRecipe.Add("Grab Buns");
        burgerRecipe.Add("Grab Cheese");
        burgerRecipe.Add("Grind Meat");
        burgerRecipe.Add("Chop Salad");
        burgerRecipe.Add("Cut Bun");
        burgerRecipe.Add("Wash Salad");
        
        activeInstructions.Add(burgerRecipe[0]);
        activeInstructions.Add(burgerRecipe[1]);

        foreach (var item in burgerRecipe)
        {
            burgerRecipeRandom.Add(item);
        }
        
        setRandom(burgerRecipeRandom);
        
        for(int k = 0;k<playerCount;k++)
        {
            burgerRecipe.Add("Complete!");
        }
        
        //Assign actions to each player.
        var players = FindObjectsOfType<Player>();
        int j = 0;
        int b = 0;
        foreach (Player p in players)
        {
            playerList.Add(p);
            p.SetGameController(this);
            p.setPlayerId(j);
            p.SetInstruction(burgerRecipe[j]);
            for (int i = 0; i < numberOfButtons; i++)
            {
                p.SetActionButtons(burgerRecipeRandom[b], i);
                b++;
            }
            j++;
        }
        //   playerList[0].SetInstruction(ordered.Pop());
//       playerList[1].SetInstruction(ordered.Pop());
        //  playerList[1].instructionText.text = ordered.Pop();
        
//        
//        for (int k = 0; k < burgerRecipe.Count; k++)
//        {
//            Debug.Log(k+" "+burgerRecipe[k]);
//            
//        }
//
//
//        Debug.Log(activeInstructions[0]);
//        Debug.Log(activeInstructions[1]);

    }

    private void Update()
    {
//        if (isRoundComplete())
//        {
//            //Call reset function
//            Debug.Log("ROUND COMPLETE MOTHERFUCKERS!!!!!!!!");
//        }

        //Show score and active instructions on server display.
        scoreText.text = score.ToString();
        

    }

    [ClientRpc]
    public void RpcUpdateInstructions(String newInstruction, int playerID)
    {
        playerList[playerID].SetInstruction(newInstruction);
    }
    
    public void updateInstruction(string instruction, int instructionNumber)
    {
        activeInstructions[instructionNumber] = instruction;
    }


    [Server]
    public void CheckAction(string action, int playerId)
    {
        //When an action button is pressed by a player-client, check if action matches an active instruction.
        for (int i = 0; i < activeInstructions.Count; i++)
        {
            //If match, increment score.
            if (action == activeInstructions[i])
            {
                score++;
                activeInstructions[i] = burgerRecipe[playerCount+score-1];          
                RpcUpdateInstructions(burgerRecipe[playerCount+score-1], i);
//                for (int j = 0; j < burgerRecipe.Count; j++)
//                {
//                    Debug.Log(j+" "+burgerRecipe[j]);
//                }
            }
        }

        if (score == playerCount*numberOfButtons)
        {
            //Run Reset
            Debug.Log("ROUND COMPLETE");
            RpcResetRound();
            score = 0;
        }
    }
}