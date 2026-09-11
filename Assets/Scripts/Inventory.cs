using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Where the player can interact with items they have
/// </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] private Image backPack;
    [SerializeField] private Image hotbarSlot;
    [SerializeField] private GameObject hotBar;
    [SerializeField] private Sprite closedPack;
    [SerializeField] private Sprite openedPack;

    private bool isBackPackOpen;
    private List<ItemsData> _items = new List<ItemsData>();

    private void Start()
    {
        hotBar.SetActive(false);
    }

    public void OpenBackPack()
    {
        if (!isBackPackOpen)
        {
            backPack.sprite = openedPack;
            hotBar.SetActive(true);
            isBackPackOpen = true;
        }
        else
        {
            backPack.sprite = closedPack;
            hotBar.SetActive(false);
            isBackPackOpen = false;
        }
    }

    public void AddItem(ItemsData item)
    {
        _items.Add(item);
        hotbarSlot.sprite = item.ItemSprite;
    }
}
