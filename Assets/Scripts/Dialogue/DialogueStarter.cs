using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueStarter : MonoBehaviour
{
    [SerializeField] private GameObject mainUI;
    [SerializeField] private string conversationName;

    [Header("Use flag if player has comepleted progression in story")]
    [SerializeField] private string flagToSetOnConversationEnd;
    [SerializeField] private string requiredFlag;

    [SerializeField] private PlayerInteraction _playerInteraction;
    [SerializeField] private GameState _gameState;

    private void OnDisable()
    {
        if (DialogueManager.instance != null) DialogueManager.instance.conversationEnded -= OnConversationEnd;

    }

    /// <summary>
    /// Makes a specific conversation begin based on the of 'conversationName'
    /// </summary>
    public void StartDialogue()
    {
        mainUI.SetActive(false);
        _playerInteraction.SetDialogueActive(true);

        DialogueManager.instance.conversationEnded += OnConversationEnd;
        DialogueManager.StartConversation(conversationName);
    }

    private void OnConversationEnd(Transform _actor)
    {
        DialogueManager.instance.conversationEnded -= OnConversationEnd;

        if (!string.IsNullOrEmpty(flagToSetOnConversationEnd))
        {
            if (string.IsNullOrEmpty(requiredFlag) || _gameState.GetFlag(requiredFlag))
            {
                _gameState.SetFlag(flagToSetOnConversationEnd, true);
            }
        }

        mainUI.SetActive(true);
        _playerInteraction.SetDialogueActive(false);
    }
}
