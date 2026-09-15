using UnityEngine;

/// <summary>
/// Handles the name, sprite, and type a lootable item is
/// </summary>
[CreateAssetMenu(fileName = "ItemsData", menuName = "Scriptable Objects/ItemsData")]
public class ItemsData : ScriptableObject
{
    public string ItemName;
    public Sprite ItemSprite;
    [Header("The item the player will be holding when clicking on the item in hotbar.")]
    public Sprite HeldItemSprite;
}
