using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    [Header("Required to fill in")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private string conversationName;
    [SerializeField] private PlayerInteraction _playerInteraction;
    [SerializeField] private GameState _gameState;

    [Header("Optional")]
    [SerializeField] private string flagToSetOnConversationEnd;
    [SerializeField] private string requiredFlag;
    [SerializeField] private ObjectSpawner _objectSpawner;
    [SerializeField] private DialogueTrigger _dialogueTrigger;
    [SerializeField] private CutSceneOverlay _cutSceneOverlay;

    private void OnDisable()
    {
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.conversationEnded -= OnConversationEnd;
            DialogueManager.instance.conversationEnded -= OnExternalConversationEnd;
        }
    }

    public void StartDialogue()
    {
        mainUI.SetActive(false);
        _playerInteraction.SetDialogueActive(true);

        DialogueManager.instance.conversationEnded += OnConversationEnd;
        DialogueManager.StartConversation(conversationName);
    }

    public static bool StartExternalDialogue(string _conversationName)
    {
        DialogueStarter dialogueStarter = FindFirstObjectByType<DialogueStarter>();

        if (dialogueStarter == null)
        {
            Debug.LogError("No active DialogueStarter was found in the gameplay scene. Cannot prepare the return conversation.");
            return false;
        }

        if (dialogueStarter.mainUI == null || dialogueStarter._playerInteraction == null)
        {
            Debug.LogError("The selected DialogueStarter is missing its Main UI or PlayerInteraction reference.");
            return false;
        }

        if (DialogueManager.instance == null)
        {
            Debug.LogError("DialogueManager was not found. Cannot start the return conversation.");
            return false;
        }

        dialogueStarter.PrepareForExternalDialogue(_conversationName);
        return true;
    }

    private void PrepareForExternalDialogue(string _conversationName)
    {
        mainUI.SetActive(false);
        _playerInteraction.SetDialogueActive(true);

        DialogueManager.instance.conversationEnded -= OnExternalConversationEnd;
        DialogueManager.instance.conversationEnded += OnExternalConversationEnd;

        DialogueManager.StartConversation(_conversationName);
    }

    private void OnExternalConversationEnd(Transform _actor)
    {
        DialogueManager.instance.conversationEnded -= OnExternalConversationEnd;

        mainUI.SetActive(true);
        _playerInteraction.SetDialogueActive(false);
    }

    private void OnConversationEnd(Transform _actor)
    {
        DialogueManager.instance.conversationEnded -= OnConversationEnd;

        bool _progressionCompleted = false;

        if (!string.IsNullOrEmpty(flagToSetOnConversationEnd))
        {
            if (string.IsNullOrEmpty(requiredFlag) || _gameState.GetFlag(requiredFlag))
            {
                _gameState.SetFlag(flagToSetOnConversationEnd, true);
                _progressionCompleted = true;
            }
        }

        if (_objectSpawner != null && _progressionCompleted)
            _objectSpawner.SpawnObject();

        if (_dialogueTrigger != null)
            _dialogueTrigger.DisableTrigger();

        if (_cutSceneOverlay != null)
            _cutSceneOverlay.ClearArtwork();

        mainUI.SetActive(true);
        _playerInteraction.SetDialogueActive(false);
    }
}