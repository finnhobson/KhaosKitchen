using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Station : MonoBehaviour
{
    public Dictionary<string, bool> StationItems = new Dictionary<string, bool>();

    public Station(List<string> items)
    {
        foreach (var tag in items)
        {
            StationItems.Add(tag, true);
        }
    }

    public bool CheckForMatch(string check)
    {
        bool value;
        if (StationItems.TryGetValue(check, out value))
        {
            if(value) UpdateItems(check);
            return value;
        }
        return false;
    }

    private void UpdateItems(string check)
    {
        foreach (var key in StationItems.Keys)
        {
            StationItems[key] = true;
        }

        StationItems[check] = false;
    }

    public string GetItem()
    {
        Random rand = new Random();
        string item = "";
        while (true)
        {
            int x = rand.Next(0, StationItems.Count);
            if (!StationItems.ElementAt(x).Value) continue;
            item = StationItems.Keys.ElementAt(x);
            break;
        }
        
        return item;
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
