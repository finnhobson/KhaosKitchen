using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public GameController gameController;

    public Button eggButton;
    public Button fryButton;
    public Button serveButton;

    public Text scoreText;

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Update()
    {
        scoreText.text = gameController.score.ToString();
    }

    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }

    [Command]
    public void CmdAction(int x)
    {
        if (x == 1) gameController.CheckEgg();
        if (x == 2) gameController.CheckFry();
        if (x == 3) gameController.CheckServe();
    }

    public void OnClickEgg()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Egg Clicked");
            CmdAction(1);
        }
    }

    public void OnClickFry()
    {
        if (isLocalPlayer)
        {
            CmdAction(2);
        }
        
    }

    public void OnClickServe()
    {
        if (isLocalPlayer)
        {
            CmdAction(3);
        }
    }
}
