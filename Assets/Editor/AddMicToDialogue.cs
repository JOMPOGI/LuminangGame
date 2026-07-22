using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Luminang.UI.Minigames;

#if UNITY_EDITOR
public class AddMicToDialogue : MonoBehaviour
{
    [MenuItem("Luminang/UI/Add Rush Mic to Dialogue")]
    public static void InjectMicButton()
    {
        // 1. Find the DialogueUIController
        DialogueUIController dialogueUI = Object.FindAnyObjectByType<DialogueUIController>();
        if (dialogueUI == null)
        {
            Debug.LogError("Could not find DialogueUIController in the scene.");
            return;
        }

        // 2. Find the WordRushManager prefab to extract the mic button
        string prefabPath = "Assets/Prefabs/Mini Games/Rush Game/WordRushManager.prefab";
        GameObject rushPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (rushPrefab == null)
        {
            // Try alternate path
            prefabPath = "Assets/Prefabs/Mini Games/Rush Game/WordRush_Managers.prefab";
            rushPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        if (rushPrefab == null)
        {
            Debug.LogError("Could not find WordRushManager prefab in the Rush Game folder.");
            return;
        }

        // Extract VoiceVisualizer component from prefab
        VoiceVisualizer rushVisualizer = rushPrefab.GetComponentInChildren<VoiceVisualizer>(true);
        if (rushVisualizer == null)
        {
            Debug.LogError("Could not find VoiceVisualizer inside WordRush prefab.");
            return;
        }

        // 3. Inject into Dialogue Panel
        Transform dialoguePanel = dialogueUI.dialoguePanel.transform;
        
        // Check if already injected
        VoiceVisualizer existing = dialoguePanel.GetComponentInChildren<VoiceVisualizer>(true);
        if (existing != null)
        {
            Debug.Log("Mic button already injected into Dialogue Panel.");
            return;
        }

        GameObject micInstance = PrefabUtility.InstantiatePrefab(rushVisualizer.gameObject, dialoguePanel) as GameObject;
        micInstance.name = "Dialogue_Mic_Visualizer";
        
        RectTransform rt = micInstance.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        rt.pivot = new Vector2(0.5f, 0);
        rt.anchoredPosition = new Vector2(0, 20); // Just above bottom edge
        
        // 4. Create Adapter script
        STTVoiceVisualizerAdapter adapter = micInstance.AddComponent<STTVoiceVisualizerAdapter>();
        adapter.visualizer = micInstance.GetComponent<VoiceVisualizer>();
        
        // Wire up the button
        Button btn = micInstance.GetComponentInChildren<Button>(true);
        if (btn != null)
        {
            // Clear old minigame listeners
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 0);
            
            // Add adapter listener
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, new UnityEngine.Events.UnityAction(adapter.OnMicClicked));
        }

        EditorUtility.SetDirty(dialogueUI.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("Successfully injected the Rush Game Mic Button into the STT Dialogue flow!");
    }
}
#endif
