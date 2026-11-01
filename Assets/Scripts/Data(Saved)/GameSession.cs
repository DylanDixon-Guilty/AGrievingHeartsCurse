using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores game progression data that persist between scenes
/// </summary>
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [Header("Information To Save")]
    public string ReturnNode { get; private set; } //What Node the player came from when going to Ethan'sHouse
    public List<string> ReturnNodeHistory { get; private set; } = new List<string>(); //Stores the Node history before the player went to Ethan'sHouse
    public string ReturnScene { get; private set; } //What Scene the player came from when going to Ethan'sHouse
    private Dictionary<string, bool> flags = new Dictionary<string, bool>(); //What Flag needs to be saved (such as unlocking Ethan'sHouse)
    private HashSet<string> collectedLootableIDs = new HashSet<string>(); //Storing items ID to know what items to restore/delete
    private ItemsData[] savedInventory = new ItemsData[12]; //Keeping track what the player has in their inventory before moving onto Ethan'sHouse

    private bool hasSavedInventory;

    private void Awake()
    {
        // Keep only one GameSession alive.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Returns the value of a progression flag. Flags that have not been set return false
    /// </summary>
    public bool GetFlag(string _flagName)
    {
        if (flags.TryGetValue(_flagName, out bool _value))
        {
            return _value;
        }

        return false;
    }

    /// <summary>
    /// Sets or updates a progression flag
    /// </summary>
    public void SetFlag(string _flagName, bool _value)
    {
        if (string.IsNullOrWhiteSpace(_flagName))
        {
            Debug.LogWarning("GameSession: Cannot set a flag with an empty name.");
            return;
        }

        flags[_flagName] = _value;
        Debug.Log($"{_flagName} had been set to {_value}");
    }

    /// <summary>
    /// Returns the player to the previous scene once clicking on the Book (in Ethan'sHouse)
    /// </summary>
    /// <summary>
    /// Saves the complete scrapbook navigation state.
    /// </summary>
    public void SetNavigationState(string _sceneName, string _nodeName, List<string> _nodeHistory)
    {
        if (string.IsNullOrWhiteSpace(_sceneName))
        {
            Debug.LogWarning("GameSession: Cannot save an empty scrapbook scene name.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_nodeName))
        {
            Debug.LogWarning("GameSession: Cannot save an empty scrapbook node name.");
            return;
        }

        ReturnScene = _sceneName;
        ReturnNode = _nodeName;
        ReturnNodeHistory = new List<string>(_nodeHistory);
    }

    public void ClearAllData()
    {
        flags.Clear();
        ReturnNodeHistory.Clear();
        collectedLootableIDs.Clear();
        ReturnScene = null;
        ReturnNode = null;
    }

    /// <summary>
    /// Removes data from the current session
    /// </summary>
    public void ClearSessionData()
    {
        ReturnNodeHistory.Clear();
        ReturnScene = null;
        ReturnNode = null;
    }

    /// <summary>
    /// Records a lootable item as collected
    /// </summary>
    public void MarkLootableCollected(string _lootableID)
    {
        if (string.IsNullOrWhiteSpace(_lootableID))
        {
            Debug.LogWarning("GameSession: Cannot record a lootable with an empty ID.");
            return;
        }

        collectedLootableIDs.Add(_lootableID);
    }

    /// <summary>
    /// Checks whether a lootable item has already been collected
    /// </summary>
    public bool IsLootableCollected(string _lootableID)
    {
        if (string.IsNullOrWhiteSpace(_lootableID))
        {
            return false;
        }

        return collectedLootableIDs.Contains(_lootableID);
    }

    /// <summary>
    /// Saves the current inventory contents
    /// </summary>
    public void SaveInventory(ItemsData[] _items)
    {
        if (_items == null)
        {
            Debug.LogWarning("GameSession: Cannot save a null inventory.");
            return;
        }

        savedInventory = new ItemsData[_items.Length];
        System.Array.Copy(_items, savedInventory, _items.Length);
        hasSavedInventory = true;
    }

    /// <summary>
    /// Returns the saved inventory contents
    /// </summary>
    public ItemsData[] LoadInventory()
    {
        return savedInventory != null ? (ItemsData[])savedInventory.Clone() : new ItemsData[12];
    }

    /// <summary>
    /// Indicates whether the inventory has previously been saved
    /// </summary>
    public bool HasSavedInventory()
    {
        return hasSavedInventory;
    }
}