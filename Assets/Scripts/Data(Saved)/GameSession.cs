using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores game progression data that persist between scenes
/// </summary>
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [Header("Information To Save")]
    public string ReturnNode { get; private set; } // What Node the player came from when going to Ethan'sHouse
    public List<string> ReturnNodeHistory { get; private set; } = new List<string>(); // Stores the Node history before the player went to Ethan'sHouse
    public string ReturnScene { get; private set; } // What Scene the player came from when going to Ethan'sHouse

    private Dictionary<string, bool> flags = new Dictionary<string, bool>(); // What Flag needs to be saved (such as unlocking Ethan'sHouse)
    private HashSet<string> collectedLootableIDs = new HashSet<string>(); // Storing items ID to know what items to restore/delete
    private HashSet<string> collectedReferenceIDs = new HashSet<string>(); // This stores the items collected in Ethan'sHouse
    private Dictionary<string, ItemsData> pendingDrawings = new Dictionary<string, ItemsData>();
    private ItemsData[] savedInventory = new ItemsData[12]; // Keeping track what the player has in their inventory before moving onto Ethan'sHouse

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
    /// Returns the value of a progression flag. Flags that have not been set (blank) return false
    /// </summary>
    public bool GetFlag(string _flagName)
    {
        if (string.IsNullOrWhiteSpace(_flagName))
        {
            Debug.LogWarning("GameSession: Cannot get a flag with an empty name.");
            return false;
        }

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

        Debug.Log($"GameSession: Flag '{_flagName}' set to {_value}.");

    }

    /// <summary>
    /// Returns a copy of all progression flags
    /// </summary>
    public Dictionary<string, bool> GetAllFlags()
    {
        return new Dictionary<string, bool>(flags);
    }

    /// <summary>
    /// Saves the complete scrapbook navigation state
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

    public void ClearAllData() // Clear the data of the current save (NOT YET IMPLEMENTED)
    {
        flags.Clear();
        ReturnNodeHistory.Clear();
        collectedLootableIDs.Clear();
        collectedReferenceIDs.Clear();
        pendingDrawings.Clear();
        savedInventory = null;
        ReturnScene = null;
        ReturnNode = null;
        hasSavedInventory = false;
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

    /// <summary>
    /// Checks whether a real-world reference has already been collected
    /// </summary>
    public bool IsReferenceCollected(string _referenceID)
    {
        if (string.IsNullOrWhiteSpace(_referenceID))
        {
            return false;
        }

        return collectedReferenceIDs.Contains(_referenceID);
    }

    /// <summary>
    /// Records a collected reference and the paper item it will produce
    /// </summary>
    public bool CollectReference(string _referenceID, ItemsData _drawingItem)
    {
        if (string.IsNullOrWhiteSpace(_referenceID))
        {
            Debug.LogWarning("GameSession: Cannot collect a reference with an empty ID.");
            return false;
        }

        if (_drawingItem == null)
        {
            Debug.LogWarning($"GameSession: Reference '{_referenceID}' has no drawing item assigned.");
            return false;
        }

        if (collectedReferenceIDs.Contains(_referenceID))
        {
            return false;
        }

        collectedReferenceIDs.Add(_referenceID);
        pendingDrawings.Add(_referenceID, _drawingItem);

        Debug.Log($"GameSession: Reference '{_referenceID}' collected. Drawing '{_drawingItem.ItemName}' is pending.");
        return true;
    }

    /// <summary>
    /// Returns a copy of all drawings waiting to be added to the inventory
    /// </summary>
    public Dictionary<string, ItemsData> GetPendingDrawings()
    {
        return new Dictionary<string, ItemsData>(pendingDrawings);
    }

    /// <summary>
    /// Removes a drawing from the pending list after it has been added to inventory
    /// </summary>
    public void MarkDrawingProcessed(string _referenceID)
    {
        if (string.IsNullOrWhiteSpace(_referenceID))
        {
            return;
        }

        pendingDrawings.Remove(_referenceID);
    }
}