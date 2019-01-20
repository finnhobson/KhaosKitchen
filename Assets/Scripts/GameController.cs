using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameController : NetworkBehaviour {

    public Text scoreText, instruction1, instruction2, instruction3, instruction4;
    
    [SyncVar]
    public int score = 0;

    SyncListString activeInstructions = new SyncListString();

    public int IDcount = 1;


    void Start()
    {
        //Show server display only on the server.
        if (isServer) GetComponentInChildren<Canvas>().enabled = true;

        //Assign actions to each player.
        var players = FindObjectsOfType<Player>();
        foreach (Player p in players) {
            p.SetGameController(this);
            p.SetActionButtons(IDcount);
            IDcount++;
        }

        //Add instructions to list of active instructions.
        activeInstructions.Add("Chop Carrot");
        activeInstructions.Add("Fry Burger");
        activeInstructions.Add("Scramble Eggs");
        activeInstructions.Add("Heat Oven");
    }


    private void Update()
    {
        //Show score and active instructions on server display.
        scoreText.text = score.ToString();
        instruction1.text = activeInstructions[0];
        instruction2.text = activeInstructions[1];
        instruction3.text = activeInstructions[2];
        instruction4.text = activeInstructions[3];
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
                activeInstructions[i] = "COMPLETE!";
            }
        }
    }
    
}
