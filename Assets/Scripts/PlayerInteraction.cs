using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform defaultPosition;

    private void Awake()
    {
        _camera.transform.position = defaultPosition.transform.position;
    }

    private void Update()
    {
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

    private void MoveCamera(Transform _tranform)
    {
        _camera.transform.position = _tranform.position;
    }
}
