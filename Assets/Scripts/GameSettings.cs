using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class GameSettings
{
    /*XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
     * HOW TO USE:
     * 
     * 1. Set up static var with get and set in GameSettings Class (here)
     * 2. Create syncvar in Game Controller to link to
     * 3. Update LoadSettings function in GameController
     * 4. Create new input field within Settings Panel within the LobbyManager 
     *     (Can copy and paste existing ones - set content type to whatever needed)
     * 5. State a text object within the LobbyManager 
     *     (i.e. public Text exampleFieldText; )
     * 6. Link the input field text child to the new text object in LobbyManager (drag and drop)
     * 7. Update SetSettings, SetDefaultSettings and ResetSettingsToLast functions within LobbyManager
     *     (Don't forget text will be a string so much use conversion to relevant var)
     * 
     * XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
     */
    private static int rTime, iTime, pCount;
    private static float pointMultiplier;

    public static int PlayerCount
    {
        get
        {
            return pCount;
        }
        set
        {
            pCount = value;
        }
    }

    public static int RoundTime
    {
        get
        {
            return rTime;
        }
        set
        {
            rTime = value;
        }
    }

    public static int InstructionTime
    {
        get
        {
            return iTime;
        }
        set
        {
            iTime = value;
        }
    }

    public static float PointMultiplier
    {
        get
        {
            return pointMultiplier;
        }
        set
        {
            pointMultiplier = value;
        }
    }

}
