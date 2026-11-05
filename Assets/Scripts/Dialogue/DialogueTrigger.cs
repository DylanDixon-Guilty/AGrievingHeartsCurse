using UnityEngine;

/// <summary>
/// Handles automatically triggering a dialogue sequence or cutscene
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueStarter _dialogueStarter;
    [SerializeField] private bool conversationStartOnTrigger = true;

    [Header("Trigger Completion")]
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private string completionFlag;

    private void Start()
    {
        // Check whether this trigger has already been completed
        if (triggerOnce && !string.IsNullOrWhiteSpace(completionFlag))
        {
            if (GameSession.Instance != null && GameSession.Instance.GetFlag(completionFlag)) enabled = false;
        }
    }

    /// <summary>
    /// Starts a specific dialogue when the player reaches the location this script is attached to
    /// </summary>
    public void TriggerDialogue()
    {
        if (!enabled || !conversationStartOnTrigger) return;

        _dialogueStarter.StartDialogue();
    }

    /// <summary>
    /// Handles trigger completion after the conversation ends
    /// </summary>
    public void DisableTrigger()
    {
        if (triggerOnce)
        {
            if (string.IsNullOrWhiteSpace(completionFlag))
            {
                Debug.LogWarning($"DialogueTrigger on {gameObject.name} has Trigger Once enabled, but no completion flag is assigned.");
            }
            else if (GameSession.Instance != null)
            {
                GameSession.Instance.SetFlag(completionFlag, true);
            }
        }

        if (triggerOnce) enabled = false;
    }
}