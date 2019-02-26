using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction
{
    public int ID { get; set; }
    public string Action { get; set; }
    public float Timer { get; set; }
    public bool IsActive { get; set; }

    public void printInstruction()
    {
        Debug.Log("ID: " + ID + "\nAction: " + Action + "\n Timer: " + Timer + "\n IsActive: " + IsActive);
    }
}
