using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class CutSceneController : MonoBehaviour
{
    [Header("Existing Cutscene System")]
    [SerializeField] private CutSceneOverlay cutSceneOverlay;

    [Header("Shot Assets")]
    [SerializeField] private CutSceneShot[] shots;

    [Header("Speech Bubble Panels")]
    [SerializeField] private RectTransform normalSpeechBubble;
    [SerializeField] private RectTransform reversedSpeechBubble;

    [Tooltip("Put name of conversation for testing a conversation outside of the Gameplay Scene")]
    [SerializeField] private string testConversationName;

    private Dictionary<string, CutSceneShot> shotsByID;

    private void Awake()
    {
        BuildShotLookup();
    }

    private void BuildShotLookup()
    {
        shotsByID = new Dictionary<string, CutSceneShot>();

        foreach (CutSceneShot shot in shots)
        {
            if (shot == null || string.IsNullOrWhiteSpace(shot.shotID))
            {
                continue;
            }

            if (shotsByID.ContainsKey(shot.shotID))
            {
                Debug.LogWarning($"Duplicate CutsceneShot ID: {shot.shotID}", this);
                continue;
            }

            shotsByID.Add(shot.shotID, shot);
        }
    }

    public void ApplyShot(string shotID)
    {
        if (shotsByID == null)
        {
            BuildShotLookup();
        }

        if (string.IsNullOrWhiteSpace(shotID) || !shotsByID.TryGetValue(shotID, out CutSceneShot shot))
        {
            Debug.LogWarning($"Cutscene shot not found: {shotID}", this);
            return;
        }

        // A blank artwork name means keep the current artwork.
        if (!string.IsNullOrWhiteSpace(shot.artworkName))
        {
            if (cutSceneOverlay == null)
            {
                Debug.LogError("CutSceneOverlay is not assigned.", this);
                return;
            }

            cutSceneOverlay.ShowArtwork(shot.artworkName);
        }

        // Position both speech bubble variants.
        // Pixel Crushers still determines which panel displays the current dialogue entry.
        switch (shot.displayMode)
        {
            case CutsceneDisplayMode.NormalSpeechBubble:
                PositionBubble(normalSpeechBubble, shot.screenX, shot.screenY);
                break;

            case CutsceneDisplayMode.ReversedSpeechBubble:
                PositionBubble(reversedSpeechBubble, shot.reversedScreenX, shot.reversedScreenY);
                break;

            case CutsceneDisplayMode.Narration:
                // Narration stays anchored at the bottom.
                break;
        }
    }

        /// <summary>
        /// The position the Speech Bubble will sit
        /// </summary>
        private void PositionBubble(RectTransform bubble, float screenX, float screenY)
        {
            if (bubble == null)
            {
                return;
            }

            Vector2 position = new Vector2(screenX, screenY);

            bubble.anchorMin = position;
            bubble.anchorMax = position;
            bubble.anchoredPosition = Vector2.zero;
        }

        public void StartConversation()
        {
        if (string.IsNullOrWhiteSpace(testConversationName))
        {
            Debug.LogWarning("Enter a conversation name in CutsceneController.", this);
            return;
        }

        DialogueManager.StartConversation(testConversationName);
        }

    [ContextMenu("Testing/Start Test Conversation")]
    private void StartTestConversation()
    {
        StartConversation();
    }
}
