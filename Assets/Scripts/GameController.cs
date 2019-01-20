using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameController : NetworkBehaviour {
    
    [SyncVar]
    public int score = 0;

    SyncListString activeInstructions = new SyncListString();

    public int IDcount = 1;

    void Start()
    {
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
