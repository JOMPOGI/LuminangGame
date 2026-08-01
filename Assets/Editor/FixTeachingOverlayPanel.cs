using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class FixTeachingOverlayPanel
{
    [MenuItem("Tools/Calle Crisologo/Fix Teaching Overlay Panel")]
    public static void FixTransform()
    {
        var panel = Object.FindFirstObjectByType<TeachingOverlayPanel>(FindObjectsInactive.Include);
        if (panel != null)
        {
            RectTransform rt = panel.GetComponent<RectTransform>();
            if (rt != null)
            {
                Undo.RecordObject(rt, "Fix TeachingOverlayPanel Transform");
                
                // 1. Fix Z position and scale which were hiding it behind the camera
                Vector3 pos = rt.localPosition;
                pos.z = 0;
                rt.localPosition = pos;
                rt.localScale = Vector3.one;

                // 2. Fix Anchors so it stretches to cover the ENTIRE SCREEN
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                
                // 3. Reset anchored position and size delta to perfectly fit the screen bounds
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;

                // 4. Move it up in the hierarchy so the Dialogue Panel renders ON TOP of it
                // (like how it works in Magellan's Cross)
                rt.SetSiblingIndex(0); 

                EditorUtility.SetDirty(panel.gameObject);
                EditorSceneManager.MarkSceneDirty(panel.gameObject.scene);

                Debug.Log($"<color=green>Fixed TeachingOverlayPanel Transform! It is now fully stretched to the screen!</color>");
                EditorUtility.DisplayDialog("Fixed Properly!", "The Teaching Overlay Panel has been fully reset!\n\nIts anchors and size were squished into a tiny box off-screen. It is now properly stretched to cover the entire screen.", "OK");
            }
        }
        else
        {
            Debug.LogError("TeachingOverlayPanel not found in the scene.");
        }
    }
}
