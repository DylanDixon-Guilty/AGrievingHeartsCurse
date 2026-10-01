using UnityEngine;

/// <summary>
/// Defines the items required to create a new item.
/// </summary>
[CreateAssetMenu(fileName = "CombinationData", menuName = "Scriptable Objects/CombinationData")]
public class CombinationData : ScriptableObject
{
    [Header("Items Required")]
    public ItemsData[] requiredItems;

    [Header("Result")]
    public ItemsData resultItem;
}
