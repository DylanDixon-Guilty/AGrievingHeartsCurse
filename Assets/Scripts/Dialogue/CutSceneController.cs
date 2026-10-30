using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles dialogue UI switching and cutscene conversations.
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

    [Tooltip("Put name of conversation here for testing a conversation outside of the Gameplay Scene.")]
    [SerializeField] private string testConversationName;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueUIPrefab;

    private Dictionary<string, CutSceneShot> shotsByID;

    private void Awake()
    {
        BuildShotLookup();
    }

    /// <summary>
    /// Switches to the assigned UI when the scene loads. Starts a conversation only when prompted
    /// </summary>
    private void Start()
    {
        string conversationName = null;

        if (startConversationOnSceneLoad)
        {
            conversationName = CutsceneLaunchData.ConsumeConversation();

            // Use the Inspector test conversation if no conversation was passed
            if (string.IsNullOrWhiteSpace(conversationName))
            {
                conversationName = testConversationName;
            }

            if (string.IsNullOrWhiteSpace(conversationName))
            {
                Debug.LogWarning("CutSceneController: No conversation name was provided. " + "The dialogue UI will still be switched.", this);
            }
        }

        StartCoroutine(SwitchDialogueUIAndStartConversation(conversationName));
    }

    private System.Collections.IEnumerator SwitchDialogueUIAndStartConversation(
        string _conversationName)
    {
        // Allow the scene to initialize
        yield return null;

        if (DialogueManager.instance == null)
        {
            Debug.LogError("CutSceneController: No persistent Dialogue Manager found.", this);
            yield break;
        }

        if (dialogueUIPrefab == null)
        {
            Debug.LogError("CutSceneController: Dialogue UI prefab is not assigned.", this);
            yield break;
        }

        // Find the Canvas belonging to the persistent Dialogue Manager
        Canvas managerCanvas = DialogueManager.instance.GetComponentInChildren<Canvas>(true);

        if (managerCanvas == null)
        {
            Debug.LogError("CutSceneController: No Canvas found under the persistent Dialogue Manager.", this);
            yield break;
        }

        // Create the UI assigned to this scene
        GameObject newDialogueUI = Instantiate(dialogueUIPrefab, managerCanvas.transform, false);

        // Switch Pixel Crushers to the newly instantiated UI
        DialogueManager.UseDialogueUI(newDialogueUI);

        // Find speech bubbles only in the CutScenes scene. Normal gameplay UI does not need these references.
        if (SceneManager.GetActiveScene().name == "CutScenes")
        {
            if (!FindSpeechBubbles(newDialogueUI))
            {
                Debug.LogError("CutSceneController: Could not initialize the CutScenes speech bubbles.", this);
                yield break;
            }
        }

        // Remove old runtime or scene UI instances after switching
        for (int i = managerCanvas.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = managerCanvas.transform.GetChild(i);

            if (child.gameObject == newDialogueUI)
            {
                continue;
            }

            string childName = child.name.Replace("(Clone)", "").Trim();

            if (childName == "Dialogue-UI" || childName == "CutScenesDialogue-UI")
            {
                Destroy(child.gameObject);
            }
        }

        // Only start a conversation if a name was provided
        if (!string.IsNullOrWhiteSpace(_conversationName))
        {
            Debug.Log("Starting conversation. Active conversation state: " + DialogueManager.isConversationActive);
            DialogueManager.StartConversation(_conversationName);
        }
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

        // A blank artwork name means keep the current artwork
        if (!string.IsNullOrWhiteSpace(shot.artworkName))
        {
            if (cutSceneOverlay == null)
            {
                Debug.LogError("CutSceneOverlay is not assigned.", this);
                return;
            }

            cutSceneOverlay.ShowArtwork(shot.artworkName);
        }

        // Position the appropriate speech bubble
        switch (shot.displayMode)
        {
            case CutsceneDisplayMode.NormalSpeechBubble:
                PositionBubble(normalSpeechBubble, shot.screenX, shot.screenY);
                break;

            case CutsceneDisplayMode.ReversedSpeechBubble:
                PositionBubble(reversedSpeechBubble, shot.reversedScreenX, shot.reversedScreenY);
                break;

            case CutsceneDisplayMode.Narration: // Narration stays anchored at the bottom
                break;
        }
    }

    /// <summary>
    /// Positions a speech bubble using normalized anchor coordinates
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
            Debug.LogWarning("Enter a conversation name in CutSceneController.", this);
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
            Debug.LogError("CutSceneController: Could not find one or both SpeechBubble objects. " + "Check the CutScenes Dialogue UI prefab hierarchy.", this);
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

    /// <summary>
    /// Call this function in a Dialogue Node to load another scene
    /// </summary>
    private void LoadScene(string _sceneName)
    {


        DialogueManager.StopConversation();
        SceneManager.LoadScene(_sceneName);
    }
}