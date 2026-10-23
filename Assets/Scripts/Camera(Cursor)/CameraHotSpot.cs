using UnityEngine;
using UnityEngine.SceneManagement;
using PixelCrushers.DialogueSystem;

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

    public void StartCutScene(string _cutScene)
    {
        if (cutSceneName != null)
        {
            SceneManager.LoadScene(CutScene_Scene);
            _cutScene = cutSceneName;
        }
    }
}
