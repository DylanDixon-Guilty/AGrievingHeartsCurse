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
}
