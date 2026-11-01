using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Will handle the player movement in the real world (Ethan's House)
/// Will also return the player to the current level they were in when clicking on the scrapbook
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float gravity = -9f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Camera")]
    public float mouseSensitivity = 2f;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Scrapbook Interaction")]
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private LayerMask scrapbookLayer;

    private CharacterController characterController;
    private float verticalVelocity;
    private float cameraPitch;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        LookAround();
        MovePlayer();
        HandleCursor();
        ScrapbookInteraction();
    }

    /// <summary>
    /// Move the player based on WASD
    /// </summary>
    private void MovePlayer()
    {
        Vector2 _movementInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                _movementInput.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                _movementInput.y -= 1f;

            if (Keyboard.current.dKey.isPressed)
                _movementInput.x += 1f;

            if (Keyboard.current.aKey.isPressed)
                _movementInput.x -= 1f;
        }

        _movementInput = Vector2.ClampMagnitude(_movementInput, 1f);

        Vector3 _movement = transform.right * _movementInput.x + transform.forward * _movementInput.y;

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (characterController.isGrounded && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity continuously throughout the jump.
        verticalVelocity += gravity * Time.deltaTime;

        _movement.y = verticalVelocity;
        characterController.Move(_movement * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Move the camera based on the Mouse
    /// </summary>
    private void LookAround()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        Vector2 _mouseDelta = Mouse.current.delta.ReadValue();

        float _mouseX = _mouseDelta.x * mouseSensitivity;
        float _mouseY = _mouseDelta.y * mouseSensitivity;

        transform.Rotate(Vector3.up * _mouseX);

        cameraPitch -= _mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    /// <summary>
    /// If the player press escape (the pause button) unlock the mouse
    /// </summary>
    private void HandleCursor()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Handles when the player clicks on the scrapbook, send them to the scene they came from
    /// </summary>
    private void ScrapbookInteraction()
    {
        if (Mouse.current == null || interactionCamera == null)
        {
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Ray ray = interactionCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, scrapbookLayer))
        {
            ReturnToScrapbook();
        }
    }

    private void ReturnToScrapbook()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("PlayerController: GameSession was not found.");
            return;
        }

        string _returnScene = GameSession.Instance.ReturnScene;

        if (string.IsNullOrWhiteSpace(_returnScene))
        {
            Debug.LogWarning("PlayerController: No scrapbook return scene has been saved.");
            return;
        }

        SceneManager.LoadScene(_returnScene);
    }
}