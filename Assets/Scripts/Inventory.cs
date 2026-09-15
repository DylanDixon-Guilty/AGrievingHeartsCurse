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
    [SerializeField] private Button[] hotbarSlots;
    [SerializeField] private MouseIconController _mouseIconController;
    [SerializeField] private Color selectedItemColor = Color.gray; //When an item is selected, the hot-bar will darken it out

    private bool isBackPackOpen;
    private ItemsData[] _items = new ItemsData[12];
    private ItemsData _selectedItem;

    private void Start()
    {
        hotBar.SetActive(false);

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            int slotIndex = i;
            hotbarSlots[i].onClick.AddListener(() => SelectItem(slotIndex));
        }
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

    /// <summary>
    /// Add the corresponding item to player's inventory
    /// </summary>
    public void AddItem(ItemsData item)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = item;

                hotbarSlots[i].image.sprite = item.ItemSprite;

                Debug.Log($"Added {item.ItemName} to inventory slot {i + 1}.");

                return;
            }
        }
    }

    /// <summary>
    /// Change the mouse icon based on the selected item
    /// </summary>
    public void SelectItem(int _slotIndex)
    {
        if (_items[_slotIndex] == null)
        {
            return;
        }

        if (_selectedItem == _items[_slotIndex])
        {
            _selectedItem = null;
            _mouseIconController.ClearHeldItem();

            Debug.Log("Item deselected.");

            return;
        }

        _selectedItem = _items[_slotIndex];
        _mouseIconController.SetHeldItemCursor(_selectedItem.HeldItemSprite);

        Debug.Log($"Selected {_selectedItem.ItemName}.");
    }
}
