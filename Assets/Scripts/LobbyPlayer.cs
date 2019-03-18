using System;
using System.Collections;
using System.Collections.Generic;
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

    [SyncVar] public string UserName;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        ApplyNameButton.onClick.AddListener(OnButtonClick);
//        else InputField.DeactivateInputField();
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
        PlayerName.text = "MyPlayer";
        PlayerName.color = new Color(0, 0, 1.0f);
        ReadyButton.enabled = true;
        ButtonText.text = "JOIN";
    }

    private void SetupOtherPlayer()
    {
        PlayerName.text = "NotMyPlayer";
        ReadyButton.enabled = false;
        ButtonText.text = "...";
    }

    public void OnButtonClick()
    {
        string userName = InputField.text;
        PlayerName.text = userName;
        CmdUpdateUserName(userName);
        
//        CmdPrint(pName);
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
}
