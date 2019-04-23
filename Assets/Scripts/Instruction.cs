using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction
{
    public float InstructionTimer { get; set; }
    public bool IsActive { get; set; }
    public int InstructionPlayerID { get; set; } //ID of player who's ordering instruction.
    public int ButtonPlayerID { get; set; }      //ID of player who's executing instruction.
    public int ButtonNumber { get; set; }
}
