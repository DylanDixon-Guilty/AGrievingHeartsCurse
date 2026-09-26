using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles where the player goes and what they can do: pick up, inspect, move, talk to NPC, etc.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform defaultPosition;
    [SerializeField] private Transform currentNode;
    [SerializeField] private MouseIconController _mouseIconController;
    [SerializeField] private Inventory _inventory;

    private Stack<Transform> _nodeHistory = new Stack<Transform>();
    private bool isDialogueActive;

    private void Awake()
    {
        _camera.transform.position = defaultPosition.transform.position;
    }

    private void Update()
    {
        if (isDialogueActive) //Checks to see if the player is in a dialogue
        {
            _mouseIconController.SetCursor(MouseIconType.Default);
            return;
        }

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CheckHover(hit);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleInteraction(hit);
            }
        }
        else
        {
            _mouseIconController.SetCursor(MouseIconType.Default);
        }
    }

    /// <summary>
    /// Checks what the player is hovering over and changes the mouse icon accordingly.
    /// </summary>
    private void CheckHover(RaycastHit _hit)
    {
        int layer = _hit.collider.gameObject.layer;

        if (layer == LayerMask.NameToLayer("MoveForward"))
        {
            _mouseIconController.SetCursor(MouseIconType.GoForward);
        }
        else if (layer == LayerMask.NameToLayer("MoveRight"))
        {
            _mouseIconController.SetCursor(MouseIconType.GoRight);
        }
        else if (layer == LayerMask.NameToLayer("MoveLeft"))
        {
            _mouseIconController.SetCursor(MouseIconType.GoLeft);
        }
        else if (layer == LayerMask.NameToLayer("Lootable"))
        {
            _mouseIconController.SetCursor(MouseIconType.Grab);
        }
        else if (layer == LayerMask.NameToLayer("Inspect"))
        {
            _mouseIconController.SetCursor(MouseIconType.Inspect);
        }
        else
        {
            _mouseIconController.SetCursor(MouseIconType.Default);
        }
    }

    /// <summary>
    /// Handles the interaction when the player clicks on an object.
    /// </summary>
    private void HandleInteraction(RaycastHit _hit)
    {
        CameraHotSpot hotspot = _hit.collider.GetComponent<CameraHotSpot>();
        Lootable lootable = _hit.collider.GetComponent<Lootable>();
        DialogueStarter dialogueStarter = _hit.collider.GetComponent<DialogueStarter>();

        if (hotspot != null)
        {
            MoveCamera(hotspot.Node);
        }

        if (lootable != null)
        {
            lootable.PickUp(_inventory);
        }

        if (dialogueStarter != null)
        {
            dialogueStarter.StartDialogue();
        }
    }

    /// <summary>
    /// Will move the camera based on where the mouse is clicking on.
    /// </summary>
    private void MoveCamera(Transform _node)
    {
        if (currentNode != null)
        {
            _nodeHistory.Push(currentNode);
        }

        currentNode = _node;
        _camera.transform.position = _node.position;

        CheckDialogueTrigger(_node); //Check if there's a dialogue trigger script on this node
    }

    /// <summary>
    /// Will go back to the previous Node the player was on, if applicable.
    /// </summary>
    public void GoBack()
    {
        if (_nodeHistory.Count == 0)
        {
            Debug.Log("I can't go back.");
            return;
        }

        currentNode = _nodeHistory.Pop();
        _camera.transform.position = currentNode.position;
    }

    /// <summary>
    /// Sets whether dialogue is currently active.
    /// </summary>
    public void SetDialogueActive(bool active)
    {
        isDialogueActive = active;
    }

    /// <summary>
    /// Checks to see if the node the player is on has a DialogueTrigger Script
    /// </summary>
    private void CheckDialogueTrigger(Transform _node)
    {
        DialogueTrigger dialogueTrigger = _node.GetComponent<DialogueTrigger>();

        if (dialogueTrigger != null)
        {
            dialogueTrigger.TriggerDialogue();
        }
    }
}
