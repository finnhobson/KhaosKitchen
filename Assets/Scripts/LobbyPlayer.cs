using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

    public GameObject ParentPref;
    public Button ReadyButton;
    public Text PlayerName;
    public Text ButtonText;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
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
        ReadyButton.enabled = true;
        ButtonText.text = "JOIN";
    }

    private void SetupOtherPlayer()
    {
        PlayerName.text = "NotMyPlayer";
        ReadyButton.enabled = false;
        ButtonText.text = "...";
    }
}
