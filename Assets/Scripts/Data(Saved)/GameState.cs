using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    public bool GetFlag(string flagName)
    {
        if (flags.TryGetValue(flagName, out bool value))
        {
            return value;
        }

        return false;
    }

    public void SetFlag(string flagName, bool value)
    {
        flags[flagName] = value;
    }

    public Dictionary<string, bool> GetAllFlags()
    {
        return new Dictionary<string, bool>(flags);
    }

    public void SetAllFlags(Dictionary<string, bool> savedFlags)
    {
        flags = new Dictionary<string, bool>(savedFlags);
    }

    /// <summary>
    /// For testing to see if the flag is being called correctly
    /// </summary>
    public void DebugFlag(string flagName)
    {
        Debug.Log($"{flagName}: {GetFlag(flagName)}");
    }
}
