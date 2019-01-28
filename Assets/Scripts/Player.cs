using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public GameController gameController;

    public Button button1, button2, button3, button4;

    public Text scoreText, instructionText;

    public int playerId;
    
    
    private string nfcValue = "";
    
    private HashSet<String> validNfc = new HashSet<String>{"Grab Meat","Grab Pasta"};


    //    public Button[] buttons = new Button[4];
    public void setPlayerId(int assignedId)
    {
        playerId = assignedId;
    }

    public int getPlayerId()
    {
        return playerId;
    }

    private void Start()
    {
        //Link Player GameObject to GameController.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //        buttons[0] = button1;
        //        buttons[1] = button2;
        //        buttons[2] = button3;
        //        buttons[3] = button4;


    }


    private void Update()
    {
        //Display score.
        //scoreText.text = gameController.score.ToString();
        if(MicListener.MicLoudness > 0.15f){
            scoreText.text = "Noise";
        }

        nfcValue = nfcCheck();
        scoreText.text = nfcValue;
        if (validNfc.Contains(nfcValue))
        {
            nfcClick(nfcValue);
        }

        if (ShakeListener.shaking)
        {
            scoreText.text = "shaking";
        }
    }
    
    private string nfcCheck()
    {
        string value = NFCListener.GetValue();
        if (value == "BPO/2m8/gA==")
        {
            
            NFCListener.SetValue("");
            return "Grab Meat";
        } else if (value == "BPjA2m8/gA==")
        {
            NFCListener.SetValue("");
            return "Grab Pasta";
        }
        else
        {
            return value;
        }
    }


    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }

    //Assign instructions to each player (NOTE: Currently only works for up to 2 players).
    public void SetActionButtons(string instruction, int i)
    {
        if (isLocalPlayer)
        {
            switch (i)
            {
                case 0:
                    button1.GetComponentInChildren<Text>().text = instruction;
                    break;
                case 1:
                    button2.GetComponentInChildren<Text>().text = instruction;
                    break;
                case 2:
                    button3.GetComponentInChildren<Text>().text = instruction;
                    break;
                case 3:
                    button4.GetComponentInChildren<Text>().text = instruction;
                    break;
                default:
                    Console.WriteLine("ERROOOR");
                    break;
            }
        }
//        {
//            if (ID == 1)
//            {
//                button1.GetComponentInChildren<Text>().text = "Chop Carrot";
//                button2.GetComponentInChildren<Text>().text = "Wash Dishes";
//                button3.GetComponentInChildren<Text>().text = "Blend Smoothie";
//                button4.GetComponentInChildren<Text>().text = "Fry Burger";
//            }
//            if (ID == 2)
//            {
//                button1.GetComponentInChildren<Text>().text = "Heat Oven";
//                button2.GetComponentInChildren<Text>().text = "Chop Onion";
//                button3.GetComponentInChildren<Text>().text = "Scramble Eggs";
//                button4.GetComponentInChildren<Text>().text = "Slice Cheese";
//            }
//        }
    }

    public void SetInstruction(String d)
    {
        instructionText.text = d;
    }

    public string GetInstruction()
    {
        return instructionText.text;
    }


    [Command]
    public void CmdAction(string action)
    {
        gameController.CheckAction(action, getPlayerId());
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
    
    public void nfcClick(String nfcString)
    {
        if (isLocalPlayer)
        {
            CmdAction(nfcString);
        }
    }

}
