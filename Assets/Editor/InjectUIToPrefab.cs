using UnityEngine;
using UnityEditor;
using TMPro;

public class InjectUIToPrefab : EditorWindow
{
    [MenuItem("Tools/Luminang/Inject Time UI into HUD")]
    public static void InjectUI()
    {
        string prefabPath = "Assets/Prefabs/HUD/HUDGroups.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Could not find HUDGroups prefab!");
            return;
        }

        GameObject inst = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        if (inst.transform.Find("TimeWeatherPanel") == null)
        {
            GameObject uiPanel = new GameObject("TimeWeatherPanel");
            uiPanel.transform.SetParent(inst.transform, false);
            RectTransform rt = uiPanel.AddComponent<RectTransform>();
            
            // Anchor to Top-Right
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            
            // Adjust position (moved further down to avoid overlapping other top-right items if any exist)
            rt.anchoredPosition = new Vector2(-20, -20);
            rt.sizeDelta = new Vector2(250, 100);

            // Time Text
            GameObject timeObj = new GameObject("TimeText");
            timeObj.transform.SetParent(uiPanel.transform, false);
            var timeText = timeObj.AddComponent<TextMeshProUGUI>();
            timeText.fontSize = 36;
            timeText.fontStyle = FontStyles.Bold;
            timeText.alignment = TextAlignmentOptions.TopRight;
            var trt = timeObj.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 0.5f);
            trt.anchorMax = new Vector2(1, 1);
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;

            var twUI = uiPanel.AddComponent<TimeWeatherUI>();
            twUI.timeText = timeText;

            PrefabUtility.ApplyPrefabInstance(inst, InteractionMode.UserAction);
            Debug.Log("<color=green>SUCCESS: Time UI successfully injected into HUDGroups prefab!</color>");
        }
        else
        {
            Debug.Log("TimeWeatherPanel already exists in HUD.");
        }

        DestroyImmediate(inst);
    }
}
