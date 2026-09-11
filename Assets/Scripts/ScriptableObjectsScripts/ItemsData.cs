using UnityEngine;

/// <summary>
/// Handles the name, sprite, and type a lootable item is
/// </summary>
[CreateAssetMenu(fileName = "ItemsData", menuName = "Scriptable Objects/ItemsData")]
public class ItemsData : ScriptableObject
{
    public string ItemName;
    public Sprite ItemSprite;
}
