using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The node the camera will spawn to based on where the player clicked
/// </summary>
public class CameraHotSpot : MonoBehaviour
{
    [SerializeField] private Transform _node;

    public Transform Node => _node;
}
