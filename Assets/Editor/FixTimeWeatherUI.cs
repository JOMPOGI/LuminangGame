using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class FixTimeWeatherUI : EditorWindow
{
    [MenuItem("Tools/Luminang/Fix Time Weather UI Layout")]
    public static void FixUI()
    {
        string prefabPath = "Assets/Prefabs/HUD/HUDGroups.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return;

        GameObject inst = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        Transform panel = inst.transform.Find("TimeWeatherPanel");
        if (panel != null)
        {
            // 1. Ignore Layout Group
            LayoutElement le = panel.GetComponent<LayoutElement>();
            if (le == null) le = panel.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = true;

            // 2. Fix Anchors so it goes to Top-Right
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-20, -20);
            rt.sizeDelta = new Vector2(400, 100);

            // 3. Fix Fonts
            TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"); // Fallback
            
            Transform timeTxt = panel.Find("TimeText");
            if (timeTxt != null)
            {
                var tmp = timeTxt.GetComponent<TextMeshProUGUI>();
                if (tmp.font == null && defaultFont != null) tmp.font = defaultFont;
                tmp.color = Color.black; // Make it visible against bright sky
                
                // Add an outline for readability
                tmp.fontSharedMaterial = tmp.font.material; // ensure it's not null
            }
            
            PrefabUtility.ApplyPrefabInstance(inst, InteractionMode.UserAction);
            Debug.Log("<color=green>SUCCESS: Time Weather Panel layout fixed!</color>");
        }

        DestroyImmediate(inst);
    }
}
