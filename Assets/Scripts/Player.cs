using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = System.Random;

/*
 * 1. a BAxnSotfgQ==
 *    b BPlnSotfgA==
 *    c BA1nSotfgQ==
 *
 * 2 a BORnSotfgA==
 *   b BNBnSotfgA==
 *   c BPhnSotfgA==
 *
 * 3 a BF5nSotfgQ==
 *   b BHFnSotfgQ==
 *   c BFVnSotfgA==
 *
 * 4 a BFZnSotfgA==
 *   b BGlnSotfgA==
 *   c BGpnSotfgA==
 *
 *
 * 
 */
public class Player : NetworkBehaviour {

    //Custom objects
    public GameController gameController;
    public InstructionController InstructionController;

    //Buttons
    public Button button1, button2, button3, button4;
    public Button[] AllButtons;
    public Button nfcButton, micButton, shakeButton;

    //Sound
    public AudioClip[] correctActions;
    public AudioClip[] incorrectActions;
    public AudioClip[] badNoises;
    private AudioSource source;
    private int switcher = 0;
    private int failSwitch = 0;
    private int numberOfFails;
    private int numberOfButtonSounds;
    public float VolumeOfSoundEffects { get; set; }
    private float Volume = 2f;

    //Unity GameObjects
    public Text scoreText, instructionText, timerText, gpsText, roundScoreText, topChefText, countdownText, roundNumberText, nameText, micVolumeText;
    public GameObject nfcPanel, micPanel, shakePanel, gameOverPanel, roundCompletePanel, roundStartPanel, groupMessagePanel;
    public Text nfcText, micText, shakeText;
    public GameObject nfcOkayButton, micOkayButton, shakeOkayButton;
    public GameObject fullScreenPanel;
    public Text fullScreenPanelText;
    public GameObject cameraController, cameraPanel;
    
    //Player
    [SyncVar] public string PlayerUserName;
    [SyncVar] public Color PlayerColour;
    [SyncVar (hook = "UpdateScore") ] public int PlayerScore;
    [SyncVar] public int PlayerId;
    public uint PlayerNetworkID;
    
    private List<Station> GoodStations = new List<Station>();
    private List<Station> BadStations = new List<Station>();
    

    [SyncVar (hook = "DisplayTopChef")] private string topChef;
    public string TopChef
    {
        get { return topChef; }
        set { topChef = value; }
    }
    public string topChefPush;

    //Extras
    [SyncVar] private string nfcValue = "";
    private string validNfc = "";
    public int playerCount;
    public int instTime;
    public bool easyPhoneInteractions;
//    private HashSet<String> validNfc = new HashSet<String>{"Food Waste","Recycling Bin","Window A","Window B"};
    public MicListener micListener;

    //Timer
    //private int instTime = 30;
    public GameObject instBar;
    public float instTimeLeft, instStartTime;

    //Score
    private int scoreStreak = 0;

    public int ScoreStreak
    {
        get
        {
            return scoreStreak;
        }
    }

    private const int scoreStreakMax = 3;

    //Booleans
    public bool isGamePaused;

    public bool IsGamePaused
    {
        get
        {
            return isGamePaused;
        }
    }

    private bool isBinA = true;
    private bool isWindowA = true;
    private bool isFail = false;
    private bool isServe = false;

    public bool IsServe
    {
        get
        {
            return isServe;
        }
    }

    private bool micActive = false;
    private bool timerStarted = false;
    [SyncVar] public bool isSetupComplete;	
    [SyncVar] public bool isGroupActive;
    [SyncVar] public bool isShaking;
    [SyncVar] public int activityNumber = 1;
    [SyncVar] public bool isNFCRace;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        numberOfFails = badNoises.Length;
        numberOfButtonSounds = correctActions.Length;
    }

    private void Start()
    {
        //Link Player GameObject to GameController.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        InstructionController = GameObject.FindGameObjectWithTag("InstructionController").GetComponent<InstructionController>();
        Screen.orientation = ScreenOrientation.Portrait;
        
        // ------------------------------------------------------------------------------
//        fullScreenPanel.SetActive(false);
//        roundCompletePanel.SetActive(false);
//        roundStartPanel.SetActive(false);
        // ------------------------------------------------------------------------------
        
        if (isLocalPlayer)
        {
            CmdSetNetworkID(PlayerNetworkID);
            CmdSetNameAndColour(PlayerUserName, PlayerColour);
            CmdSetPlayerReady();        
        }

        StartInstTimer();
        VolumeOfSoundEffects = Volume;
        nameText.text += PlayerUserName;
        micListener.enabled = false;

        int rand = UnityEngine.Random.Range(0, 1);
        if (rand == 0)
        {
            isBinA = false;
        }
        rand = UnityEngine.Random.Range(0, 1);
        if (rand == 0)
        {
            isWindowA = false;
        }
    }

    private void Update()
    {
        //Display score.
//        scoreText.text = gameController.score.ToString();
//        scoreText.text = NfcCheck();
        
//        groupMessagePanel.SetActive(isGroupActive);
        if (gameOverPanel.activeSelf) return;
        
        groupMessagePanel.SetActive(isGroupActive);

        if (isGroupActive)
        {
            if (isClient)
            {
                CheckGroupActivity();
            }
        }

        
        //if (micActive) micVolumeText.text = micListener.MicLoudness.ToString("F4");
//        topChefText.text = TopChef;
//        scoreText.text = PlayerScore.ToString();

        if (!timerStarted && gameController.isGameStarted)
        {
            StartInstTimer();
            timerStarted = true;
            if (isLocalPlayer) CmdUpdateChefPrefab();
        }

        if (gameController.isGameStarted && gameController.roundTimeLeft > 0)
        {
            UpdateInstTimeLeft();
            if (instTimeLeft < 0 && isLocalPlayer)
            {
                string tmp = GetBadNextNFC();
                validNfc = tmp;
                CmdFail(instructionText.text, tmp);
               // CmdFail(instructionText.text,(isBinA) ? "Food Waste" : "Recycling Bin");
                PlayFailSound();
                StartInstTimer();
                isFail = true;
            }
            else
            {
                SetTimerText(instTimeLeft.ToString("F1"));
            }

            if (micPanel.activeInHierarchy && !micActive)
            {
                micListener.enabled = true;
                micActive = true;
            }

            if (micPanel.activeInHierarchy && micListener.MicLoudness > 0.15f)
            {
                micPanel.SetActive(false);
                micActive = false;
                micListener.enabled = false;
                CmdIncreaseScore();
                StartInstTimer();
            }

            nfcValue = NfcCheck();
            if (validNfc.Equals(nfcValue) && nfcPanel.activeSelf)
            {
                nfcPanel.SetActive(false);
                CmdIncreaseScore();
                StartInstTimer();                       
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

//            instructionText.text = nfcValue;
        }
        else
        {
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

    public int GetPlayerId()
    {
        return PlayerId;
    }

    private string NfcCheck()
    {
        string value = NFCListener.GetValue();
        
        //Group 1
        if (value == "BAxnSotfgQ==") return GoodStations[0].GetStationItem(0);
        if (value == "BPlnSotfgA==")return GoodStations[0].GetStationItem(1);
        if (value == "BA1nSotfgQ==")return GoodStations[0].GetStationItem(2);

        //nfc 2
        if (value == "BORnSotfgA==")return GoodStations[1].GetStationItem(0);
        if (value == "BNBnSotfgA==")return GoodStations[1].GetStationItem(1);
        if (value == "BPhnSotfgA==")return GoodStations[1].GetStationItem(2);
    //nfc 3
        if (value == "BF5nSotfgQ==")return BadStations[0].GetStationItem(0);
        if (value == "BHFnSotfgQ==")return BadStations[0].GetStationItem(1);
        if (value == "BFVnSotfgA==")return BadStations[0].GetStationItem(2);
    //nfc 4
        if (value == "BFZnSotfgA==")return BadStations[1].GetStationItem(0);
        if (value == "BGlnSotfgA==")return BadStations[1].GetStationItem(1);
        if (value == "BGpnSotfgA==")return BadStations[1].GetStationItem(2);
        return value;




//        if (value == "BFTT4mEzgA==")
//        {
//            NFCListener.SetValue("");
//            return "Food Waste";
//        } else if (value == "BFXT4mEzgA==")
//        {
//            NFCListener.SetValue("");
//            return "Recycling Bin";
//        }
//        else if (value == "BDbT4mEzgA==")
//        {
//            NFCListener.SetValue("");
//            return "Window A";
//        } 
//        else if (value == "BBrT4mEzgA==")
//        {
//            NFCListener.SetValue("");
//            return "Window B";
//        }
//        else
//        {
//            return value;
//        }
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
    public void CmdFail(string action, string bin)
    {
        InstructionController.FailAction(action, bin);
        ResetScoreStreak();
    }

    [Command]
    public void CmdIncreaseScore()
    {
        gameController.IncreaseScore();
        PlayerScore++;
    }

    [Command]
    public void CmdIncreasePlayerScore()
    {
        PlayerScore++;
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
        instStartTime = gameController.CalculateInstructionTime();
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
        else if (nfcPanel.activeSelf || micPanel.activeSelf || shakePanel.activeSelf || isGamePaused)
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

    public void IncreaseScoreStreak()
    {
        scoreStreak++;
    }

    public void ResetScoreStreak()
    {
        scoreStreak = 0;
    }

    public void OnClickShakeButton()
    {
        if (isLocalPlayer)
        {
            shakePanel.SetActive(false);
        }
    }

    public void OnClickCameraButton()
    {
        if (cameraPanel.activeInHierarchy)
        {
            cameraPanel.SetActive(false);
            cameraController.SetActive(false);
        }
        else
        {
            cameraController.SetActive(true);
            cameraPanel.SetActive(true);
        }
    }

    public void ScoreStreakCheck()
    {
        if (scoreStreak >= scoreStreakMax)
        {

            isServe = true;
            String window = GetGoodNextNFC();
            validNfc = window;
            ResetScoreStreak();
            SetNfcPanel(" Great Work!\n Serve dish to " + window + "!\n\n (TAP ON " + window + " NFC)");

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
//        CmdPrint(buttonNumber);
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.green;
        CmdIncrementScore();
        
        PlayCorrectSound();
        CmdIncreasePlayerScore();
        
        StartCoroutine(ResetButtonColour(0.5f, buttonNumber));
    }

    private void ThisButtonWasNotPressed(int buttonNumber)
    {
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.red;

        PlayIncorrectSound();

        StartCoroutine(ResetButtonColour(0.5f, buttonNumber));
    }

    private IEnumerator ResetButtonColour(float x, int buttonNumber)
    {
        yield return new WaitForSecondsRealtime(x);
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.white;

    }

    [Command]
    private void CmdPrint(int buttonNumber)
    {
        gameController.PrintOut(buttonNumber);
    }

    private void PlayFailSound()
    {
        source.PlayOneShot(badNoises[failSwitch], VolumeOfSoundEffects);
        failSwitch = (failSwitch + 1) % numberOfFails;
    }

    private void PlayCorrectSound()
    {
        source.PlayOneShot(correctActions[switcher], VolumeOfSoundEffects);
        switcher = (switcher + 1) % numberOfButtonSounds;
    }

    public void DisableOkayButtonsOnPanels()
    {
        nfcOkayButton.SetActive(false);
        micOkayButton.SetActive(false);
        shakeOkayButton.SetActive(false);
    }


    private void PlayIncorrectSound()
    {
        source.PlayOneShot(incorrectActions[switcher], VolumeOfSoundEffects);
        Vibrate();
        switcher = (switcher + 1) % numberOfButtonSounds;

    }

    [Command]
    public void CmdUpdateChefPrefab()
    {
        var chefs = GameObject.FindGameObjectsWithTag("ChefPrefab");
        foreach (GameObject chef in chefs)
        {
            if (chef.GetComponent<ChefController>().arrow.GetComponent<Image>().color == PlayerColour)
            {
                //UPDATE PREFAB HERE
                List<GameObject> hatParts = chef.GetComponent<ChefController>().hat;
                foreach (GameObject part in hatParts)
                {
                    Material hatColour = new Material(part.GetComponent<MeshRenderer>().material);
                    hatColour.color = PlayerColour;
                    part.GetComponent<MeshRenderer>().material = hatColour;
                }
            }
        }
    }
    

    private void Vibrate()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    [Command]
    private void CmdPrintName(string name)
    {
        Debug.Log(name);
    }


    public void BackToMainMenu()
    {
        Application.Quit();
    }

    private void DisplayTopChef(string topChef)
    {
        topChefText.text = topChef;
    }

    [Command]
    public void CmdIncrementScore()
    {
        PlayerScore++;
    }
    
    [Command]
    public void CmdSetNameAndColour(string name, Color colour)
    {
        PlayerUserName = name;
        PlayerColour = colour;
    }

    public void SetRndCompletePanel()
    {
        if (topChefPush == "") topChefText.text = "Fuck";
        else topChefText.text = topChefPush;
        roundCompletePanel.SetActive(true);
    }
    
    [Command]
    public void CmdSetPlayerReady()
    {
        isSetupComplete = true;
    }

    [Command]
    public void CmdSetNetworkID(uint ID)
    {
        PlayerNetworkID = ID;
    }

    private void UpdateScore(int x)
    {
        scoreText.text = x.ToString();
    }

    [Command]
    public void CmdSetShake(bool shake)
    {
        isShaking = shake;
    }

    [Command]
    public void CmdSetNFCRace(bool isNFCFinished)
    {
        isNFCRace = isNFCFinished;
    }
    
    private void CheckGroupActivity()
    {
        switch (activityNumber)
        {
            case 0: 
                CmdSetShake(ShakeListener.shaking);
                break;
                    
            case 1:
                CmdSetNFCRace(nfcPanel.activeSelf);
                break;
                    
            default:
                //
                Console.WriteLine("Fucked!");
                break;
        }
    }


    public void GenerateGoodStation(List<List<string>> stations)
    {
        foreach (var station in stations)
        {
            GoodStations.Add(new Station(station));
        }
    }    
    
    public void GenerateBadStation(List<List<string>> stations)
    {
        foreach (var station in stations)
        {
            BadStations.Add(new Station(station));
        }
    }

    public void printStations()
    {
        foreach (var station in GoodStations)
        {
            foreach (var item in station.GetStationItems())
            {
                Debug.Log("Item: " + item);
            }
        }
        foreach (var station in BadStations)
        {
            foreach (var item in station.GetStationItems())
            {
                Debug.Log("Item: " + item);
            }
        }
    }

    private string GetGoodNextNFC()
    {
        Random rand = new Random();
        int x = rand.Next(0, GoodStations.Count);
        return GoodStations[x].GetItem(nfcValue);
    }    
    
    private string GetBadNextNFC()
    {
        Random rand = new Random();
        int x = rand.Next(0, BadStations.Count);
        return BadStations[x].GetItem(nfcValue);
    }

    public void StartNFCRace(int x)
    {
        switch (x)
        {
            case 0:
                validNfc = GoodStations[0].GetItem(nfcValue);
                break;
            case 1:
                validNfc = GoodStations[1].GetItem(nfcValue);
                break;
            case 2:
                validNfc = BadStations[0].GetItem(nfcValue);
                break;
            case 3:
                validNfc = BadStations[1].GetItem(nfcValue);
                break;
        }
        
        SetNfcPanel(validNfc);
    }

}

