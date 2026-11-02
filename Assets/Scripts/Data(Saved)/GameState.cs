using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides access to progression flags stored persistently in GameSession
/// </summary>
public class GameState : MonoBehaviour
{
    /// <summary>
    /// Get a flag and returns the value of a progression flag
    /// </summary>
    public bool GetFlag(string _flagName)
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("GameState: GameSession was not found.");
            return false;
        }

        return GameSession.Instance.GetFlag(_flagName);
    }

    /// <summary>
    /// Sets or updates a progression flag
    /// </summary>
    public void SetFlag(string _flagName, bool _value)
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("GameState: GameSession was not found.");
            return;
        }

        GameSession.Instance.SetFlag(_flagName, _value);
    }

    /// <summary>
    /// Returns a copy of all progression flags
    /// </summary>
    public Dictionary<string, bool> GetAllFlags()
    {
        if (GameSession.Instance == null)
        {
            return new Dictionary<string, bool>();
        }

        return GameSession.Instance.GetAllFlags();
    }

    public void SetAllFlags(Dictionary<string, bool> _savedFlags)
    {
        if (GameSession.Instance == null || _savedFlags == null)
        {
            Debug.LogWarning("GameState: Cannot restore flags.");
            return;
        }

        foreach (KeyValuePair<string, bool> _flag in _savedFlags)
        {
            GameSession.Instance.SetFlag(_flag.Key, _flag.Value);
        }
    }

    public void DebugAllFlags()
    {
        Debug.Log($"HiddenLanguageDiscovered: {GetFlag("HiddenLanguageDiscovered")}");
        Debug.Log($"PillarUnlocked: {GetFlag("PillarUnlocked")}");
    }
}