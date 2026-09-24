using NUnit.Framework.Interfaces;
using UnityEngine;

/// <summary>
/// When the player clicks on a lootable item, it will transfer that data and delete itself
/// </summary>
public class Lootable : MonoBehaviour
{
    public ItemsData ItemsData => _itemsData;

    [SerializeField] private ItemsData _itemsData;

    /// <summary>
    /// Picks up the item based on what the player grabbing onto, if they have the inventory space for it
    /// </summary>
    /// <param name="_inventory"></param>
    public void PickUp(Inventory _inventory)
    {
        _inventory.AddItem(_itemsData);
        Destroy(gameObject);
    }
}
