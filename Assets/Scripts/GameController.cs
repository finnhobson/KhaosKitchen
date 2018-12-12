using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameController : NetworkBehaviour {
    
    [SyncVar]
    public int score = 0;

    SyncListBool instruction = new SyncListBool();

    void Start()
    {
        var players = FindObjectsOfType<Player>();
        foreach (Player p in players) {
            p.SetGameController(this);
        }
        instruction.Add(false);
        instruction.Add(false);
        instruction.Add(false);
    }

    public void CheckEgg()
    {
        if (instruction[0] == false)
        {
            Debug.Log("Egg Completed");
            score++;
            instruction[0] = true;
        }
    }

    public void CheckFry()
    {
        if ((instruction[0] == true) && instruction[1]==false)  
        {
            score++;
            instruction[1] = true;
        }
    }

    public void CheckServe()
    {
        if (instruction[1] == true && instruction[2]==false) 
        {
            score++;
            instruction[2] = true;
        }
    }

    
}
