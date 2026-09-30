using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles spawning in CloseUps of artwork for dialogues
/// </summary>
public class CutSceneOverlay : MonoBehaviour
{
    [Header("Artwork Prefabs")]
    [SerializeField] private GameObject[] artworkPrefabs;

    [SerializeField] private Camera _camera;
    [SerializeField] private Image skipFade;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private ConversationControl _conversationControl;

    private GameObject currentArtwork; //The current artwork being displayed
    private Coroutine fadeCoroutine;

    /// <summary>
    /// When called, show the corresponding artwork for dialogue
    /// </summary>
    public void ShowArtwork(string _artworkName)
    {
        if (_conversationControl.skipAll)
        {
            ClearArtwork();
            StartSkipFade();
            return;
        }

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

        ResetSkipFade();
    }

    private void StartSkipFade()
    {
        if (fadeCoroutine != null) return;

        fadeCoroutine = StartCoroutine(FadeToBlack());
    }

    /// <summary>
    /// When the player presses the skip button, show a fade in and out paper to cover background
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeToBlack()
    {
        if (skipFade == null) yield break;

        Color fadeColor = skipFade.color;
        fadeColor.a = 1f;
        skipFade.color = fadeColor;

        yield return new WaitForSeconds(fadeDuration);

        fadeColor.a = 0f;
        skipFade.color = fadeColor;
        fadeCoroutine = null;
        ClearArtwork();
    }

    private void ResetSkipFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (skipFade != null)
        {
            Color fadeColor = skipFade.color;
            fadeColor.a = 0f;
            skipFade.color = fadeColor;
        }
    }
}
