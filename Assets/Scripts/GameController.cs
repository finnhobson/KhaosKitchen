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
        if (isServer) GetComponentInChildren<Canvas>().enabled = true;

        var players = FindObjectsOfType<Player>();
        foreach (Player p in players) {
            p.SetGameController(this);
            p.SetActionButtons(IDcount);
            IDcount++;
        }
        activeInstructions.Add("Chop Carrot");
        activeInstructions.Add("Fry Burger");
        activeInstructions.Add("Scramble Eggs");
        activeInstructions.Add("Heat Oven");
    }

    private void Update()
    {
        scoreText.text = score.ToString();
        instruction1.text = activeInstructions[0];
        instruction2.text = activeInstructions[1];
        instruction3.text = activeInstructions[2];
        instruction4.text = activeInstructions[3];


    }

    [Server]
    public void CheckAction(string action)
    {
        foreach (string instruction in activeInstructions)
        {
            if (action == instruction)
            {
                score++;
            }
        }
    }
    
}
