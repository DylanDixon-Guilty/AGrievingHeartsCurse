using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueGameState : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    private void Awake()
    {
        Lua.RegisterFunction(nameof(CheckGameStateFlag), this, SymbolExtensions.GetMethodInfo(() => CheckGameStateFlag(string.Empty)));
    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction(nameof(CheckGameStateFlag));
    }

    public bool CheckGameStateFlag(string flagName)
    {
        return _gameState.GetFlag(flagName);
    }
}
