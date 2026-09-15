using UnityEngine;



/// <summary>
/// Set the correct cursor based on where it is located on the screen.
/// </summary>
public class MouseIconController : MonoBehaviour
{
    [SerializeField] private Texture2D _defaultIcon;
    [SerializeField] private Texture2D _grabIcon;
    [SerializeField] private Texture2D _inspectIcon;
    [SerializeField] private Texture2D _goForwardIcon;
    [SerializeField] private Texture2D _goRightIcon;
    [SerializeField] private Texture2D _goLeftIcon;

    private Vector2 cursorHotspot = new Vector2(25, 0);
    private bool isHoldingItem;
    private Sprite heldItemSprite; // The sprite used when holding an item in the hotbar (reference: ItemsData)

    /// <summary>
    /// Sets the cursor type when hovering over or clicking on an item.
    /// </summary>
    public void SetCursor(MouseIconType cursorType)
    {
        Texture2D cursorIcon = cursorType switch
        {
            MouseIconType.Default => _defaultIcon,
            MouseIconType.Grab => _grabIcon,
            MouseIconType.Inspect => _inspectIcon,
            MouseIconType.GoForward => _goForwardIcon,
            MouseIconType.GoRight => _goRightIcon,
            MouseIconType.GoLeft => _goLeftIcon,
            _ => _defaultIcon
        };

        Cursor.SetCursor(cursorIcon, cursorHotspot, CursorMode.ForceSoftware);
    }

    public void SetHeldItem(ItemsData item)
    {
        isHoldingItem = true;
        heldItemSprite = item.HeldItemSprite;

        Cursor.SetCursor(heldItemSprite.texture, Vector2.zero, CursorMode.Auto);
    }
}
