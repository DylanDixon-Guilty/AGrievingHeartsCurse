using UnityEngine;

public class MouseIconController : MonoBehaviour
{
    [SerializeField] private Texture2D _defaultMouseIcon;

    private void Start()
    {
        Cursor.SetCursor(_defaultMouseIcon, Vector2.zero, CursorMode.Auto);
    }
}
