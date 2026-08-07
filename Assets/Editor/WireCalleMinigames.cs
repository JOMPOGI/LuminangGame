using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// One-click tool that fully ports Magellan's Cross minigames into Calle Crisologo:
/// 1. Creates AfterMinigame dialogue assets for Lito (Word Rush) and Klara (Matching Game)
/// 2. Injects WordRush_Managers + MatchingGame_Managers + KalawQuizBubble prefabs into the scene
/// 3. Wires Lito's dialogueEvents: "StartMinigame:WordRush" -> StartMinigame(WordRushGame prefab)
/// 4. Wires Klara's dialogueEvents: "StartMinigame:Matching" -> StartMinigame(MatchingGamePanel prefab)
/// 5. Adds "Play Word Rush!" choice to Lito_Intro and "Play Matching Game!" to Klara_Intro
/// 6. Wires Kalaw's dialogueEvents: "StartTiptipQuiz:A" -> MinigameManager.StartMinigame(KalawQuizBubble)
/// </summary>
public class WireCalleMinigames : EditorWindow
{
    [MenuItem("Tools/Wire Calle Crisologo Minigames")]
    public static void DoWork()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Calle_Crisologo"))
        {
            Debug.LogError("[WireCalleMinigames] Please open the Calle_Crisologo scene first, then run this tool again!");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();

        // ─── Step 1: Load Prefabs ────────────────────────────────────────────────
        GameObject wordRushPrefab        = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Mini Games/Rush Game/WordRushGame.prefab");
        GameObject wordRushManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Mini Games/Rush Game/WordRush_Managers.prefab");
        GameObject matchingPanelPrefab   = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Mini Games/Matching Game/MatchingGamePanel.prefab");
        GameObject matchingMgrPrefab     = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Mini Games/Matching Game/MatchingGameManagers.prefab");
        GameObject quizBubblePrefab      = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Mini Games/TiptipQuizBubble 1.prefab");

        if (wordRushPrefab == null)       Debug.LogWarning("[WireCalleMinigames] WordRushGame.prefab not found!");
        if (matchingPanelPrefab == null)  Debug.LogWarning("[WireCalleMinigames] MatchingGamePanel.prefab not found!");
        if (quizBubblePrefab == null)     Debug.LogWarning("[WireCalleMinigames] TiptipQuizBubble 1.prefab not found!");

        // ─── Step 2: Inject Manager prefabs into scene (once) ────────────────────
        bool wordRushMgrExists = false, matchingMgrExists = false, kalawQuizExists = false;
        GameObject kalawQuizInstance = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == "WordRush_Managers")  wordRushMgrExists = true;
            if (root.name == "MatchingGame_Managers") matchingMgrExists = true;
            if (root.name == "KalawQuizBubble") { kalawQuizExists = true; kalawQuizInstance = root; }
        }

        if (!wordRushMgrExists && wordRushManagerPrefab != null)
        {
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(wordRushManagerPrefab, scene);
            inst.name = "WordRush_Managers";
            EditorUtility.SetDirty(inst);
            Debug.Log("<color=cyan>[Step 2] WordRush_Managers injected.</color>");
        }

        if (!matchingMgrExists && matchingMgrPrefab != null)
        {
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(matchingMgrPrefab, scene);
            inst.name = "MatchingGame_Managers";
            EditorUtility.SetDirty(inst);
            Debug.Log("<color=cyan>[Step 2] MatchingGame_Managers injected.</color>");
        }

        if (!kalawQuizExists && quizBubblePrefab != null)
        {
            kalawQuizInstance = (GameObject)PrefabUtility.InstantiatePrefab(quizBubblePrefab, scene);
            kalawQuizInstance.name = "KalawQuizBubble";
            kalawQuizInstance.SetActive(false);
            // Swap TiptipInlineQuiz → KalawInlineQuiz
            TiptipInlineQuiz oldScript = kalawQuizInstance.GetComponent<TiptipInlineQuiz>();
            if (oldScript != null) DestroyImmediate(oldScript);
            kalawQuizInstance.AddComponent<KalawInlineQuiz>();
            EditorUtility.SetDirty(kalawQuizInstance);
            Debug.Log("<color=cyan>[Step 2] KalawQuizBubble injected with KalawInlineQuiz component.</color>");
        }

        // ─── Step 3: Create Dialogue Assets ─────────────────────────────────────
        string litoDir  = "Assets/Dialogues/CalleCrisologo/Level2_FunctionalNavigational/Quest5_Requests/Lito";
        string klaraDir = "Assets/Dialogues/CalleCrisologo/Level2_FunctionalNavigational/Quest6_Directions/Klara";

        DialogueNode litoAfter0  = GetOrCreateDialogue(litoDir + "/Lito_AfterMinigame.asset",   "Lito",  "Napintas unay! Nagtawid ka! (Excellent! You passed the Word Rush!)",    "You passed the Word Rush!",    "",  "");
        DialogueNode litoAfter1  = GetOrCreateDialogue(litoDir + "/Lito_AfterMinigame_1.asset",  "Lito",  "Agpayso a mangisalawsalawsaw ka ti Ilocano. Nagtawid ka metten! (You are truly learning Ilocano. You've passed!)", "You've passed!", "", "CompleteObjective:WordRush_Lito");
        DialogueNode klaraAfter0 = GetOrCreateDialogue(klaraDir + "/Klara_AfterMinigame.asset",  "Klara", "Awan sabali! Inaramid mo! (Perfect! You did it!)",                        "You matched all direction words!", "", "");
        DialogueNode klaraAfter1 = GetOrCreateDialogue(klaraDir + "/Klara_AfterMinigame_1.asset","Klara", "Ammom na ti direksyon ti Ilocano! (You know Ilocano directions!)",         "You know Ilocano directions!", "", "CompleteObjective:MatchingGame_Klara");

        // Chain: litoAfter0 → litoAfter1
        if (litoAfter0 != null && litoAfter1 != null && litoAfter0.choices.Count == 0)
        {
            litoAfter0.choices.Add(new DialogueChoice { choiceText = "Great!", nextNode = litoAfter1 });
            EditorUtility.SetDirty(litoAfter0);
        }
        if (klaraAfter0 != null && klaraAfter1 != null && klaraAfter0.choices.Count == 0)
        {
            klaraAfter0.choices.Add(new DialogueChoice { choiceText = "Salamat!", nextNode = klaraAfter1 });
            EditorUtility.SetDirty(klaraAfter0);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=cyan>[Step 3] AfterMinigame dialogue assets created.</color>");

        // ─── Step 4: Find NPCs in scene ─────────────────────────────────────────
        InteractableNPC[] allNPCs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        InteractableNPC lito = null, klara = null, kalaw = null;

        foreach (var npc in allNPCs)
        {
            string n = npc.gameObject.name.ToLower();
            if (n.Contains("lito") && lito == null)   lito = npc;
            if (n.Contains("klara") && klara == null)  klara = npc;
            if (n.Contains("kalaw") && kalaw == null)  kalaw = npc;
        }

        // ─── Step 5: Wire Lito → Word Rush ─────────────────────────────────────
        if (lito != null && wordRushPrefab != null)
        {
            lito.minigameCategory   = "Requests";
            lito.minigameLanguageId = 1;

            // Add dialogueEvent mapping for "StartMinigame:WordRush"
            AddMinigameEventMapping(lito, "StartMinigame:WordRush", wordRushPrefab);

            // Add "Play Word Rush!" choice to Lito_Intro
            string litoIntroPath = litoDir + "/Lito_Intro.asset";
            DialogueNode litoIntro = AssetDatabase.LoadAssetAtPath<DialogueNode>(litoIntroPath);
            if (litoIntro != null && litoAfter0 != null)
            {
                if (!HasMinigameChoice(litoIntro))
                {
                    litoIntro.choices.Add(new DialogueChoice
                    {
                        choiceText  = "Play Word Rush!",
                        nextNode    = litoAfter0,
                        choiceEvent = "StartMinigame:WordRush",
                        isWrong     = false
                    });
                    EditorUtility.SetDirty(litoIntro);
                    Debug.Log("<color=cyan>[Step 5] Added 'Play Word Rush' choice to Lito_Intro.</color>");
                }
            }
            else Debug.LogWarning($"[Step 5] Lito_Intro not found at {litoIntroPath}!");

            EditorUtility.SetDirty(lito);
            Debug.Log("<color=cyan>[Step 5] Lito wired for Word Rush.</color>");
        }
        else if (lito == null) Debug.LogWarning("[Step 5] Lito NPC not found in scene — wire manually.");

        // ─── Step 6: Wire Klara → Matching Game ────────────────────────────────
        if (klara != null && matchingPanelPrefab != null)
        {
            klara.minigameCategory   = "Directions";
            klara.minigameLanguageId = 1;

            AddMinigameEventMapping(klara, "StartMinigame:Matching", matchingPanelPrefab);

            string klaraIntroPath = klaraDir + "/Klara_Intro.asset";
            DialogueNode klaraIntro = AssetDatabase.LoadAssetAtPath<DialogueNode>(klaraIntroPath);
            if (klaraIntro != null && klaraAfter0 != null)
            {
                if (!HasMinigameChoice(klaraIntro))
                {
                    klaraIntro.choices.Add(new DialogueChoice
                    {
                        choiceText  = "Play Matching Game!",
                        nextNode    = klaraAfter0,
                        choiceEvent = "StartMinigame:Matching",
                        isWrong     = false
                    });
                    EditorUtility.SetDirty(klaraIntro);
                    Debug.Log("<color=cyan>[Step 6] Added 'Play Matching Game' choice to Klara_Intro.</color>");
                }
            }
            else Debug.LogWarning($"[Step 6] Klara_Intro not found at {klaraIntroPath}!");

            EditorUtility.SetDirty(klara);
            Debug.Log("<color=cyan>[Step 6] Klara wired for Matching Game.</color>");
        }
        else if (klara == null) Debug.LogWarning("[Step 6] Klara NPC not found in scene — wire manually.");

        // ─── Step 7: Wire Kalaw → Inline Quiz ──────────────────────────────────
        if (kalaw != null && kalawQuizInstance != null)
        {
            // We pass the KalawQuizBubble PREFAB to MinigameManager — not the scene instance.
            // The scene instance is disabled; MinigameManager will Instantiate from the prefab.
            // So wire the quizBubblePrefab (already has KalawInlineQuiz added in memory).
            AddMinigameEventMapping(kalaw, "StartTiptipQuiz:A", quizBubblePrefab);
            AddMinigameEventMapping(kalaw, "StartTiptipQuiz:B", quizBubblePrefab);
            AddMinigameEventMapping(kalaw, "StartTiptipQuiz:C", quizBubblePrefab);

            // Add a quiz trigger to Kalaw_Intro_Yes (the "Yes, I want to learn" branch)
            string kalawYesPath = "Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest1_Greetings/Kalaw/Kalaw_Intro_Yes.asset";
            DialogueNode kalawYes = AssetDatabase.LoadAssetAtPath<DialogueNode>(kalawYesPath);
            if (kalawYes != null)
            {
                if (!HasMinigameChoice(kalawYes))
                {
                    kalawYes.choices.Add(new DialogueChoice
                    {
                        choiceText  = "Take the quiz!",
                        nextNode    = null, // dialogue resumes after quiz auto-dismisses
                        choiceEvent = "StartTiptipQuiz:A",
                        isWrong     = false
                    });
                    EditorUtility.SetDirty(kalawYes);
                    Debug.Log("<color=cyan>[Step 7] Added 'Take the quiz!' choice to Kalaw_Intro_Yes.</color>");
                }
            }
            else Debug.LogWarning($"[Step 7] Kalaw_Intro_Yes not found at {kalawYesPath}!");

            EditorUtility.SetDirty(kalaw);
            Debug.Log("<color=cyan>[Step 7] Kalaw wired for Inline Quiz.</color>");
        }
        else if (kalaw == null) Debug.LogWarning("[Step 7] Kalaw NPC not found in scene — wire manually.");

        // ─── Save everything ────────────────────────────────────────────────────
        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=green>SUCCESS: All 3 minigames wired into Calle Crisologo!\n" +
                  "  Kalaw  → Inline Quiz (StartTiptipQuiz:A/B/C)\n" +
                  "  Lito   → Word Rush   (StartMinigame:WordRush)\n" +
                  "  Klara  → Matching    (StartMinigame:Matching)</color>");
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────

    private static DialogueNode GetOrCreateDialogue(string path, string speaker, string text, string translation, string animTrigger, string endEvent)
    {
        DialogueNode existing = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        if (existing != null) return existing;

        EnsureFolder(System.IO.Path.GetDirectoryName(path).Replace("\\", "/"));

        DialogueNode node = ScriptableObject.CreateInstance<DialogueNode>();
        node.speakerName     = speaker;
        node.dialogueText    = text;
        node.translatedText  = translation;
        node.animationTrigger = animTrigger;
        node.endEventName    = endEvent;
        AssetDatabase.CreateAsset(node, path);
        return node;
    }

    private static void EnsureFolder(string folderPath)
    {
        string[] parts = folderPath.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private static bool HasMinigameChoice(DialogueNode node)
    {
        foreach (var c in node.choices)
            if (c.choiceEvent != null && (c.choiceEvent.StartsWith("StartMinigame") || c.choiceEvent.StartsWith("StartTiptipQuiz")))
                return true;
        return false;
    }

    private static void AddMinigameEventMapping(InteractableNPC npc, string eventName, GameObject prefab)
    {
        if (prefab == null) return;

        // Check if already mapped
        foreach (var mapping in npc.dialogueEvents)
            if (mapping.eventName == eventName) return;

        var newMapping = new DialogueEventMapping
        {
            eventName = eventName,
            onEventTriggered = new UnityEngine.Events.UnityEvent()
        };

        // Wire: npc.StartMinigame(prefab)
        UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<GameObject>(
            newMapping.onEventTriggered,
            npc.StartMinigame,
            prefab
        );

        npc.dialogueEvents.Add(newMapping);
        Debug.Log($"<color=lime>[WireCalleMinigames] {npc.gameObject.name}: mapped '{eventName}' → StartMinigame({prefab.name})</color>");
    }
}
