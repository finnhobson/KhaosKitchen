using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

/*
 *
 *  MILK:                     BEZoSotfgA==
 *  CHEESE:                   BH5nSotfgA==
 *  PASTA:                    BDNoSotfgA==
 *  LENTILS:                  BJJnSotfgA==
 *  WHISK:                    BG5oSotfgA==
 *  CHOPPING BOARD:           BFpoSotfgA==
 *  SPOON:                    BJloSotfgA==
 *  PLATE:                    BFtoSotfgA==
 *  GLASS:                    BG9oSotfgA==
 *  FOOD WASTE:               BH1nSotfgA==
 *  NTERGALACTIC\nBLACK HOLE: BINoSotfgA==
 *  PLASTIC:                  BIRoSotfgA== 
 *  SPARE 1:                  BJhoSotfgA==
 *  SPARE 2:                  
 */
public class Player : NetworkBehaviour {

    //Custom objects
    public GameController gameController;
    public InstructionController InstructionController;
    public CameraController cameraController;

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
    public Text scoreText, instructionText, timerText, gpsText, roundScoreText, topChefText, countdownText, roundNumberText, nameText, micVolumeText, groupMessageText, gameOverText;
    public GameObject nfcPanel, micPanel, shakePanel, gameOverPanel, roundCompletePanel, roundStartPanel, shopPanel, groupMessagePanel, cameraPanel;
    public Text nfcText, micText, shakeText, cameraText, debugText;
    public GameObject nfcOkayButton, micOkayButton, shakeOkayButton;
    public GameObject fullScreenPanel;
    public Text fullScreenPanelText;
    public GameObject backgroundPanel;

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
    public int cameraColour = -1;
    [SyncVar] private string nfcValue = "";
    private string validNfc = "";
    [SyncVar]public string validNfcRace = "";
    public int playerCount;
    public int instTime;
    public bool easyPhoneInteractions = true;
//    private HashSet<String> validNfc = new HashSet<String>{"Food Waste","Recycling Bin","Window A","Window B"};
   // public MicListener micListener;

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


    //Group activity
    [SyncVar] public bool isGroupActive;
    [FormerlySerializedAs("isGroupComplete")] [SyncVar] public bool isGroupActivityPlayerComplete;
    [SyncVar] public bool isShaking;
    [SyncVar] public int activityNumber = 1;
    [FormerlySerializedAs("isNFCRace")] [SyncVar] public bool isNFCRaceStarted;
    [SyncVar] public int nfcStation;
    [SyncVar] public bool IsNFCRaceCompleted;
    [SyncVar] public bool wait;

    private int i = 0;

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
        transform.SetAsLastSibling();

        debugText.text = "yes";

        if (isLocalPlayer)
        {
            CmdSetNetworkID(PlayerNetworkID);
            CmdSetNameAndColour(PlayerUserName, PlayerColour);
            CmdSetPlayerReady();        
        }

        StartInstTimer();
        VolumeOfSoundEffects = Volume;
        nameText.text += PlayerUserName;
        nameText.color = PlayerColour;
        scoreText.color = PlayerColour;

       // micListener.enabled = false;
        cameraController.enabled = false;
    }

    private void Update()
    {
        debugText.text = "Start";
        if (wait) return;
        debugText.text = "Second";




        groupMessagePanel.SetActive(isGroupActive);

        if (isGroupActive)
        {
            if (isLocalPlayer)
            {
                debugText.text = "Inside";

                CheckGroupActivity();
                debugText.text = "Inside 2";

                TurnEverythingOff();
                debugText.text = "Inside 3";

            }
        }

        debugText.text = "After";




        if (groupMessagePanel.activeSelf) return;

        debugText.text = "GroupPanel";


        if (!isClient) return;

        debugText.text = "isClient";


        if (gameOverPanel.activeSelf) return;

        debugText.text = "gameOver";

        if (roundCompletePanel.activeSelf)
        {
            TurnEverythingOff();
            return;
        }

        debugText.text = "roundComplete";


        if (!timerStarted && gameController.isGameStarted)
        {
            StartInstTimer();
            timerStarted = true;
            //if (isLocalPlayer) CmdChangeHatColour();
        }

        else if (gameController.isGameStarted && gameController.roundTimeLeft > 0)
        {           
            UpdateInstTimeLeft();
            if (instTimeLeft < 0 && isLocalPlayer)
            {
                string tmp = GetBadNextNFC();
                validNfc = tmp;
                CmdFail(instructionText.text, tmp);
                PlayFailSound();
                StartInstTimer();
                isFail = true;
            }
            else
            {
                SetTimerText(instTimeLeft.ToString("F1"));
            }

        
          /*  if (micPanel.activeSelf)
            {
                if (micListener.MicLoudness > 0.15f)
                {
                    micPanel.SetActive(false);
                    micActive = false;
                    micListener.enabled = false;
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            } */ /*

            if (cameraPanel.activeSelf)
            {
                bool cameraBool = false;
                if (cameraColour == 0) cameraBool = cameraController.red;
                else if (cameraColour == 1) cameraBool = cameraController.orange;
                else if (cameraColour == 2) cameraBool = cameraController.yellow;
                else if (cameraColour == 3) cameraBool = cameraController.green;
                else if (cameraColour == 4) cameraBool = cameraController.blue;

                if (cameraBool)
                {
                    cameraController.enabled = false;
                    cameraPanel.SetActive(false);
                    cameraController.red = false;
                    cameraController.blue = false;
                    cameraController.green = false;
                    cameraController.orange = false;
                    cameraController.yellow = false;
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            } */

            if (shakePanel.activeSelf)
            {
                //shakeClick(Instruction text to be completed by shaking, matching that in activeInstructions);
                
                if (ShakeListener.shaking)
                {
                    shakePanel.SetActive(false);
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            }


            if (nfcPanel.activeSelf)
            {
                nfcValue = NfcCheck();
                if (validNfc.Equals(nfcValue))
                {
                    nfcPanel.SetActive(false);
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            }

        }
        else
        {
            SetTimerText("0");
            nfcPanel.SetActive(false);
            shakePanel.SetActive(false);
           // micPanel.SetActive(false);
        }
        debugText.text = "End";

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
        
        //fridge
        if (value == "BEZoSotfgA==") return GoodStations[0].GetStationItem(0);
        if (value == "BH5nSotfgA==") return GoodStations[0].GetStationItem(1);

        //cupboard
        if (value == "BDNoSotfgA==") return GoodStations[1].GetStationItem(0);
        if (value == "BJJnSotfgA==") return GoodStations[1].GetStationItem(1);
        
        //prep
        if (value == "BG5oSotfgA==") return GoodStations[2].GetStationItem(0);
        if (value == "BFpoSotfgA==") return GoodStations[2].GetStationItem(1);
        
        //serve
        if (value == "BJloSotfgA==") return GoodStations[3].GetStationItem(0);
        if (value == "BFtoSotfgA==") return GoodStations[3].GetStationItem(1);

        //bin A
        if (value == "BG9oSotfgA==") return BadStations[0].GetStationItem(0);
        if (value == "BH1nSotfgA==") return BadStations[0].GetStationItem(1);
    
        //bin B
        if (value == "BINoSotfgA==") return BadStations[1].GetStationItem(0);
        if (value == "BIRoSotfgA==") return BadStations[1].GetStationItem(1);
        
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
       // micListener.enabled = true;
        micActive = true;
        micText.text = text;
    }

    public void SetCameraPanel(int colour, string text)
    {
        cameraController.enabled = true;
        cameraPanel.SetActive(true);
        cameraColour = colour;
        cameraText.text = text;
    }

    public void OnClickRefresh()
    {
        cameraController.enabled = false;
        cameraController.enabled = true;
    }

    public void OnClickCameraButton()
    {
        if (cameraPanel.activeInHierarchy)
        {
            cameraPanel.SetActive(false);
            cameraController.enabled = false;
        }
        else
        {
            cameraController.enabled = true;
            cameraPanel.SetActive(true);
        }
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
    public void CmdUpdateChefPrefab(int item)
    {
        var chefs = GameObject.FindGameObjectsWithTag("ChefPrefab");
        foreach (GameObject chef in chefs)
        {
            if (chef.GetComponent<ChefController>().arrow.GetComponent<Image>().color == PlayerColour)
            {
                if (item == 1)
                {
                    GameObject roller = chef.GetComponent<ChefController>().roller;
                    if (roller.activeInHierarchy == false && PlayerScore >= 25)
                    {
                        roller.SetActive(true);
                        PlayerScore -= 25;
                        RpcCloseShop();
                    }
                    else RpcVibrate();
                }

                if (item == 2)
                {
                    GameObject ogreEars = chef.GetComponent<ChefController>().ogreEars;
                    Material ogreColour = chef.GetComponent<ChefController>().ogreColour;
                    List<GameObject> skin = chef.GetComponent<ChefController>().skin;
                    if (ogreEars.activeInHierarchy == false && PlayerScore >= 100)
                    {
                        ogreEars.SetActive(true);
                        foreach (GameObject s in skin)
                        {
                            s.GetComponent<MeshRenderer>().material = ogreColour;
                        }
                        PlayerScore -= 100;
                        RpcCloseShop();
                    }
                    else RpcVibrate();
                }

                if (item == 3)
                {
                    GameObject crown = chef.GetComponent<ChefController>().crown;
                    List<GameObject> hatParts = chef.GetComponent<ChefController>().hat;
                    if (crown.activeInHierarchy == false && PlayerScore >= 250)
                    {
                        crown.SetActive(true);
                        foreach (GameObject part in hatParts)
                        {
                            part.SetActive(false);
                        }
                        PlayerScore -= 250;
                        RpcCloseShop();
                    }
                    else RpcVibrate();
                }
            }
        }
    }

    [ClientRpc]
    void RpcCloseShop()
    {
        shopPanel.SetActive(false);
    }

    [ClientRpc]
    void RpcVibrate()
    {
        Vibrate();
    }

    [Command]
    public void CmdChangeHatColour()
    {
        var chefs = GameObject.FindGameObjectsWithTag("ChefPrefab");
        foreach (GameObject chef in chefs)
        {
            if (chef.GetComponent<ChefController>().arrow.GetComponent<Image>().color == PlayerColour)
            {
                //UPDATE HAT COLOUR
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

    public void OnClickShopButton1()
    {
        CmdUpdateChefPrefab(1);
    }

    public void OnClickShopButton2()
    {
        CmdUpdateChefPrefab(2);
    }

    public void OnClickShopButton3()
    {
        CmdUpdateChefPrefab(3);
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
        GameObject.FindGameObjectWithTag("lobbyBackground").SetActive(true);
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
        roundScoreText.text = x.ToString();
    }

    [Command]
    public void CmdSetShake(bool shake)
    {
        isShaking = shake;
    }

    [Command]
    public void CmdSetNFCRace(bool isNFCFinished)
    {
        IsNFCRaceCompleted = isNFCFinished;
        if (IsNFCRaceCompleted) validNfcRace = "";
    }
    
    private void CheckGroupActivity()
    {
        switch (activityNumber)
        {
            case 0:
                //groupMessageText.text = "Look at main screen \n (shaking)";

                CmdSetShake(ShakeListener.shaking);

                break;

            case 1:
                debugText.text = "check 1";

                nfcValue = NfcCheck();
                debugText.text = "check 2";


                if (!isNFCRaceStarted)
                {
                    StartNFCRace();
                    debugText.text = "check 3";
                }

                //                else if (!IsNFCRaceCompleted && isNFCRaceStarted) CmdSetNFCRace(nfcPanel.activeSelf);
                else if (!IsNFCRaceCompleted && isNFCRaceStarted)
                {
                    CmdSetNFCRace(validNfcRace.Equals(nfcValue));
                    debugText.text = "check 4";

                }
                else
                {
                    groupMessageText.text = "DONE!";
                    debugText.text = "check 5";

                    wait = true;
                }
             
                break;
                    
            default:
                //
                Console.WriteLine("Fucked!");
                i -= 999999999;
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

    public void PrintStations()
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

    public void StartNFCRace()
    {
        nfcValue = NfcCheck();
        debugText.text = "START";

        Debug.Log("StartNFC");
        switch ( nfcStation )
        {
            case 0:
                validNfcRace = GoodStations[0].GetItem(nfcValue) ;
                break;
            case 1:
                validNfcRace = GoodStations[1].GetItem(nfcValue);
                break;
            case 2:
                validNfcRace = GoodStations[2].GetItem(nfcValue);
                break;
            case 3:
                validNfcRace = GoodStations[3].GetItem(nfcValue);
                break;
            case 4:
                validNfcRace = BadStations[0].GetItem(nfcValue);
                break;
            case 5:
                validNfcRace = BadStations[1].GetItem(nfcValue);
                break;
            default:
                validNfcRace = GoodStations[0].GetItem(nfcValue);
                break;
        }
        groupMessageText.text = "ALL HANDS ON DECK!\n\nLOOK AT THE MAIN SCREEN!";

        CmdSetValidNfcRace(validNfcRace);
         debugText.text = "START 2";


        IsNFCRaceCompleted = false;
        isNFCRaceStarted = true;
          debugText.text = "START 3";

    }

    [Command]
    public void CmdSetValidNfcRace(string tmp)
    {
        validNfcRace = tmp;
    }

    private void TurnEverythingOff()
    {
        nfcPanel.SetActive(false);
        shakePanel.SetActive(false);
        micPanel.SetActive(false);
        micActive = false;
        //micListener.enabled = false;
        cameraPanel.SetActive(false);
        cameraController.enabled = false;
        cameraController.red = false;
        cameraController.blue = false;
        cameraController.green = false;
        cameraController.orange = false;
        cameraController.yellow = false;
    }



}

