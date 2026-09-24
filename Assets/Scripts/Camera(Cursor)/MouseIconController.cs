using Unity.VisualScripting;
using UnityEngine;



/// <summary>
/// Set the correct cursor based on where it is located on the screen.
/// </summary>
public class MouseIconController : MonoBehaviour
{
    [SerializeField] private Texture2D defaultIcon;
    [SerializeField] private Texture2D grabIcon;
    [SerializeField] private Texture2D inspectIcon;
    [SerializeField] private Texture2D goForwardIcon;
    [SerializeField] private Texture2D goRightIcon;
    [SerializeField] private Texture2D goLeftIcon;

    private Vector2 cursorHotspot = new Vector2(25, 0);
    private bool isHoldingItem;
    private Sprite heldItemSprite; // The sprite used when holding an item in the hotbar (reference: ItemsData)

    /// <summary>
    /// Sets the cursor type when hovering over or clicking on an item.
    /// </summary>
    public void SetCursor(MouseIconType cursorType)
    {
        if (isHoldingItem) //Check to see if the player is holding an item.
        {
            return;
        }

        Texture2D cursorIcon = cursorType switch
        {
            MouseIconType.Default => defaultIcon,
            MouseIconType.Grab => grabIcon,
            MouseIconType.Inspect => inspectIcon,
            MouseIconType.GoForward => goForwardIcon,
            MouseIconType.GoRight => goRightIcon,
            MouseIconType.GoLeft => goLeftIcon,
            _ => defaultIcon
        };

        Cursor.SetCursor(cursorIcon, cursorHotspot, CursorMode.ForceSoftware);
    }

    /// <summary>
    /// When the player is no longer holding an item
    /// </summary>
    public void ClearHeldItem()
    {
        isHoldingItem = false;
        SetCursor(MouseIconType.Default);
    }

    public void SetHeldItemCursor(Sprite _itemSprite)
    {
        isHoldingItem = true;
        Cursor.SetCursor(_itemSprite.texture, cursorHotspot, CursorMode.ForceSoftware);
    }
}
