﻿using System;
using System.Collections;
using System.Net.Sockets;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class LobbyPlayer : NetworkLobbyPlayer {

    public GameObject ParentPref;
    public Button ReadyButton;
    public Text PlayerName;
    public Text ButtonText;
    public InputField InputField;
    public Button ApplyNameButton;
    public Button ColourButton;

    [SyncVar (hook = "UpdatePlayerName")] public string UserName;
    [SyncVar] public Color UserColour;

    private int palleteSelector = 0;
    private int palleteCount;
    
    //Booleans
    private bool allowEnter = true;
    
    private static System.Collections.Generic.List<String> lsi = new System.Collections.Generic.List<string>(new string[] { "Grab", "Fetch", "Grate", "Grill", "Melt", "Serve", "Stir", "Chop", "Cut", "Mash", "Season", "Flambé", "Bake", "Fry", "Taste", "Microwave" });

    private static List<Color> Pallete = new List<Color>( new Color[] {Color.black, Color.cyan, Color.white,  Color.green, Color.blue, Color.magenta, Color.red, Color.yellow, });
    
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        UserColour = Color.black;
        palleteCount = Pallete.Count;
    }

    private void Update()
    {
//        PlayerName.text = UserName;
        
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
        UserName = "MyPlayer";
        PlayerName.text = UserName;
        PlayerName.color = new Color(0, 0, 1.0f);
        ReadyButton.enabled = true;
        ButtonText.text = "JOIN";
        InputField.enabled = true;
        ApplyNameButton.onClick.AddListener(OnSetNameClick);
        ColourButton.onClick.AddListener(OnSetColourClick);
    }

    private void SetupOtherPlayer()
    {
        UserName = "NotMyPlayer";
        PlayerName.text = UserName;
        ReadyButton.enabled = false;
        ButtonText.text = "...";
        InputField.enabled = false;
    }

    public void OnSetNameClick()
    {
        string userName = InputField.text;
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
    }
    
    /*
     * REMEMBER TO ADD THE RETURN KEY LISTENER
     */
}
