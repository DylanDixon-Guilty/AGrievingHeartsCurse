using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles where the player goes and what they can do: pick up, inspect, move, talk to NPC, etc.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public bool IsDialogueActive => isDialogueActive;

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform defaultPosition;
    [SerializeField] private Transform currentNode;
    [SerializeField] private MouseIconController _mouseIconController;
    [SerializeField] private Inventory _inventory;

    private Stack<Transform> _nodeHistory = new Stack<Transform>();
    private bool isDialogueActive;

    private void Awake()
    {
        if (_camera == null)
        {
            Debug.LogWarning("PlayerInteraction: Camera is not assigned.");
            return;
        }

        if (RestoreNavigationState())
        {
            return;
        }

        if (defaultPosition != null)
        {
            currentNode = defaultPosition;
            _camera.transform.position = defaultPosition.position;
        }
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
            // A named cutscene takes priority over normal camera movement.
            if (!string.IsNullOrWhiteSpace(hotspot.cutSceneName))
            {
                hotspot.StartCutScene(hotspot.cutSceneName);
                return;
            }

            // Blank cutscene name: behave like a normal camera hotspot.
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

    /// <summary>
    /// Saves the current scrapbook scene, camera node, and navigation history
    /// </summary>
    public void SaveNavigationState()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("PlayerInteraction: GameSession was not found.");
            return;
        }

        Transform nodeToSave = currentNode != null ? currentNode : defaultPosition;

        if (nodeToSave == null)
        {
            Debug.LogWarning("PlayerInteraction: No camera node is available to save.");
            return;
        }

        List<string> nodeNames = new List<string>();

        foreach (Transform node in _nodeHistory)
        {
            if (node != null)
            {
                nodeNames.Add(node.name);
            }
        }

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        GameSession.Instance.SetNavigationState(currentScene, nodeToSave.name, nodeNames);
        Debug.Log($"Saved scrapbook navigation: Scene = {currentScene}, " + $"Node = {nodeToSave.name}, History Count = {nodeNames.Count}");
    }

    /// <summary>
    /// Restores the saved scrapbook camera node and navigation history
    /// </summary>
    private bool RestoreNavigationState()
    {
        if (GameSession.Instance == null)
        {
            return false;
        }

        if (GameSession.Instance.ReturnScene != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            return false;
        }

        string savedNodeName = GameSession.Instance.ReturnNode;

        if (string.IsNullOrWhiteSpace(savedNodeName))
        {
            return false;
        }

        GameObject savedNodeObject = GameObject.Find(savedNodeName);

        if (savedNodeObject == null)
        {
            Debug.LogWarning($"PlayerInteraction: Could not find saved camera node '{savedNodeName}'.");
            return false;
        }

        currentNode = savedNodeObject.transform;
        _camera.transform.position = currentNode.position;

        _nodeHistory.Clear();

        List<string> savedNodeNames =
            GameSession.Instance.ReturnNodeHistory;

        // Stack enumerates from top to bottom, so push in reverse order
        for (int i = savedNodeNames.Count - 1; i >= 0; i--)
        {
            GameObject nodeObject = GameObject.Find(savedNodeNames[i]);

            if (nodeObject != null)
            {
                _nodeHistory.Push(nodeObject.transform);
            }
            else
            {
                Debug.LogWarning($"PlayerInteraction: Could not restore history node '{savedNodeNames[i]}'.");
            }
        }

        Debug.Log($"Restored scrapbook navigation: Node = {savedNodeName}, " + $"History Count = {_nodeHistory.Count}");

        GameSession.Instance.ClearSessionData();

        return true;
    }
}