using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public class FixCalleCrisologo
{
    static FixCalleCrisologo()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void OnHierarchyChanged()
    {
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.name != "Calle_Crisologo") return;

        // Fix Terrain Collider spam
        Terrain[] terrains = Object.FindObjectsOfType<Terrain>();
        foreach (var t in terrains)
        {
            MeshCollider mc = t.GetComponent<MeshCollider>();
            if (mc != null)
            {
                Object.DestroyImmediate(mc);
                Debug.Log($"<color=green>[AutoFix] Removed incompatible MeshCollider from Terrain: {t.name}</color>");
            }
        }

        // Ensure TeachingOverlayPanel is under a Canvas
        TeachingOverlayPanel overlay = Object.FindObjectOfType<TeachingOverlayPanel>(true);
        if (overlay != null)
        {
            Canvas parentCanvas = overlay.GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                // Find DialogueUIController's Canvas
                DialogueUIController diagUI = Object.FindObjectOfType<DialogueUIController>(true);
                if (diagUI != null)
                {
                    Canvas targetCanvas = diagUI.GetComponentInParent<Canvas>();
                    if (targetCanvas != null)
                    {
                        overlay.transform.SetParent(targetCanvas.transform, false);
                        Debug.Log($"<color=green>[AutoFix] Moved TeachingOverlayPanel to Canvas: {targetCanvas.name}</color>");
                    }
                }
            }
        }

        // Ensure PopupPanel is under a Canvas
        PopupManager popup = Object.FindObjectOfType<PopupManager>(true);
        if (popup != null && popup.popupPanel != null)
        {
            Canvas popupCanvas = popup.popupPanel.GetComponentInParent<Canvas>(true);
            if (popupCanvas == null)
            {
                 popup.popupPanel.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                 popup.popupPanel.AddComponent<CanvasScaler>();
                 popup.popupPanel.AddComponent<GraphicRaycaster>();
                 Debug.Log($"<color=green>[AutoFix] Added Canvas to PopupPanel.</color>");
            }
        }
    }
}
