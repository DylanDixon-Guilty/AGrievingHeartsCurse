using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The node the camera will spawn to based on where the player clicked
/// </summary>
public class CameraHotSpot : MonoBehaviour
{
    [SerializeField] private Transform node;

    public Transform Node => node;

    public void SetNode(Transform _node)
    {
        node = _node;
    }
}
