#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class STTIntegrationTool : EditorWindow
{
    [MenuItem("Luminang/Integrations/Integrate STT into Calle Crisologo")]
    public static void IntegrateSTT()
    {
        // 1. Check if the active scene is Calle_Crisologo
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "Calle_Crisologo")
        {
            EditorUtility.DisplayDialog("Error", "Please open the 'Calle_Crisologo' scene first before running this tool.", "OK");
            return;
        }

        // Find existing managers in the scene
        GameObject managersRoot = GameObject.Find("Managers");
        if (managersRoot == null)
        {
            managersRoot = GameObject.Find("[Managers]");
        }
        if (managersRoot == null)
        {
            managersRoot = new GameObject("Managers");
        }

        GameObject dialogueManagerGO = Object.FindFirstObjectByType<DialogueManager>()?.gameObject;
        if (dialogueManagerGO == null)
        {
            EditorUtility.DisplayDialog("Error", "DialogueManager not found in the active scene! Cannot attach STTDialogueAdapter.", "OK");
            return;
        }

        // 2. Open the STT_TestScene additively
        string testScenePath = "Assets/Scenes/STT_TestScene.unity";
        Scene testScene = EditorSceneManager.OpenScene(testScenePath, OpenSceneMode.Additive);
        if (!testScene.IsValid())
        {
            EditorUtility.DisplayDialog("Error", "Could not load STT_TestScene additively. Make sure it exists at Assets/Scenes/STT_TestScene.unity", "OK");
            return;
        }

        // Find STT_System and Canvas in testScene
        GameObject sttSystemSrc = null;
        GameObject canvasSrc = null;

        foreach (GameObject root in testScene.GetRootGameObjects())
        {
            if (root.name == "STT_System")
            {
                sttSystemSrc = root;
            }
            else if (root.name == "Canvas")
            {
                canvasSrc = root;
            }
        }

        if (sttSystemSrc == null || canvasSrc == null)
        {
            EditorSceneManager.UnloadSceneAsync(testScene);
            EditorUtility.DisplayDialog("Error", "Could not find 'STT_System' or 'Canvas' in the STT_TestScene!", "OK");
            return;
        }

        // 3. Clone STT_System into Calle_Crisologo
        GameObject sttSystemClone = Instantiate(sttSystemSrc);
        sttSystemClone.name = "[STT_System]";
        sttSystemClone.transform.SetParent(managersRoot.transform);
        SceneManager.MoveGameObjectToScene(sttSystemClone, activeScene);

        // 4. Clone Canvas (or its relevant children) into Calle_Crisologo
        // Let's create an STT UI Overlay Canvas
        GameObject sttCanvasClone = Instantiate(canvasSrc);
        sttCanvasClone.name = "STT_UI_Canvas";
        SceneManager.MoveGameObjectToScene(sttCanvasClone, activeScene);

        // Remove the RegionSelector from the Canvas as it's only for testing language selection
        Transform regionSelector = sttCanvasClone.transform.Find("RegionSelector");
        if (regionSelector != null)
        {
            DestroyImmediate(regionSelector.gameObject);
        }

        // Disable the root Panel so it doesn't block the screen immediately
        Transform panelTrans = sttCanvasClone.transform.Find("Panel");
        if (panelTrans != null)
        {
            panelTrans.gameObject.SetActive(false);
        }

        // 5. Connect UI references to the cloned STTGameController
        STTGameController controller = sttSystemClone.GetComponent<STTGameController>();
        if (controller != null)
        {
            // Find references inside sttCanvasClone
            Button micBtn = sttCanvasClone.transform.Find("Panel/Mic_Button")?.GetComponent<Button>();
            TextMeshProUGUI statusTxt = sttCanvasClone.transform.Find("Panel/Status_Text")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI accuracyTxt = sttCanvasClone.transform.Find("Panel/Accuracy_text")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI feedbackTxt = sttCanvasClone.transform.Find("Panel/Feedback_Text")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI transcriptTxt = sttCanvasClone.transform.Find("Panel/Transcipt_Text")?.GetComponent<TextMeshProUGUI>();
            GameObject listeningIndicator = sttCanvasClone.transform.Find("Panel/ListeningIndicator")?.gameObject;
            Button retryBtn = sttCanvasClone.transform.Find("Panel/Retry_Button")?.GetComponent<Button>();

            // Setup bindings using SerializedObject to bypass private field restrictions
            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.FindProperty("micButton").objectReferenceValue = micBtn;
            serializedController.FindProperty("statusText").objectReferenceValue = statusTxt;
            serializedController.FindProperty("accuracyText").objectReferenceValue = accuracyTxt;
            serializedController.FindProperty("feedbackText").objectReferenceValue = feedbackTxt;
            serializedController.FindProperty("transcriptText").objectReferenceValue = transcriptTxt;
            serializedController.FindProperty("listeningIndicator").objectReferenceValue = listeningIndicator;
            serializedController.FindProperty("retryButton").objectReferenceValue = retryBtn;
            serializedController.ApplyModifiedProperties();

            Debug.Log("[STT Setup] Successfully bound UI references to STTGameController.");
        }

        // 6. Set PhraseEvaluator RegionMode to Ilokano for Calle_Crisologo
        PhraseEvaluator evaluator = sttSystemClone.GetComponent<PhraseEvaluator>();
        if (evaluator != null)
        {
            SerializedObject serializedEvaluator = new SerializedObject(evaluator);
            // RegionMode: 0 = Ilokano, 1 = Cebuano, 2 = BossBattle
            serializedEvaluator.FindProperty("<CurrentRegion>k__BackingField").intValue = 0; 
            serializedEvaluator.ApplyModifiedProperties();
            Debug.Log("[STT Setup] Set PhraseEvaluator region to Ilokano.");
        }

        // 7. Attach STTDialogueAdapter to DialogueManager
        if (dialogueManagerGO.GetComponent<STTDialogueAdapter>() == null)
        {
            dialogueManagerGO.AddComponent<STTDialogueAdapter>();
            Debug.Log("[STT Setup] Attached STTDialogueAdapter to DialogueManager GameObject.");
        }

        // 8. Clean up and unload
        EditorSceneManager.CloseScene(testScene, true);

        EditorUtility.SetDirty(managersRoot);
        EditorUtility.SetDirty(sttCanvasClone);
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorUtility.DisplayDialog("Success", "STT Integration completed successfully!\n\n1. [STT_System] added under Managers\n2. STT_UI_Canvas added to hierarchy\n3. STTDialogueAdapter attached to DialogueManager\n\nDon't forget to save your scene (Ctrl + S).", "OK");
    }
}
#endif
