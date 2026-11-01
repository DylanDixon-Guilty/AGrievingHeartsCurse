using UnityEngine;

/// <summary>
/// When the player clicks on a lootable item, it transfers the item data to the inventory and removes itself from the scene
/// </summary>
public class Lootable : MonoBehaviour
{
    public ItemsData ItemsData => _itemsData;

    [SerializeField] private ItemsData _itemsData;

    [Header("World Item Tracking")]
    [Tooltip("REMEMBER to name the item after itself")]
    [SerializeField] private string lootableID;

    private void Start()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning($"Lootable '{gameObject.name}': GameSession was not found. " + "Cannot check whether this item has already been collected.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(lootableID))
        {
            Debug.LogWarning($"Lootable '{gameObject.name}' has no unique ID assigned.", this);
            return;
        }

        if (GameSession.Instance.IsLootableCollected(lootableID))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Picks up the item and records the object's ID
    /// </summary>
    public void PickUp(Inventory _inventory)
    {
        if (_inventory == null)
        {
            Debug.LogWarning($"Lootable '{gameObject.name}': Inventory was not assigned.", this);
            return;
        }

        if (_itemsData == null)
        {
            Debug.LogWarning($"Lootable '{gameObject.name}': ItemsData was not assigned.", this);
            return;
        }

        if (GameSession.Instance == null)
        {
            Debug.LogWarning("Lootable: GameSession was not found. Cannot save collection state.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(lootableID))
        {
            Debug.LogWarning($"Lootable '{gameObject.name}' has no unique ID assigned.", this);
            return;
        }

        if (GameSession.Instance.IsLootableCollected(lootableID))
        {
            Destroy(gameObject);
            return;
        }

        bool itemAdded = _inventory.AddItem(_itemsData); // Check to see if the inventory is full
        if (!itemAdded)
        {
            return;
        }

        // Record the collection so the object does not reappear on scene reload.
        GameSession.Instance.MarkLootableCollected(lootableID);

        Destroy(gameObject);
    }

}