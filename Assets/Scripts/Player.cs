using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public GameController gameController;
    public InstructionController InstructionController;

    public Button button1, button2, button3, button4;
    public Button[] AllButtons;

    public AudioClip correctAction;
    public AudioClip incorrectAction;
    public AudioSource source;

    public Text scoreText, instructionText, timerText, gpsText, roundScoreText, topChefText, countdownText, roundNumberText;

    public GameObject nfcPanel, micPanel, shakePanel, gameOverPanel, roundCompletePanel, roundStartPanel;
    public Button nfcButton, micButton, shakeButton;
    public Text nfcText, micText, shakeText;

    public int PlayerId { get; set; }
    public string PlayerUserName { get; set; }
    public int PlayerScore { get; set; }

    private string nfcValue = "";

    private int instTime = 30;

    private HashSet<String> validNfc = new HashSet<String>{"Grab Meat","Grab Pasta"};

    public MicListener micListener;

    public GameObject instBar;

    public float instTimeLeft, instStartTime;

    private int scoreStreak = 0;

    public bool isGamePaused = false;

    //Sets score streak where event will occur
    private const int scoreStreakMax = 5;

    private void Start()
    {
        //Link Player GameObject to GameController.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        InstructionController = GameObject.FindGameObjectWithTag("InstructionController").GetComponent<InstructionController>();
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
            if (instTimeLeft < 0 && isLocalPlayer)
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

            nfcValue = NfcCheck();
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

            gpsText.text = nfcValue;
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

    public void SetPlayerId(int assignedId)
    {
        PlayerId = assignedId;
    }

    public int getPlayerId()
    {
        return PlayerId;
    }

    private string NfcCheck()
    {
        string value = NFCListener.GetValue();
        if (value == "BFTT4mEzgA==")
        {
            NFCListener.SetValue("");
            return "Grab Meat";
        } else if (value == "BFXT4mEzgA==")
        {
            NFCListener.SetValue("");
            return "Grab Meat";
        }
        else
        {
            return value;
        }
    }

    /**
     * What is the point of this function when the game controller has already been assigned by tag?
     **/
    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }

    public void SetInstructionController(InstructionController instructionController)
    {
        InstructionController = instructionController;
    }

    //Assign instructions to each player (NOTE: Currently only works for up to 2 players).
    public void SetActionButtons(string instruction, int i)
    {
        if (!isLocalPlayer) return;
        CmdUpdateIHWithButtonData(i, instruction, PlayerId);
        
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

    public void SetInstruction(String d)
    {
        if (!isLocalPlayer) return;
        instructionText.text = d;
        if (isGamePaused) return;
        CmdUpdateIHWithInstructionData(d);
    }

    public string GetInstruction()
    {
        return instructionText.text;
    }

    [Command]
    public void CmdAction(string action)
    {
        InstructionController.CheckAction(action);
    }

    [Command]
    public void CmdFail(string action)
    {
        InstructionController.FailAction(action);
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
            CheckInstruction(button1.GetComponentInChildren<Text>().text, 0);
//            CmdAction(button1.GetComponentInChildren<Text>().text);
        }
    }

    public void OnClickButton2()
    {
        if (isLocalPlayer)
        {
            CheckInstruction(button2.GetComponentInChildren<Text>().text, 1);

//            CmdAction(button2.GetComponentInChildren<Text>().text);
        }
    }

    public void OnClickButton3()
    {
        if (isLocalPlayer)
        {
            CheckInstruction(button3.GetComponentInChildren<Text>().text, 2);

//            CmdAction(button3.GetComponentInChildren<Text>().text);
        }
    }

    public void OnClickButton4()
    {
        if (isLocalPlayer)
        {
            CheckInstruction(button4.GetComponentInChildren<Text>().text, 3);

//            CmdAction(button4.GetComponentInChildren<Text>().text);
        }
    }
    
    public void NfcClick(string nfcString)
    {
        if (isLocalPlayer)
        {
            CmdAction(nfcString);
        }
    }
    
    public void MicClick(string micString)
    {
        if (isLocalPlayer)
        {
            CmdAction(micString);
        }
    }
    
    public void ShakeClick(string shakeString)
    {
        if (isLocalPlayer)
        {
            CmdAction(shakeString);
        }
    }

    public void StartInstTimer()
    {
        instStartTime = instTime;
        instTimeLeft = instStartTime; 
    }

    private void UpdateInstTimeLeft()
    {
        if (isGamePaused)
        {
            //Reset timer
            instTimeLeft = instStartTime;
            instBar.GetComponent<RectTransform>().localScale = new Vector3(instTimeLeft / instStartTime, 1, 1);
        }
        else if(nfcPanel.activeSelf||micPanel.activeSelf||shakePanel.activeSelf||isGamePaused)
        {
            //panel active so no timer 
        }
        else
        {
            instTimeLeft -= Time.deltaTime;
            instBar.GetComponent<RectTransform>().localScale = new Vector3(instTimeLeft / instStartTime, 1, 1);
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

    public void PausePlayer()
    {
        isGamePaused = true;
    }
    
    public void UnpausePlayer()
    {
        isGamePaused = false;
    }
    
    /*
     * Updates instruction button number, playerID.
     */
    [Command]
    public void CmdUpdateIHWithButtonData(int buttonNumber, string action, int playerID)
    {
        InstructionController.PlayerUpdateButton(buttonNumber, action, playerID);
    }
    
    /*
    * Updates instruction button number, playerID.
    */
    [Command]
    public void CmdUpdateIHWithInstructionData(string action)
    {
        InstructionController.PlayerUpdateInstruction(action, PlayerId);
    }

    private void CheckInstruction(string action, int buttonNumber)
    {
        CmdAction(action);
        if (InstructionController.ActiveInstructions.Contains(action))
        {
            ThisButtonWasPressed(buttonNumber);
        } 
        
        else
        {
            ThisButtonWasNotPressed(buttonNumber);
        }
    }

    private void ThisButtonWasPressed(int buttonNumber) 
    {
        //Activate feedback on this button
        CmdPrint(buttonNumber);
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.green;
        //Insert Sound here
        StartCoroutine(RestartNewRoundAfterXSeconds(0.5f, buttonNumber));
    }

    private void ThisButtonWasNotPressed(int buttonNumber)
    {
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.red;
        StartCoroutine(RestartNewRoundAfterXSeconds(0.5f, buttonNumber));
    }

    private IEnumerator RestartNewRoundAfterXSeconds(float x, int buttonNumber)
    {
        yield return new WaitForSecondsRealtime(x);
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.white;
        
    }

    [Command]
    private void CmdPrint(int buttonNumber)
    {
        gameController.PrintOut(buttonNumber);
    }
}



