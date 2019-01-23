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
    
    private static List<String> burgerRecipe = new List<string>(new string[] { "Grab Meat", "Grab Salad", "Grab Buns", "Grab Cheese", "Grind Meat", "Chop Salad", "Cut Bun", "Wash Salad" });
    private static List<String> pastaRecipe = new List<string>(new string[] { "Grab Pasta", "Grab Salad", "Grab Sauce", "Grab Cheese", "Boil Pasta", "Chop Salad", "Mix-in Sauce", "Serve" });

    
    private List<String> burgerRecipeRandom = new List<string>();

    private HashSet<List<String>> setOfRecipes = new HashSet<List<String>>()
    {
        burgerRecipe, pastaRecipe
    };
    
    private Dictionary<String, List<String>> listOfRecipes = new Dictionary<string, List<string>>()
        {
            {"BurgerRecipe", burgerRecipe},
            {"PastaRecipe", pastaRecipe}
        };
    
    private List<String> genericRecipe = new List<string>();
    private List<String> genericRecipeRandom = new List<string>();

    // private List<String> otherRecipe; 

    private Stack<String> random = new Stack<string>();
   // private Stack<String> ordered = new Stack<string>();
    private int numberOfButtons = 4;
    private int playerCount = 2;

    public List<Player> playerList = new List<Player>();

    private void ChooseRecipe()
    {
        Debug.Log("IN CHOOSE RECIPE");
        genericRecipe.Clear();
        genericRecipeRandom.Clear();
        activeInstructions.Clear();
        System.Random rnd = new System.Random();
        int k = rnd.Next(0, 1);
        Debug.Log(k);
        genericRecipe = setOfRecipes.ElementAt(k);
    }

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

    void ResetRound()
    {
        Debug.Log("ResetStart");

        ChooseRecipe();
        Debug.Log("Recipe Rechosen");
        

        activeInstructions.Add(genericRecipe[0]);
        activeInstructions.Add(genericRecipe[1]);

        foreach (var VARIABLE in genericRecipe)
        {
            genericRecipeRandom.Add(VARIABLE);
        }
        
        setRandom(genericRecipeRandom);
        
        for(int k = 0;k<playerCount;k++)
        {
            genericRecipe.Add("Complete!");
        }
        
        var players = FindObjectsOfType<Player>();
        int j = 0;
        int b = 0;
        foreach (Player p in players)
        {
            Debug.Log("in");
            RpcUpdateInstructions(genericRecipe[p.getPlayerId()], j);
            for (int i = 0; i < numberOfButtons; i++)
            {
                p.SetActionButtons(genericRecipeRandom[b], i);
                b++;
            }

            j++;
        } 
        
//        for (int k = 0; k < genericRecipe.Count; k++)
//        {
//            Debug.Log(k+" "+genericRecipe[k]);
//        }
        

    }

    void Start()
    {
        //Show server display only on the server.
        if (isServer) GetComponentInChildren<Canvas>().enabled = true;
        
        ChooseRecipe();

        activeInstructions.Add(genericRecipe[0]);
        activeInstructions.Add(genericRecipe[1]);

        foreach (var VARIABLE in genericRecipe)
        {
            genericRecipeRandom.Add(VARIABLE);
        }

        setRandom(genericRecipeRandom);

        for (int k = 0; k < playerCount; k++)
        {
            genericRecipe.Add("Complete!");
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
            p.SetInstruction(genericRecipe[j]);
            for (int i = 0; i < numberOfButtons; i++)
            {
                p.SetActionButtons(genericRecipeRandom[b], i);
                b++;
            }

            j++;
        }
    }

    private void Update()
    {
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
                activeInstructions[i] = genericRecipe[playerCount+score-1];          
                RpcUpdateInstructions(genericRecipe[playerCount+score-1], i);
            }
        }

        if (score == playerCount*numberOfButtons)
        {
            //Run Reset
            Debug.Log("ROUND COMPLETE");
            ResetRound();
            score = 0;
        }
    }
}