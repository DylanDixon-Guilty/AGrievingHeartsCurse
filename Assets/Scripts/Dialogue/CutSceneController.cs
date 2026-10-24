using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// Handles loading the converasation the Main Gameplay Scenes loaded
/// </summary>
public class CutSceneController : MonoBehaviour
{
    [Header("Conversation Launch")]
    [SerializeField] private bool startConversationOnSceneLoad = true;

    [Header("Existing Cutscene System")]
    [SerializeField] private CutSceneOverlay cutSceneOverlay;

    [Header("Shot Assets")]
    [SerializeField] private CutSceneShot[] shots;

    [Header("Speech Bubble Panels")]
    [SerializeField] private RectTransform normalSpeechBubble;
    [SerializeField] private RectTransform reversedSpeechBubble;

    [Tooltip("Put name of conversation for testing a conversation outside of the Gameplay Scene")]
    [SerializeField] private string testConversationName;

    [Header("CutScene UI")]
    [SerializeField] private GameObject cutSceneDialogueUIPrefab;

    private Dictionary<string, CutSceneShot> shotsByID;

    private void Awake()
    {
        BuildShotLookup();
    }

    /// <summary>
    /// At the start, see if a conversation has been passed, if so, start the conversation and load the proper shots(artwork).
    /// </summary>
    private void Start()
    {
        if (!startConversationOnSceneLoad) //If testing in Scene, skip this.
        {
            return;
        }

        string conversationName = CutsceneLaunchData.ConsumeConversation();

        // Allow the existing Inspector test field to serve as a fallback when entering the scene directly.
        if (string.IsNullOrWhiteSpace(conversationName))
        {
            conversationName = testConversationName;
        }

        if (string.IsNullOrWhiteSpace(conversationName))
        {
            Debug.LogWarning("CutsceneController: No conversation name was provided.", this);
            return;
        }

        StartCoroutine(StartConversationNextFrame(conversationName));
    }

    private System.Collections.IEnumerator StartConversationNextFrame(string _conversationName)
    {
        // Wait for the CutScenes scene to initialize.
        yield return null;

        if (DialogueManager.instance == null)
        {
            Debug.LogError("CutSceneController: No persistent Dialogue Manager found.", this);
            yield break;
        }

        if (cutSceneDialogueUIPrefab == null)
        {
            Debug.LogError("CutSceneController: CutScenes Dialogue UI prefab is not assigned.", this);
            yield break;
        }

        // Find the Canvas belonging to the persistent Dialogue Manager.
        Canvas managerCanvas = DialogueManager.instance.GetComponentInChildren<Canvas>(true);

        if (managerCanvas == null)
        {
            Debug.LogError("CutSceneController: No Canvas found under the persistent Dialogue Manager.", this);
            yield break;
        }

        // Create the CutScenes UI under the persistent manager's Canvas.
        GameObject cutSceneUI = Instantiate(cutSceneDialogueUIPrefab, managerCanvas.transform, false);

        // Switch Pixel Crushers to the newly instantiated UI.
        DialogueManager.UseDialogueUI(cutSceneUI);
        if (!FindSpeechBubbles(cutSceneUI))
        {
            yield break;
        }

        // Start the conversation only after assigning the UI.
        DialogueManager.StartConversation(_conversationName);
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

    private bool FindSpeechBubbles(GameObject _dialogueUI)
    {
        Transform normalPanel = _dialogueUI.transform.Find("Dialogue-Panel/SpeechBubbleNormal-Panel/SpeechBubble");
        Transform reversedPanel = _dialogueUI.transform.Find("Dialogue-Panel/SpeechBubbleReversed-Panel/SpeechBubble");

        if (normalPanel == null || reversedPanel == null)
        {
            Debug.LogError("CutSceneController: Could not find one or both SpeechBubble objects. " + "Check the CutSceneDialogue-UI prefab hierarchy.", this);
            return false;
        }

        normalSpeechBubble = normalPanel.GetComponent<RectTransform>();
        reversedSpeechBubble = reversedPanel.GetComponent<RectTransform>();

        return normalSpeechBubble != null && reversedSpeechBubble != null;
    }

    [ContextMenu("Testing/Start Test Conversation")]
    private void StartTestConversation()
    {
        StartConversation();
    }
}