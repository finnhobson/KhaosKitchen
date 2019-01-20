﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShowLocalCanvas : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        if (isLocalPlayer) GetComponentInChildren<Canvas>().enabled = true;
        else GetComponentInChildren<Canvas>().enabled = false;
    }
	

}
