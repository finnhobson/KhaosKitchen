using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionHandler
{
    private IDictionary<string, Instruction> Instructions = new Dictionary<string, Instruction>();

    public Instruction GetInstruction(string key)
    {
        return Instructions[key];
    }

    public void AddValue(string s, Instruction i)
    {
        Instructions.Add(s, i);
    }

    public void ClearInstructions()
    {
        Instructions.Clear();
    }

    public int GetButtonPlayerID(string s)
    {
        return GetInstruction(s).ButtonPlayerID;
    }
    
    public int GetInstructionPlayerID(string s)
    {
        return GetInstruction(s).InstructionPlayerID;
    }

    public int GetButtonNumber(string s)
    {
        return GetInstruction(s).ButtonNumber;
    }

    public float GetTimer(string s)
    {
        return GetInstruction(s).InstructionTimer;
    }

    public bool GetIsActive(string s)
    {
        return GetInstruction(s).IsActive;
    }

    public void SetIsActive(string s)
    {
        GetInstruction(s).IsActive = true;
    }

    public void SetNotActive(string s)
    {
        GetInstruction(s).IsActive = false;
    }

    public void SetButtonNumber(string s, int buttonNumber)
    {
        GetInstruction(s).ButtonNumber = buttonNumber;
    }

    public void SetInstructionPlayerID(string s, int id)
    {
        GetInstruction(s).InstructionPlayerID = id;
        GetInstruction(s).IsActive = true;
    }
    
    public void SetButtonPlayerID(string s, int id)
    {
        GetInstruction(s).ButtonPlayerID = id;
    }

    public void PrintInstructions()
    {
        foreach (var instruction in Instructions)
        {
            if(instruction.Value.IsActive) Debug.Log(instruction.Key + " : IP = " + instruction.Value.InstructionPlayerID + " : Active = " + instruction.Value.IsActive);
        }
    }

    public void InstructionCompleted(string action)
    {
        GetInstruction(action).InstructionPlayerID = 69;
        SetNotActive(action);
    }
}
