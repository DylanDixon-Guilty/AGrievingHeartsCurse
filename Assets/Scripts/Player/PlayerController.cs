using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Will handle the player movement in the real world (Ethan's House)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float gravity = -9f;

    [Header("Camera")]
    [SerializeField] private Transform playerCamera;
    public float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

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
}