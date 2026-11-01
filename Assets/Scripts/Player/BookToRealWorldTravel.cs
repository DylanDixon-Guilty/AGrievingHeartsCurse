using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles holding down Tab to travel from the scrapbook mode to the real world
/// </summary>
public class BookToRealWorldTravel : MonoBehaviour
{
    [Header("Travel Settings")]
    [SerializeField] private float tabHoldDuration = 1f;
    [SerializeField] private string travelUnlockFlag = "RealWorldTravelUnlocked";

    private float tabHoldTimer;
    private bool travelRequestHandled;

    private void Update()
    {
        if (Keyboard.current == null)
        {
            ResetTabHold();
            return;
        }

        if (!Keyboard.current.tabKey.isPressed)
        {
            ResetTabHold();
            return;
        }

        if (travelRequestHandled)
        {
            return;
        }

        tabHoldTimer += Time.deltaTime;

        if (tabHoldTimer >= tabHoldDuration)
        {
            travelRequestHandled = true;
            CheckTravelUnlock();
        }
    }

    /// <summary>
    /// Checks to see if the player has unlocked Travel. If so, let player go to real world
    /// </summary>
    private void CheckTravelUnlock()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("ScrapbookTravelController: GameSession was not found.");
            return;
        }

        if (!GameSession.Instance.GetFlag(travelUnlockFlag))
        {
            Debug.Log("Real-world travel is still locked.");
            return;
        }

        string _currentScene = SceneManager.GetActiveScene().name;

        GameSession.Instance.SetScrapbookReturnScene(_currentScene);
        Debug.Log($"Saved scrapbook return scene: {_currentScene}");

        SceneManager.LoadScene("Ethan'sHouse");
    }

    private void ResetTabHold()
    {
        tabHoldTimer = 0f;
        travelRequestHandled = false;
    }

}