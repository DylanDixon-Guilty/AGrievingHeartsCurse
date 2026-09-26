using UnityEngine;

/// <summary>
/// Handles automatically triggering a dialogue sequence or cutscene
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueStarter _dialogueStarter;

    /// <summary>
    /// Starts the dialogue when the player reaches the location this script is attached to
    /// </summary>
    public void TriggerDialogue()
    {
        if (!enabled)
        {
            return;
        }

        _dialogueStarter.StartDialogue();
    }

    /// <summary>
    /// Disables this trigger so the dialogue cannot automatically start again
    /// </summary>
    public void DisableTrigger()
    {
        enabled = false;
    }
}
