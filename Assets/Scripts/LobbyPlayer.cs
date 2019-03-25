using System;
using System.Collections;
using System.Net.Sockets;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class LobbyPlayer : NetworkLobbyPlayer {

    //Unity game objects
    public GameObject ParentPref;
    public Button ReadyButton;
    public Text PlayerName;
    public Text ButtonText;
    public InputField InputField;
    public Button ApplyNameButton;
    public Button ColourButton;
    public GameObject CoverPanel;
    public Text OtherPlayerName;

    [SyncVar (hook = "UpdatePlayerName")] public string UserName;
    [SyncVar (hook = "UpdatePlayerColour")] public Color UserColour;

    //Primitive variables
    private int palleteSelector = 0;
    private int palleteCount;
    
    //Booleans
    private bool allowEnter = true;
    
    private static List<Color> Pallete = new List<Color>( new Color[] {Color.black, Color.cyan, Color.white,  Color.green, Color.blue, Color.magenta, Color.red, Color.yellow, });
    
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        UserColour = Color.black;
        palleteCount = Pallete.Count;
        ReadyButton.enabled = false;
    }

    private void Update()
    {        
        if (allowEnter && (InputField.text.Length > 0) && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))) {
            OnSetNameClick();
            allowEnter = false;
        } 
        else allowEnter = InputField.isFocused;
    }

    public void OnClickReady()
    {
        SendReadyToBeginMessage();
        ButtonText.text = "READY";
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
        ParentPref = GameObject.FindGameObjectWithTag("ParentPref");
        gameObject.transform.SetParent(ParentPref.transform);
        if (isLocalPlayer)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupOtherPlayer();
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        SetupLocalPlayer();
    }

    private void SetupLocalPlayer()
    {
        CoverPanel.SetActive(false);
        UserName = "MyPlayer";
//        PlayerName.text = UserName;
        PlayerName.color = new Color(0, 0, 1.0f);
        ReadyButton.enabled = false;
        ButtonText.text = "JOIN";
        InputField.enabled = true;
        ApplyNameButton.onClick.AddListener(OnSetNameClick);
        ColourButton.onClick.AddListener(OnSetColourClick);
    }

    private void SetupOtherPlayer()
    {
        UserName = "NotMyPlayer";
        ReadyButton.enabled = false;
        ButtonText.text = "...";
        InputField.enabled = false;
        CoverPanel.SetActive(true);
        OtherPlayerName.text = UserName;
    }

    public void OnSetNameClick()
    {
        string userName = InputField.text;
        ReadyButton.enabled = true;
        CmdUpdateUserName(userName);
    }
    
    public void OnSetColourClick()
    {
        palleteSelector = (palleteSelector + 1) % palleteCount;
        //Going to have to do the thing that happens in the tutorial
        Color colour = Pallete[palleteSelector];
        CmdUpdateUserColour(colour);
        PlayerName.color = colour;
    }

    [Command]
    public void CmdPrint(string p)
    {
        Debug.Log(p);
    }

    [Command]
    public void CmdUpdateUserName(string name)
    {
        UserName = name;
    }

    [Command]
    public void CmdUpdateUserColour(Color colour)
    {
        UserColour = colour;
    }

    public void UpdatePlayerName(string name)
    {
        PlayerName.text = name;
        OtherPlayerName.text = name;
    }

    public void UpdatePlayerColour(Color colour)
    {
        OtherPlayerName.color = colour;
    }
    
}
