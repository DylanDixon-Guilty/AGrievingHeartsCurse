using UnityEngine;

/// <summary>
/// Provides methods for updating persistent game progression
/// </summary>
public class GameProgression : MonoBehaviour
{
    /// <summary>
    /// Sets a progression flag in GameSession (Script)
    /// </summary>
    public void SetProgressFlag(string _flagName, bool _value)
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("GameProgression: GameSession was not found. " + $"Could not set flag '{_flagName}'.");
            return;
        }

        GameSession.Instance.SetFlag(_flagName, _value);
    }

    /// <summary>
    /// Unlocks the ability to travel to the real-world scene
    /// </summary>
    public void UnlockFlag(string _flagName)
    {
        SetProgressFlag(_flagName, true);
    }
}