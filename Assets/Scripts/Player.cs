using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public GameController gameController;

    public Button button1, button2, button3, button4;

    public Text scoreText;


    private void Start()
    {
        //Link Player GameObject to GameController.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }


    private void Update()
    {
        //Display score.
        scoreText.text = gameController.score.ToString();
    }


    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }

    //Assign instructions to each player (NOTE: Currently only works for up to 2 players).
    public void SetActionButtons(int ID)
    {
        if (isLocalPlayer)
        {
            if (ID == 1)
            {
                button1.GetComponentInChildren<Text>().text = "Chop Carrot";
                button2.GetComponentInChildren<Text>().text = "Wash Dishes";
                button3.GetComponentInChildren<Text>().text = "Blend Smoothie";
                button4.GetComponentInChildren<Text>().text = "Fry Burger";
            }
            if (ID == 2)
            {
                button1.GetComponentInChildren<Text>().text = "Heat Oven";
                button2.GetComponentInChildren<Text>().text = "Chop Onion";
                button3.GetComponentInChildren<Text>().text = "Scramble Eggs";
                button4.GetComponentInChildren<Text>().text = "Slice Cheese";
            }
        }
    }


    [Command]
    public void CmdAction(string action)
    {
        gameController.CheckAction(action);
    }


    public void OnClickButton1()
    {
        if (isLocalPlayer)
        {
            CmdAction(button1.GetComponentInChildren<Text>().text);
        }
    }


    public void OnClickButton2()
    {
        if (isLocalPlayer)
        {
            CmdAction(button2.GetComponentInChildren<Text>().text);
        }
    }


    public void OnClickButton3()
    {
        if (isLocalPlayer)
        {
            CmdAction(button3.GetComponentInChildren<Text>().text);
        }
    }


    public void OnClickButton4()
    {
        if (isLocalPlayer)
        {
            CmdAction(button4.GetComponentInChildren<Text>().text);
        }
    }

}
