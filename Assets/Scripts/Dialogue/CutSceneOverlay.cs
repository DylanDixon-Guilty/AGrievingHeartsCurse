using UnityEngine;

/// <summary>
/// Handles spawning in CloseUps of artwork for dialogues
/// </summary>
public class CutSceneOverlay : MonoBehaviour
{
    [Header("Artwork Prefabs")]
    [SerializeField] private GameObject[] artworkPrefabs;

    [SerializeField] private Camera _camera;

    private GameObject currentArtwork; //The current artwork being displayed

    /// <summary>
    /// When called, show the corresponding artwork for dialogue
    /// </summary>
    public void ShowArtwork(string _artworkName)
    {
        foreach (GameObject _artworkPrefab in artworkPrefabs)
        {
            if (_artworkPrefab.name == _artworkName)
            {
                if (currentArtwork != null)
                {
                    Destroy(currentArtwork);
                }

                currentArtwork = Instantiate(_artworkPrefab, _camera.transform.position + _camera.transform.forward * 1f, _camera.transform.rotation);

                return;
            }
        }
        Debug.LogWarning($"Could not find artwork prefab: {_artworkName}. Check array or for misspelling"); //If no artwork was found or misspelled
    }

    public void ClearArtwork()
    {
        if (currentArtwork != null)
        {
            Destroy(currentArtwork);
            currentArtwork = null;
        }
    }
}
