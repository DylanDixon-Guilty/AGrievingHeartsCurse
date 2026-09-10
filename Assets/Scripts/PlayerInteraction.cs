using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform defaultPosition;
    [SerializeField] private Transform currentNode;

    private Stack<Transform> _nodeHistory = new Stack<Transform>();

    private void Awake()
    {
        _camera.transform.position = defaultPosition.transform.position;
    }

    private void Update()
    {
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
}
