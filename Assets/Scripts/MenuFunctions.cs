using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Will handle the basic functions of the menus, like Options or Main Menu
/// </summary>
public class MenuFunctions : MonoBehaviour
{
    [SerializeField] private GameObject playerInteractionUI; // The UI that houses the backpack, inventory, and backarrow
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject selectionMenu; // The default page for the pauseMenu
    [SerializeField] private GameObject optionsMenu;

    private bool isPaused = false;

    private void Start()
    {
        pauseMenu.SetActive(false);
        selectionMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    private void Update()
    {
        PauseGame();
    }

    public void NewScene(string nameOfScene)
    {
        SceneManager.LoadScene(nameOfScene);
    }

    /// <summary>
    /// When player presses the ESC key, pause the game (or unpause if already paused)
    /// </summary>
    private void PauseGame()
    {
        if (Keyboard.current.escapeKey.isPressed)
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                isPaused = true;
                if (playerInteractionUI != null)
                {
                    playerInteractionUI.SetActive(false);

                }
            }
            else
            {
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                isPaused = false;
                if (playerInteractionUI != null)
                {
                    playerInteractionUI.SetActive(true);

                }
            }
        }
    }
}
