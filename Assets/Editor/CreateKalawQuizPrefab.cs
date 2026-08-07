using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateKalawQuizPrefab : EditorWindow
{
    [MenuItem("Tools/Create Kalaw Quiz Prefab")]
    public static void DoWork()
    {
        string tiptipPrefabPath = "Assets/Prefabs/Mini Games/TiptipQuizBubble 1.prefab";
        string kalawPrefabPath = "Assets/Prefabs/Mini Games/KalawQuizBubble.prefab";

        if (!AssetDatabase.CopyAsset(tiptipPrefabPath, kalawPrefabPath))
        {
            Debug.LogError("Failed to copy TiptipQuizBubble 1.prefab. Make sure it exists!");
            // Try fallback path
            tiptipPrefabPath = "Assets/Prefabs/Mini Games/TiptipQuizBubble.prefab";
            if (!AssetDatabase.CopyAsset(tiptipPrefabPath, kalawPrefabPath))
            {
                Debug.LogError("Fallback failed too.");
                return;
            }
        }

        GameObject kalawPrefab = PrefabUtility.LoadPrefabContents(kalawPrefabPath);
        if (kalawPrefab == null)
        {
            Debug.LogError("Failed to load KalawQuizBubble.prefab");
            return;
        }

        kalawPrefab.name = "KalawQuizBubble";

        // Remove Tiptip script
        var oldScript = kalawPrefab.GetComponent<TiptipInlineQuiz>();
        var newScript = kalawPrefab.AddComponent<KalawInlineQuiz>();

        if (oldScript != null)
        {
            newScript.questionText = oldScript.questionText;
            newScript.choiceButtons = oldScript.choiceButtons;
            newScript.kalawPortrait = oldScript.tiptipPortrait;
            newScript.canvasGroup = oldScript.canvasGroup;
            newScript.panelRect = oldScript.panelRect;
            newScript.slideOffsetX = oldScript.slideOffsetX;
            newScript.slideInDuration = oldScript.slideInDuration;
            newScript.autoDismissDelay = oldScript.autoDismissDelay;

            // Load Kalaw's sprite
            Sprite kalawSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/NPCs/KalawImage.png");
            if (kalawSprite != null && newScript.kalawPortrait != null)
            {
                newScript.kalawPortrait.sprite = kalawSprite;
            }

            DestroyImmediate(oldScript, true);
        }

        PrefabUtility.SaveAsPrefabAsset(kalawPrefab, kalawPrefabPath);
        PrefabUtility.UnloadPrefabContents(kalawPrefab);

        Debug.Log("Created KalawQuizBubble prefab!");

        // Update ConversationTestManager in Calle Crisologo
        var ctm = Object.FindFirstObjectByType<ConversationTestManager>(FindObjectsInactive.Include);
        if (ctm != null)
        {
            ctm.tiptipQuizPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(kalawPrefabPath);
            EditorUtility.SetDirty(ctm);
            Debug.Log("Updated ConversationTestManager to use KalawQuizBubble prefab!");
        }

        // Apply speaker portrait to generated bridge nodes and fix choices
        string[] newNodes = new string[] {
            "Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest1_Greetings/Kalaw/Kalaw_QuizDone.asset"
        };
        Sprite kalawPortraitSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/NPCs/KalawImage.png");
        
        foreach (var path in newNodes)
        {
            var node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
            if (node != null)
            {
                node.speakerPortrait = kalawPortraitSprite;
                
                // Add a continue choice to avoid player being stuck without a choice button
                if (node.choices == null) node.choices = new List<DialogueChoice>();
                if (node.choices.Count == 0)
                {
                    node.choices.Add(new DialogueChoice {
                        choiceText = "Got it! (Go to Kyros)",
                        isWrong = false,
                        nextNode = null
                    });
                }
                
                EditorUtility.SetDirty(node);
                Debug.Log($"Applied Kalaw portrait and fixed choices in {path}");
            }
        }
        
        // Let's also check Kalaw_Intro and make sure the portrait is assigned.
        var kalawIntro = AssetDatabase.LoadAssetAtPath<DialogueNode>("Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest1_Greetings/Kalaw/Kalaw_Intro.asset");
        if (kalawIntro != null && kalawIntro.speakerPortrait == null)
        {
             kalawIntro.speakerPortrait = kalawPortraitSprite;
             EditorUtility.SetDirty(kalawIntro);
        }

        AssetDatabase.SaveAssets();
    }
}
