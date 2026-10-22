using UnityEngine;

public enum CutsceneDisplayMode
{
    NormalSpeechBubble,
    ReversedSpeechBubble,
    Narration
}

/// <summary>
/// Determines what kind of shot the scene have (the artwork, speech bubble, and where the speech bubble will be)
/// </summary>
[CreateAssetMenu(fileName = "New Cutscene Shot", menuName = "Cutscenes/Cutscene Shot")]
public class CutSceneShot : ScriptableObject
{
    [Header("Shot Identification")]
    public string shotID;

    [Header("Artwork")]
    [Tooltip("Leave blank to keep the current artwork.")]
    public string artworkName;

    [Header("Dialogue Presentation")]
    public CutsceneDisplayMode displayMode = CutsceneDisplayMode.NormalSpeechBubble;

    [Header("Speech Bubble Position")]
    [Tooltip("Normalized screen position: X and Y range from 0 to 1.")]
    [Range(0f, 1f)]
    public float screenX = 0.5f;

    [Range(0f, 1f)]
    public float screenY = 0.75f;
}