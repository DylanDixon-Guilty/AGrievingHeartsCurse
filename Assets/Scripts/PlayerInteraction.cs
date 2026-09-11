using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform defaultPosition;
    [SerializeField] private Transform currentNode;
    [SerializeField] private MouseIconController _mouseIconController;

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

                if (hotspot != null)
                {
                    MoveCamera(hotspot.Node);
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
                _mouseIconController.SetGoForwardCursor();
            }
            else if (layer == LayerMask.NameToLayer("MoveRight"))
            {
                _mouseIconController.SetGoRightCursor();
            }
            else if (layer == LayerMask.NameToLayer("MoveLeft"))
            {
                _mouseIconController.SetGoLeftCursor();
            }
            else if (layer == LayerMask.NameToLayer("Pickup"))
            {
                _mouseIconController.SetGrabCursor();
            }
            else if (layer == LayerMask.NameToLayer("Inspect"))
            {
                _mouseIconController.SetInspectCursor();
            }
            else
            {
                _mouseIconController.SetDefaultCursor();
            }
        }
        else
        {
            _mouseIconController.SetDefaultCursor();
        }
    }
}
