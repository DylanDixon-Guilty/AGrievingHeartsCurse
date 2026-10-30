using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores game progression data that persist between scenes
/// </summary>
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }
    public string ScrapbookReturnScene { get; private set; }

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        // Keep only one GameSession alive.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Returns the value of a progression flag. Flags that have not been set return false
    /// </summary>
    public bool GetFlag(string _flagName)
    {
        if (flags.TryGetValue(_flagName, out bool _value))
        {
            return _value;
        }

        return false;
    }

    /// <summary>
    /// Sets or updates a progression flag
    /// </summary>
    public void SetFlag(string _flagName, bool _value)
    {
        if (string.IsNullOrWhiteSpace(_flagName))
        {
            Debug.LogWarning("GameSession: Cannot set a flag with an empty name.");
            return;
        }

        flags[_flagName] = _value;
        Debug.Log($"{_flagName} had been set to {_value}");
    }

    /// <summary>
    /// Returns the player to the previous scene once clicking on the Book (in Ethan'sHouse)
    /// </summary>
    public void SetScrapbookReturnScene(string _sceneName)
    {
        if (string.IsNullOrWhiteSpace(_sceneName))
        {
            Debug.LogWarning("GameSession: Cannot save an empty scrapbook return scene.");
            return;
        }

        ScrapbookReturnScene = _sceneName;
    }

    /// <summary>
    /// Removes all scenes saved from the current session
    /// </summary>
    public void ClearScrapbookReturnScene()
    {
        ScrapbookReturnScene = null;
    }

    /// <summary>
    /// Removes all progression flags from the current session
    /// </summary>
    public void ClearSessionData()
    {
        flags.Clear();
    }
}