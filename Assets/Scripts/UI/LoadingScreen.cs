using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Handles spawning the loading screen between scenes
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen _instance;

    public static bool IsTransitioning { get; private set; }

    [SerializeField] private float _bufferTime = 1.5f;

    private bool _isLoading;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// If loading into a new scene, spawn in the loading Screen
    /// </summary>
    public static void LoadScene(string _sceneName)
    {
        if (IsTransitioning)
            return;

        GameObject _prefab = Resources.Load<GameObject>("LoadingScreen");

        if (_prefab == null)
        {
            Debug.LogError("LoadingScreenCanvas prefab could not be found in Resources.");
            return;
        }

        GameObject _loadingScreen = Instantiate(_prefab);
        LoadingScreen _controller = _loadingScreen.GetComponent<LoadingScreen>();

        if (_controller == null)
        {
            Debug.LogError("LoadingScreenCanvas is missing LoadingScreenController.");
            Destroy(_loadingScreen);
            return;
        }

        IsTransitioning = true;
        _controller.StartCoroutine(_controller.LoadSceneRoutine(_sceneName));
    }

    /// <summary>
    /// The time frame of which the loading screen stays on screen
    /// </summary>
    private IEnumerator LoadSceneRoutine(string _sceneName)
    {
        if (_isLoading) yield break;

        _isLoading = true;

        AsyncOperation _operation = SceneManager.LoadSceneAsync(_sceneName);

        while (!_operation.isDone) yield return null;

        // Give the destination scene time to initialize
        yield return new WaitForSeconds(_bufferTime);

        // Prevent a held click from carrying into gameplay
        while (Mouse.current != null && Mouse.current.leftButton.isPressed) yield return null;

        // Give input one frame to settle before removing the blocker
        yield return null;

        IsTransitioning = false;
        _instance = null;
        Destroy(gameObject);
    }
}