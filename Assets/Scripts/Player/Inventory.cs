using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the player's inventory, item selection, and item combinations
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private Image backPack;
    [SerializeField] private GameObject hotBar;
    [SerializeField] private GameObject hotBarContainer01;
    [SerializeField] private GameObject hotBarContainer02;
    [SerializeField] private Sprite closedPack;
    [SerializeField] private Sprite openedPack;
    [SerializeField] private Sprite emptySlotSprite;
    [SerializeField] private Button[] hotbarSlots;
    [SerializeField] private TMP_Text[] hotbarItemNames;

    [Header("Item Selection")]
    [SerializeField] private MouseIconController _mouseIconController;
    [SerializeField] private Color selectedItemColor = Color.gray;

    [Header("Item Combinations")]
    [SerializeField] private CombinationData[] _combinationRecipes;
    [SerializeField] private GameObject combineButtonPrefab;
    [SerializeField] private Vector3 combineButtonLocalPosition = new Vector3(0f, 60f, 0f);

    [SerializeField] private int alertDisplayTime = 3; // For displaying the alert message in EnterCombineMode

    private bool isBackPackOpen;
    private bool isCombineMode;

    private ItemsData[] _items = new ItemsData[12];
    private ItemsData _selectedItem;

    private GameObject combineButton;
    private List<int> combinationSlots = new List<int>();
    private int combineSlotIndex = -1;

    private void Start()
    {
        hotBar.SetActive(false);

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            int slotIndex = i;

            hotbarSlots[i].onClick.AddListener(() => SelectItem(slotIndex));

            EventTrigger trigger = hotbarSlots[i].gameObject.AddComponent<EventTrigger>();

            // Hover. Show item name
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => ShowItemName(slotIndex));
            trigger.triggers.Add(pointerEnter);

            // Stop showing item name when mouse leaves
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => HideItemName(slotIndex));
            trigger.triggers.Add(pointerExit);

            // Left/right click
            EventTrigger.Entry pointerClick = new EventTrigger.Entry();
            pointerClick.eventID = EventTriggerType.PointerClick;
            pointerClick.callback.AddListener((data) =>
            {
                PointerEventData pointerData = (PointerEventData)data;

                if (pointerData.button == PointerEventData.InputButton.Right)
                {
                    ShowCombineButton(slotIndex);
                }
                else if (pointerData.button == PointerEventData.InputButton.Left)
                {
                    HideCombineButton();
                }
            });

            trigger.triggers.Add(pointerClick);
        }

        RefreshInventoryUI();
    }

    /// <summary>
    /// Opens or closes the backpack
    /// </summary>
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

            HideCombineButton();
            ExitCombineMode();
        }
    }

    /// <summary>
    /// By clicking on the arrows on the hotbar, it will swap to the next set of inventory slots
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

        HideCombineButton();
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

                RefreshInventoryUI();

                Debug.Log($"Added {item.ItemName} to inventory slot {i + 1}.");

                return;
            }
        }

        Debug.LogWarning("Inventory is full. Could not add item.");
    }

    /// <summary>
    /// Change the mouse icon based on the selected item
    /// </summary>
    public void SelectItem(int _slotIndex)
    {
        if (_items[_slotIndex] == null)
        {
            Debug.LogWarning("Slot contains nothing or is missing a component");
            return;
        }

        // Combine Mode uses clicks to select combination ingredients
        if (isCombineMode)
        {
            SelectCombinationItem(_slotIndex);
            return;
        }

        HideCombineButton();

        if (_items[_slotIndex].isCutSceneItem)
        {
            DialogueManager.ShowAlert("I should talk to Zoe about our next step.", alertDisplayTime);
            return;
        }

        if (_selectedItem == _items[_slotIndex])
        {
            _selectedItem = null;
            hotbarSlots[_slotIndex].image.color = Color.white;
            _mouseIconController.ClearHeldItem();

            Debug.Log("Item deselected.");

            return;
        }

        if (_selectedItem != null)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == _selectedItem)
                {
                    hotbarSlots[i].image.color = Color.white;
                    break;
                }
            }
        }

        _selectedItem = _items[_slotIndex];
        hotbarSlots[_slotIndex].image.color = selectedItemColor;
        _mouseIconController.SetHeldItemCursor(_selectedItem.HeldItemSprite);

        Debug.Log($"Selected {_selectedItem.ItemName}.");
    }

    /// <summary>
    /// Shows the Combine button above the inventory slot that was right-clicked
    /// </summary>
    private void ShowCombineButton(int _slotIndex)
    {
        if (_items[_slotIndex] == null)
        {
            return;
        }

        if (isCombineMode)
        {
            return;
        }

        if (combineButton == null)
        {
            combineButton = Instantiate(combineButtonPrefab);
        }

        combineSlotIndex = _slotIndex;
        combineButton.transform.SetParent(hotbarSlots[_slotIndex].transform, false);
        combineButton.transform.localPosition = combineButtonLocalPosition;
        combineButton.SetActive(true);

        Button button = combineButton.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Combine Button prefab is missing a Button component.");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(EnterCombineMode);
    }

    /// <summary>
    /// Find the corresponding recipe for combining items based on the Item selected in the Item Slot
    /// </summary>
    private CombinationData FindCombinationRecipe(ItemsData _item)
    {
        foreach (CombinationData _recipe in _combinationRecipes)
        {
            if (_recipe == null || _recipe.requiredItems == null)
            {
                continue;
            }

            foreach (ItemsData requiredItem in _recipe.requiredItems)
            {
                if (requiredItem == _item)
                {
                    return _recipe;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// This checks what items the player already has in their inventory
    /// </summary>
    private List<ItemsData> GetMissingCombinationItems(CombinationData _recipe)
    {
        List<ItemsData> missingItems = new List<ItemsData>();

        foreach (ItemsData requiredItem in _recipe.requiredItems)
        {
            bool hasItem = false;

            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == requiredItem)
                {
                    hasItem = true;
                    break;
                }
            }

            if (!hasItem)
            {
                missingItems.Add(requiredItem);
            }
        }

        return missingItems;
    }

    /// <summary>
    /// Hides the Combine button
    /// </summary>
    private void HideCombineButton()
    {
        if (combineButton != null)
        {
            combineButton.SetActive(false);
        }
    }

    /// <summary>
    /// Enters Combine Mode
    /// </summary>
    private void EnterCombineMode()
    {
        HideCombineButton();

        if (combineSlotIndex < 0 || combineSlotIndex >= _items.Length)
        {
            Debug.LogWarning("Could not find the item selected for combining.");
            return;
        }

        ItemsData selectedCombinationItem = _items[combineSlotIndex];

        CombinationData recipe = FindCombinationRecipe(selectedCombinationItem);

        if (recipe == null)
        {
            Debug.Log("This item cannot be combined with anything.");
            return;
        }

        List<ItemsData> missingItems = GetMissingCombinationItems(recipe);

        if (missingItems.Count > 0)
        {
            string alertMessage = "I need ";

            for (int i = 0; i < missingItems.Count; i++)
            {
                alertMessage += missingItems[i].ItemName;

                if (i < missingItems.Count - 2)
                {
                    alertMessage += ", ";
                }
                else if (i == missingItems.Count - 2)
                {
                    alertMessage += " and ";
                }
            }

            alertMessage += " to complete this combination. \n Close Back Pack to cancel combination.";

            DialogueManager.ShowAlert(alertMessage, alertDisplayTime);

            if (missingItems.Count == recipe.requiredItems.Length - 1)
            {
                return;
            }
        }
        else
        {
            int otherItemsNeeded = recipe.requiredItems.Length - 1;

            string alertMessage = otherItemsNeeded == 1
                ? "Combine this item with 1 other item."
                : $"Combine this item with {otherItemsNeeded} other items. \n Close Back Pack to cancel combination.";

            DialogueManager.ShowAlert(alertMessage, alertDisplayTime);
        }

        if (_selectedItem != null)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == _selectedItem)
                {
                    hotbarSlots[i].image.color = Color.white;
                    break;
                }
            }

            _selectedItem = null;
            _mouseIconController.ClearHeldItem();
        }

        combinationSlots.Clear();

        // The item used to enter Combine Mode is the first selected item.
        combinationSlots.Add(combineSlotIndex);
        hotbarSlots[combineSlotIndex].image.color = selectedItemColor;

        isCombineMode = true;

        Debug.Log($"Added {selectedCombinationItem.ItemName} to the combination.");
        Debug.Log("Entered Combine Mode.");
    }

    /// <summary>
    /// Selects an inventory item as part of the current combination
    /// </summary>
    private void SelectCombinationItem(int _slotIndex)
    {
        if (_items[_slotIndex] == null)
        {
            return;
        }

        // Clicking the same item again removes it from the combination
        if (combinationSlots.Contains(_slotIndex))
        {
            combinationSlots.Remove(_slotIndex);
            hotbarSlots[_slotIndex].image.color = Color.white;

            Debug.Log($"Removed {_items[_slotIndex].ItemName} from combination.");

            return;
        }

        combinationSlots.Add(_slotIndex);
        hotbarSlots[_slotIndex].image.color = selectedItemColor;

        Debug.Log($"Added {_items[_slotIndex].ItemName} to combination.");

        CheckCombination();
    }

    /// <summary>
    /// Checks whether the currently selected combination items match a recipe
    /// </summary>
    private void CheckCombination()
    {
        bool recipeWasChecked = false;

        foreach (CombinationData _recipe in _combinationRecipes)
        {
            if (_recipe == null)
            {
                continue;
            }

            if (_recipe.requiredItems.Length != combinationSlots.Count)
            {
                continue;
            }

            recipeWasChecked = true;

            List<ItemsData> remainingRequiredItems =
                new List<ItemsData>(_recipe.requiredItems);

            bool validCombination = true;

            foreach (int _slotIndex in combinationSlots)
            {
                ItemsData selectedItem = _items[_slotIndex];

                int requiredIndex = remainingRequiredItems.IndexOf(selectedItem);

                if (requiredIndex == -1)
                {
                    validCombination = false;
                    break;
                }

                remainingRequiredItems.RemoveAt(requiredIndex);
            }

            if (validCombination && remainingRequiredItems.Count == 0)
            {
                CompleteCombination(_recipe);
                return;
            }
        }

        if (recipeWasChecked)
        {
            Debug.Log("The selected items do not form a combination.");
        }
    }

    /// <summary>
    /// Removes the required ingredients, compacts the inventory, and adds the result to the next available slot
    /// </summary>
    private void CompleteCombination(CombinationData _recipe)
    {
        if (_recipe.resultItem == null)
        {
            Debug.LogWarning("Combination has no result item assigned.");
            return;
        }

        // Remove the ingredients
        foreach (int _slotIndex in combinationSlots)
        {
            _items[_slotIndex] = null;
        }

        // Compact the inventory so there are no gaps
        CompactInventory();

        // Find the next available slot.
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = _recipe.resultItem;
                break;
            }
        }

        RefreshInventoryUI();

        Debug.Log($"Created {_recipe.resultItem.ItemName}.");

        ExitCombineMode();
    }

    /// <summary>
    /// Moves all inventory items toward the beginning of the inventory
    /// </summary>
    private void CompactInventory()
    {
        int nextAvailableSlot = 0;

        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] != null)
            {
                _items[nextAvailableSlot] = _items[i];

                if (nextAvailableSlot != i)
                {
                    _items[i] = null;
                }

                nextAvailableSlot++;
            }
        }
    }

    /// <summary>
    /// Updates the visual inventory slots to match the inventory data
    /// </summary>
    private void RefreshInventoryUI()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] != null)
            {
                hotbarSlots[i].image.sprite = _items[i].ItemSprite;
            }
            else
            {
                hotbarSlots[i].image.sprite = emptySlotSprite;
            }

            hotbarSlots[i].image.color = Color.white;
        }
    }

    /// <summary>
    /// Exits Combine Mode and clears combination selections
    /// </summary>
    private void ExitCombineMode()
    {
        foreach (int _slotIndex in combinationSlots)
        {
            if (_slotIndex >= 0 && _slotIndex < hotbarSlots.Length)
            {
                hotbarSlots[_slotIndex].image.color = Color.white;
            }
        }

        combinationSlots.Clear();
        isCombineMode = false;
    }

    /// <summary>
    /// When hovering over an item slot that has an item, display the name of it
    /// </summary>
    public void ShowItemName(int _slotIndex)
    {
        if (_items[_slotIndex] == null)
        {
            return;
        }

        hotbarItemNames[_slotIndex].text = _items[_slotIndex].ItemName;
        hotbarItemNames[_slotIndex].gameObject.SetActive(true);
    }

    /// <summary>
    /// After the player stops hovering over an item slot with an item, hide the name
    /// </summary>
    public void HideItemName(int _slotIndex)
    {
        hotbarItemNames[_slotIndex].gameObject.SetActive(false);
    }
}