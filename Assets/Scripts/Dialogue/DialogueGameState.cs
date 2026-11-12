using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// Checks and sets certain booleans in dialogue or puzzles
/// Like if they have a certain item to finish a puzzle
/// </summary>
public class DialogueGameState : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    [SerializeField] private Inventory _inventory;

    private void Awake()
    {
        Lua.RegisterFunction(nameof(CheckGameStateFlag), this, SymbolExtensions.GetMethodInfo(() => CheckGameStateFlag(string.Empty)));
        Lua.RegisterFunction(nameof(SetGameStateFlag), this, SymbolExtensions.GetMethodInfo(() => SetGameStateFlag(string.Empty, false)));
        Lua.UnregisterFunction(nameof(CheckInventoryItem));
    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction(nameof(CheckGameStateFlag));
        Lua.UnregisterFunction(nameof(SetGameStateFlag));
    }

    public bool CheckGameStateFlag(string _flagName)
    {
        return _gameState.GetFlag(_flagName);
    }

    public void SetGameStateFlag(string _flagName, bool _value)
    {
        _gameState.SetFlag(_flagName, _value);
    }

    public bool CheckInventoryItem(string _itemName)
    {
        return _inventory != null && _inventory.HasItem(_itemName);
    }
}