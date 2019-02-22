using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * Hold and handle gameplay data
 */
public class GameStateHandler
{
    public int RoundNumber { get; set; }
    public int TeamTotalScore { get; set; }
    
    private List<string> userNames = new List<string>();
    private Dictionary<string, int> userProfiles = new Dictionary<string, int>();

    public List<string> UserNames
    {
        get { return userNames; }
        set { userNames = value; }
    }
    public Dictionary<string, int> UserProfiles
    {
        get { return userProfiles; }
        set { userProfiles = value; }
    }

    public GameStateHandler(List<string> userNames)
    {
        RoundNumber = 0;
        TeamTotalScore = 0;
        UserNames = userNames;
        
        //Set each players initial score to 0
        foreach (var userName in userNames)
        {
            UserProfiles.Add(userName, 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
