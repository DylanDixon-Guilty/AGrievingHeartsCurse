using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    public bool GetFlag(string _flagName)
    {
        if (flags.TryGetValue(_flagName, out bool _value))
        {
            return _value;
        }

        return false;
    }

    public void SetFlag(string _flagName, bool _value)
    {
        flags[_flagName] = _value;
    }

    public Dictionary<string, bool> GetAllFlags()
    {
        return new Dictionary<string, bool>(flags);
    }

    public void SetAllFlags(Dictionary<string, bool> _savedFlags)
    {
        flags = new Dictionary<string, bool>(_savedFlags);
    }

    /// <summary>
    /// For testing to see if the flag is being called correctly
    /// </summary>
    public void DebugAllFlags()
    {
        Debug.Log($"HiddenLanguageDiscovered: {GetFlag("HiddenLanguageDiscovered")}");
        Debug.Log($"PillarUnlocked: {GetFlag("PillarUnlocked")}");
    }
}
