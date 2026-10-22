using UnityEditor;
using UnityEngine;

/// <summary>
/// Help set where the Speech Bubbles will sit for a cutscene
/// </summary>
[CustomEditor(typeof(CutSceneShot))]
public class CutSceneShotEditor : Editor
{
    // Temporary reference only; it is not saved into the asset.
    private RectTransform bubbleToCapture;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Bubble Position Helper",EditorStyles.boldLabel
        );

        bubbleToCapture = (RectTransform)EditorGUILayout.ObjectField("Bubble to Capture", bubbleToCapture, typeof(RectTransform), true);

        if (GUILayout.Button("Capture Bubble Position"))
        {
            CapturePosition();
        }
    }

    private void CapturePosition()
    {
        CutSceneShot shot = (CutSceneShot)target;

        if (bubbleToCapture == null)
        {
            Debug.LogWarning("Assign a speech bubble RectTransform first.");
            return;
        }

        Canvas canvas = bubbleToCapture.GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogWarning("The selected bubble is not inside a Canvas.");
            return;
        }

        Canvas rootCanvas = canvas.rootCanvas;

        Vector3 worldPosition = bubbleToCapture.position;

        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera, worldPosition);

        Rect canvasRect = rootCanvas.pixelRect;

        if (canvasRect.width <= 0 || canvasRect.height <= 0)
        {
            Debug.LogWarning("Canvas has an invalid size.");
            return;
        }

        float normalizedX = (screenPosition.x - canvasRect.xMin) / canvasRect.width;
        float normalizedY = (screenPosition.y - canvasRect.yMin) / canvasRect.height;

        Undo.RecordObject(shot, "Capture Cutscene Bubble Position");

        shot.screenX = Mathf.Clamp01(normalizedX);
        shot.screenY = Mathf.Clamp01(normalizedY);

        EditorUtility.SetDirty(shot);
        AssetDatabase.SaveAssets();

        Debug.Log($"Captured position for '{shot.name}': " + $"X = {shot.screenX:F3}, Y = {shot.screenY:F3}");
    }
}
