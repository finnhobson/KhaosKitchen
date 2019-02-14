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

    public Text scoreText, instructionText, timerText, gpsText;

    public GameObject nfcPanel, micPanel, shakePanel, gameOverPanel;
    public Button nfcButton, micButton, shakeButton;
    public Text nfcText, micText, shakeText;

    public int playerId;
    
    private string nfcValue = "";

    private HashSet<String> validNfc = new HashSet<String>{"Grab Meat","Grab Pasta"};

    public MicListener micListener;

    public GameObject instBar;

    public float instTimeLeft, instStartTime;

    private int scoreStreak = 0;

    //Sets score streak where event will occur
    private int scoreStreakMax = 5;

    private void Start()
    {
        //Link Player GameObject to GameController.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Screen.orientation = ScreenOrientation.Portrait;
        StartInstTimer();
    }


    private void Update()
    {
        //Display score.
        scoreText.text = gameController.score.ToString();
        if (gameController.roundTimeLeft > 0)
        {
            UpdateInstTimeLeft();
            if (instTimeLeft < 0)
            {
                CmdFail(instructionText.text);
                StartInstTimer();
            }
            else
            {
                SetTimerText(instTimeLeft.ToString("F2"));
            }

            if (MicListener.MicLoudness > 0.2f)
            {
                //shakeClick(Instruction text to be completed by shouting, matching that in activeInstructions);
                if (micPanel.activeSelf)
                {
                    micPanel.SetActive(false);
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            }

            nfcValue = nfcCheck();
            //scoreText.text = nfcValue;
            if (validNfc.Contains(nfcValue))
            {
                //nfcClick(nfcValue);
                if (nfcPanel.activeSelf)
                {
                    nfcPanel.SetActive(false);
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            }

            if (ShakeListener.shaking)
            {
                //shakeClick(Instruction text to be completed by shaking, matching that in activeInstructions);
                if (shakePanel.activeSelf)
                {
                    shakePanel.SetActive(false);
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            }

            gpsText.text = "Lat: " + GpsListener.latString + "\n Long: " + GpsListener.longString;
        }
        else{
            SetTimerText("0");
        }
    }


    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        SetTimerText("0");
    }

    public void setPlayerId(int assignedId)
    {
        playerId = assignedId;
    }

    public int getPlayerId()
    {
        return playerId;
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
            return "Grab Meat";
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
        gameController.CheckAction(action);
    }

    [Command]
    public void CmdFail(string action)
    {
        gameController.FailAction(action);
        ResetScoreStreak();
    }

    [Command]
    public void CmdIncreaseScore()
    {
        gameController.IncreaseScore();
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
    
    public void micClick(String micString)
    {
        if (isLocalPlayer)
        {
            CmdAction(micString);
        }
    }
    
    public void shakeClick(String shakeString)
    {
        if (isLocalPlayer)
        {
            CmdAction(shakeString);
        }
    }

    public void StartInstTimer()
    {
        instStartTime = 20;
        instTimeLeft = instStartTime; 
    }

    private void UpdateInstTimeLeft() 
    {
        if(nfcPanel.activeSelf||micPanel.activeSelf||shakePanel.activeSelf){
            //panel active so no timer 
        }else{
            instTimeLeft -= Time.deltaTime;
            instBar.transform.localScale = new Vector3(instTimeLeft / instStartTime, 1);
        }
    }

    private void SetTimerText(string text)
    {
        timerText.text = text;
    }

    public void SetNfcPanel(string text)
    {
        nfcPanel.SetActive(true);
        nfcText.text = text;

    }

    public void SetShakePanel(string text)
    {
        shakePanel.SetActive(true);
        shakeText.text = text;
    }

    public void SetMicPanel(string text)
    {
        micPanel.SetActive(true);
        micText.text = text;

    }

    public void OnClickNfcButton()
    {
        if (isLocalPlayer)
        {
            nfcPanel.SetActive(false);
        }
    }

    public void OnClickMicButton()
    {
        if (isLocalPlayer)
        {
            micPanel.SetActive(false);
        }
    }

    public void IncreaseScoreStreak(){
        scoreStreak++;
    }

    public void ResetScoreStreak(){
        scoreStreak = 0;
    }

    public void OnClickShakeButton()
    {
        if (isLocalPlayer)
        {
            shakePanel.SetActive(false);
        }
    }

    public void ScoreStreakCheck()
    {
        if(scoreStreak>=scoreStreakMax){
            ResetScoreStreak();
            SetNfcPanel(" Great Work!\n Dish is ready to serve!\n\n (TAP ON SERVE NFC)");
        }
    }
}



