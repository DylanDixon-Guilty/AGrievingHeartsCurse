using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueGameState : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    private void Awake()
    {
        Lua.RegisterFunction(nameof(CheckGameStateFlag), this, SymbolExtensions.GetMethodInfo(() => CheckGameStateFlag(string.Empty)));
        Lua.RegisterFunction(nameof(SetGameStateFlag), this, SymbolExtensions.GetMethodInfo(() => SetGameStateFlag(string.Empty, false)));
    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction(nameof(CheckGameStateFlag));
        Lua.UnregisterFunction(nameof(SetGameStateFlag));
    }

    public bool CheckGameStateFlag(string flagName)
    {
        return _gameState.GetFlag(flagName);
    }

    public void SetGameStateFlag(string flagName, bool value)
    {
        _gameState.SetFlag(flagName, value);
    }
}