using UnityEngine;

/// <summary>
/// Handles collecting real-world objects as references for Ethan's drawings (Inventory item)
/// </summary>
public class ReferencePickup : MonoBehaviour
{
    public string ItemName => drawingItem != null ? drawingItem.ItemName : string.Empty; // The name of the object we're picking up (Used in PlayerController)

    [Header("Reference Settings")]
    [SerializeField] private string referenceID;
    [SerializeField] private ItemsData drawingItem;

    private void Start()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning($"ReferencePickup on {gameObject.name}: GameSession was not found.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(referenceID))
        {
            Debug.LogWarning($"ReferencePickup on {gameObject.name} has no reference ID assigned.", this);
            return;
        }

        if (GameSession.Instance.IsReferenceCollected(referenceID))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Records this object as a collected reference item
    /// </summary>
    public void PickUpReference()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("ReferencePickup: GameSession was not found.");
            return;
        }

        if (string.IsNullOrWhiteSpace(referenceID) || drawingItem == null)
        {
            Debug.LogWarning($"ReferencePickup on {gameObject.name} is missing its reference ID or drawing item.");
            return;
        }

        if (GameSession.Instance.CollectReference(referenceID, drawingItem))
        {
            Debug.Log($"Collected reference: {gameObject.name}");
            Destroy(gameObject);
        }
    }
}