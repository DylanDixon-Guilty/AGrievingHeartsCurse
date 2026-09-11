using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Where the player can interact with items they have
/// </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] private Image backPack;
    [SerializeField] private GameObject hotBar;
    [SerializeField] private GameObject hotBarContainer01;
    [SerializeField] private GameObject hotBarContainer02;
    [SerializeField] private Sprite closedPack;
    [SerializeField] private Sprite openedPack;
    [SerializeField] private Image[] hotbarSlots;

    private bool isBackPackOpen;
    private ItemsData[] _items = new ItemsData[12];

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

    /// <summary>
    /// By clicking on the arrows on the hotbar, it will swap to the next set of inventory slots.
    /// </summary>
    public void SwitchInventory()
    {
        if (hotBarContainer01.activeSelf)
        {
            hotBarContainer01.SetActive(false);
            hotBarContainer02.SetActive(true);
        }
        else
        {
            hotBarContainer01.SetActive(true);
            hotBarContainer02.SetActive(false);
        }
    }

    public void AddItem(ItemsData item)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = item;

                hotbarSlots[i].sprite = item.ItemSprite;

                Debug.Log($"Added {item.ItemName} to inventory slot {i + 1}.");

                return;
            }
        }
    }
}
