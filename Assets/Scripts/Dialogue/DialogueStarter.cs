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
    [SerializeField] private CutSceneOverlay _cutSceneOverlay;
    [SerializeField] private ObjectSpawner _objectSpawner;

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
        bool _progressionCompleted = false;

        if (!string.IsNullOrEmpty(flagToSetOnConversationEnd))
        {
            if (string.IsNullOrEmpty(requiredFlag) || _gameState.GetFlag(requiredFlag))
            {
                _gameState.SetFlag(flagToSetOnConversationEnd, true);
                _progressionCompleted = true;
            }
        }

        //If spawning in a new object after the dialogue has concluded
        if (_objectSpawner != null && _progressionCompleted)
        {
            _objectSpawner.SpawnObject();
        }

        _cutSceneOverlay.ClearArtwork();
        mainUI.SetActive(true);
        _playerInteraction.SetDialogueActive(false);
    }
}
