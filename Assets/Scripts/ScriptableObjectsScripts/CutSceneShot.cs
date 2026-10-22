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
    [Tooltip("Called in the sequence of a dialogue Node")]
    [Header("Shot Identification")]
    public string shotID;

    [Tooltip("Leave blank to keep the current artwork.")]
    [Header("Artwork")]
    public string artworkName;

    [Tooltip("Which dialogue panel will this shot use")]
    [Header("Dialogue Presentation")]
    public CutsceneDisplayMode displayMode = CutsceneDisplayMode.NormalSpeechBubble;

    [Header("Normal Speech Bubble Position")]
    [Range(0f, 1f)]
    public float screenX = 0.5f;

    [Range(0f, 1f)]
    public float screenY = 0.75f;

    [Header("Reversed Speech Bubble Position")]
    [Range(0f, 1f)]
    public float reversedScreenX = 0.5f;

    [Range(0f, 1f)]
    public float reversedScreenY = 0.75f;
}