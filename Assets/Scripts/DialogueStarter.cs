using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueStarter : MonoBehaviour
{
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private string conversationName;
    [SerializeField] private PlayerInteraction _playerInteraction;

    private void OnEnable()
    {
        DialogueManager.instance.conversationEnded += OnConversationEnd;
    }

    private void OnDisable()
    {
        DialogueManager.instance.conversationEnded -= OnConversationEnd;
    }

    /// <summary>
    /// Makes a specific conversation begin based on the of 'conversationName'
    /// </summary>
    public void StartDialogue()
    {
        mainUI.SetActive(false);
        _playerInteraction.SetDialogueActive(true);

        DialogueManager.StartConversation(conversationName);
    }

    private void OnConversationEnd(Transform actor)
    {
        mainUI.SetActive(true);
        _playerInteraction.SetDialogueActive(false);
    }
}
