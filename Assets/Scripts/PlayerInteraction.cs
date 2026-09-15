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

    private void Awake()
    {
        _camera.transform.position = defaultPosition.transform.position;
    }

    private void Update()
    {
        CheckHover();

        //Will detect if it hit a collider
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CameraHotSpot hotspot = hit.collider.GetComponent<CameraHotSpot>();
                Lootable lootable = hit.collider.GetComponent<Lootable>();

                if (hotspot != null)
                {
                    MoveCamera(hotspot.Node);
                }

                if (lootable != null)
                {
                    lootable.PickUp(_inventory);
                }
            }
        }
    }

    /// <summary>
    /// Will move the camera based on where the mouse is clicking on
    /// </summary>
    private void MoveCamera(Transform _node)
    {
        if (currentNode != null)
        {
            _nodeHistory.Push(currentNode);
        }

        currentNode = _node;
        _camera.transform.position = _node.position;
    }

    /// <summary>
    /// Will go back to the previous Node the player was on (If applicable)
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
    /// Will check what the player is hovering over and change the mouse icon accordingly
    /// </summary>
    private void CheckHover()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int layer = hit.collider.gameObject.layer;

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
        else
        {
            _mouseIconController.SetCursor(MouseIconType.Default);
        }
    }
}
