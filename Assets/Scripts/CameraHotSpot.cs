using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHotSpot : MonoBehaviour
{
    [SerializeField] private Transform _node;

    public Transform Node => _node;
}
