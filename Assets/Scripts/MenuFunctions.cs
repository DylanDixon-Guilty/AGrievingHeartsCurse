using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Will handle the basic functions of the menus, like Options or Main Menu
/// </summary>
public class MenuFunctions : MonoBehaviour
{
    [SerializeField] private GameObject playerInteractionUI; // The UI that houses the backpack, inventory, and backarrow
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject selectionMenu; // The default page for the pauseMenu
    [SerializeField] private GameObject optionsMenu;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundEffectsSlider;

    public bool isPaused = false;

    private void Start()
    {
        pauseMenu.SetActive(false);
        selectionMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) PauseGame();
    }

    /// <summary>
    /// Put the player into a new scene (based on the text typed in the button inspector)
    /// </summary>
    public void NewScene(string nameOfScene)
    {
        SceneManager.LoadScene(nameOfScene);
    }

    /// <summary>
    /// When player presses the ESC key, pause the game (or unpause if already paused)
    /// </summary>
    public void PauseGame()
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

    /// <summary>
    /// When the player presses the Options button, go to the Options page
    /// </summary>
    public void GoToOptions()
    {
        selectionMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    /// <summary>
    /// If the presses the back arrow, go back to the Selection menu
    /// REMINDER: THIS FUNCTION MAY CHANGE LATER
    /// </summary>
    public void GoBack()
    {
        optionsMenu.SetActive(false);
        selectionMenu.SetActive(true);
    }

    /// <summary>
    /// Updates the Master volume
    /// </summary>
    public void SetMasterVolume()
    {
        SetVolume("Master", masterSlider.value);
    }

    /// <summary>
    /// Updates the Music volume
    /// </summary>
    public void SetMusicVolume()
    {
        SetVolume("Music", musicSlider.value);
    }

    /// <summary>
    /// Updates the Sound Effects volume
    /// </summary>
    public void SetSoundEffectsVolume()
    {
        SetVolume("SoundEffects", soundEffectsSlider.value);
    }

    /// <summary>
    /// Adjust volume for the player
    /// </summary>
    private void SetVolume(string _parameterName, float _sliderValue)
    {
        float _volume = Mathf.Log10(Mathf.Max(_sliderValue, 0.0001f)) * 20f;
        audioMixer.SetFloat(_parameterName, _volume);
    }
}