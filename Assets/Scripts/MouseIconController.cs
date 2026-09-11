using UnityEngine;

public class MouseIconController : MonoBehaviour
{
    [SerializeField] private Texture2D _defaultIcon;
    [SerializeField] private Texture2D _grabIcon;
    [SerializeField] private Texture2D _InspectIcon;
    [SerializeField] private Texture2D _GoForwardIcon;
    [SerializeField] private Texture2D _GoRightIcon;
    [SerializeField] private Texture2D _GoLeftIcon;

    private void Start()
    {
        Cursor.SetCursor(_defaultIcon, new Vector2(25, 0), CursorMode.ForceSoftware);
    }

    public void SetDefaultCursor() 
    {
        Cursor.SetCursor(_defaultIcon, new Vector2(25, 0), CursorMode.ForceSoftware);
    }

    public void SetGrabCursor()
    {
        Cursor.SetCursor(_grabIcon, new Vector2(25, 0), CursorMode.ForceSoftware);
    }

    public void SetInspectCursor()
    {
        Cursor.SetCursor(_InspectIcon, new Vector2(25, 0), CursorMode.ForceSoftware);
    }

    public void SetGoForwardCursor()
    {
        Cursor.SetCursor(_GoForwardIcon, new Vector2(25, 0), CursorMode.ForceSoftware);
    }

    public void SetGoRightCursor()
    {
        Cursor.SetCursor(_GoRightIcon, new Vector2(25, 0), CursorMode.ForceSoftware);
    }

    public void SetGoLeftCursor()
    {
        Cursor.SetCursor(_GoLeftIcon, new Vector2(25, 0), CursorMode.ForceSoftware);
    }
}
