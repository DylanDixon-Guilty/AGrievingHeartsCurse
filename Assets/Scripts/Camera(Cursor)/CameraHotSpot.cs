using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The node the camera will spawn to based on where the player clicked
/// </summary>
public class CameraHotSpot : MonoBehaviour
{
    [Tooltip("Put name of conversation to start CutScene")]
    public string cutSceneName;

    [SerializeField] private Transform node;

    private string CutScene_Scene = "CutScenes";

    public Transform Node => node;

    public void SetNode(Transform _node)
    {
        node = _node;
    }

    /// <summary>
    /// If cutSceneName is not blank, send player to CutScenes and start corresponding conversation
    /// </summary>
    public void StartCutScene(string _cutScene)
    {
        // Prefer the conversation configured on this hotspot.
        // Fall back to the method argument if the field is blank.
        string conversationToStart = !string.IsNullOrWhiteSpace(cutSceneName) ? cutSceneName : _cutScene;

        if (string.IsNullOrWhiteSpace(conversationToStart))
        {
            Debug.LogWarning("CameraHotSpot: No cutscene conversation name was provided.", this);
            return;
        }

        // Save the conversation name before changing scenes.
        CutsceneLaunchData.SetConversation(conversationToStart);

        // Load the cutscene scene.
        SceneManager.LoadScene(CutScene_Scene);
    }
}
