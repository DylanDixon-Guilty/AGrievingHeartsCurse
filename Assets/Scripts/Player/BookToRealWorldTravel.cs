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

    [SerializeField] private bool bypassTravelUnlock = true;
    [SerializeField] private PlayerInteraction _playerInteraction;
    [SerializeField] private Inventory _inventory;

    private float tabHoldTimer;
    private bool travelRequestHandled;

    private void Update()
    {
        if (_playerInteraction != null && _playerInteraction.IsDialogueActive)
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
    /// Checks to see if the player has unlocked Travel. If so, let player go to the real world
    /// </summary>
    private void CheckTravelUnlock()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("ScrapbookTravelController: GameSession was not found.");
            return;
        }

        if (!bypassTravelUnlock && !GameSession.Instance.GetFlag(travelUnlockFlag))
        {
            Debug.Log("Real-world travel is still locked.");
            return;
        }

        if (_playerInteraction == null)
        {
            Debug.LogWarning("ScrapbookTravelController: PlayerInteraction is not assigned.");
            return;
        }

        _playerInteraction.SaveNavigationState();

        if (GameSession.Instance.ReturnScene != SceneManager.GetActiveScene().name)
        {
            Debug.LogWarning("ScrapbookTravelController: Navigation state was not saved correctly.");
            return;
        }

        if (_inventory == null)
        {
            Debug.LogWarning("BookToRealWorldTravel: Inventory is not assigned.");
            return;
        }

        _inventory.SaveInventoryState(); //Saves the inventory of the player before leaving the scene

        SceneManager.LoadScene("Ethan'sHouse");
    }

    private void ResetTabHold()
    {
        tabHoldTimer = 0f;
        travelRequestHandled = false;
    }
}