﻿using System.Collections; using System.Collections.Generic; using UnityEngine;  public class SplatScript : MonoBehaviour {      //private float spinx = 0;     //private float spiny = 0;     //public float spinz = 0;      public float rotationsPerSecond;           // Start is called before the first frame update           // Update is called once per frame     void Update()     {         transform.Rotate(0, 0, rotationsPerSecond*Time.deltaTime*360);      } }  