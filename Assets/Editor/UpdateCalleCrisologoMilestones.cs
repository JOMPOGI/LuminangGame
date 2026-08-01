using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class UpdateCalleCrisologoMilestones
{
    [MenuItem("Tools/Calle Crisologo/Update Milestones and Levels")]
    public static void UpdateMilestones()
    {
        int updatedCount = 0;

        // 1. Kalaw Intro
        UpdateNodeChoiceEvent(
            "Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest1_Greetings/Kalaw/Kalaw_Intro.asset",
            "ShowPopup:welcome_level1",
            ref updatedCount
        );

        // 2. Kyros Success
        UpdateNodeTextAndSingleChoice(
            "Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest1_Greetings/Kyros/Kyros_W08_Success.asset",
            "Agpakada akon! Go meet Vendor Irah at the weaving loom. She has something important to teach you about showing gratitude.",
            "ShowPopup:greetings",
            null,
            ref updatedCount
        );

        // 3. Jom Gratitude Success
        UpdateNodeTextAndChoiceEvent(
            "Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest2_Gratitude/Jom/Jom_W13_Success.asset",
            "Dispensaren nak! Sharp manners! Stick around for the Response trials!",
            "ShowPopup:gratitude",
            ref updatedCount
        );

        // 4. Jom Responses Success
        UpdateNodeTextAndSingleChoice(
            "Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest2_Gratitude/Jom/Jom_W18_Success.asset",
            "Diak maawatan! No worries-learning takes practice!",
            "ShowPopup:responses",
            null,
            ref updatedCount
        );

        // 5. Sally Level I Complete - Set objective with next NPC (Sally)
        UpdateNodeTextAndSingleChoice(
            "Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest4_Identity/Sally/Sally_W22_Success.asset",
            "Excellent! You just told me where you're from!",
            "ShowPopup:identity,complete_level1,welcome_level2",
            null,
            ref updatedCount,
            clearTrigger: true,
            newEndEvent: "SetObjective_LEVEL I COMPLETE! Head to Level II: Talk to Sally"
        );

        // 6. Lito Requests Success
        UpdateNodeTextAndChoiceEvent(
            "Assets/Dialogues/CalleCrisologo/Level2_FunctionalNavigational/Quest5_Requests/Lito/Lito_W27_Success.asset",
            "Mabalin kadi agsaludsod! Ask away!",
            "ShowPopup:requests",
            ref updatedCount
        );

        // 7. Klara Directions Success
        UpdateNodeTextAndChoiceEvent(
            "Assets/Dialogues/CalleCrisologo/Level2_FunctionalNavigational/Quest6_Directions/Klara/Klara_W37_Success.asset",
            "Uray ditoy! Talk to me again for the Counting lessons!",
            "ShowPopup:directions",
            ref updatedCount
        );

        // 8. Mang Lance Level II Complete - Set objective with next NPC (Mang Lance)
        UpdateNodeTextAndSingleChoice(
            "Assets/Dialogues/CalleCrisologo/Level2_FunctionalNavigational/Quest7_Count/MangLance/MangLance_W47_Success.asset",
            "Sangapulo! You made it to ten!",
            "ShowPopup:counting,complete_level2,welcome_level3",
            null,
            ref updatedCount,
            clearTrigger: true,
            newEndEvent: "SetObjective_LEVEL II COMPLETE! Head to Level III: Talk to Mang Lance"
        );

        // 9. Aling Rosa Action Verbs Success
        UpdateNodeTextAndChoiceEvent(
            "Assets/Dialogues/CalleCrisologo/Level3_GrammaticalFoundations/Quest8_ActionVerbs/AlingRosa/AlingRosa_W55_Success.asset",
            "Agsao! Stay here for Linking Verbs!",
            "ShowPopup:actionVerbs",
            ref updatedCount
        );

        // 10. Aling Riza Linking Verbs Success
        UpdateNodeTextAndChoiceEvent(
            "Assets/Dialogues/CalleCrisologo/Level3_GrammaticalFoundations/Quest9_LinkingVerbs/AlingRiza/AlingRiza_W65_Success.asset",
            "Marikna! Talk to me again for Pronouns!",
            "ShowPopup:linkingVerbs",
            ref updatedCount
        );

        // 11. Aling Riza Pronouns Success
        UpdateNodeTextAndSingleChoice(
            "Assets/Dialogues/CalleCrisologo/Level3_GrammaticalFoundations/Quest9_LinkingVerbs/AlingRiza/AlingRiza_W74_Success.asset",
            "Isuda! You've learned how Ilokano can talk about different people!",
            "ShowPopup:pronouns",
            null,
            ref updatedCount
        );

        // 12. Lola Bebang Level III Success - Set objective with next NPC (Kalaw)
        UpdateNodeTextAndSingleChoice(
            "Assets/Dialogues/CalleCrisologo/Level3_GrammaticalFoundations/Quest11_Interrogatives/LolaBebang/LolaBebang_W81_Success.asset",
            "Mano! Now you can ask about quantity and numbers. You've learned how to communicate in many everyday situations throughout your journey. But learning doesn't end with repeating what you've heard. Head back to Kalaw at the plaza. Your final challenge awaits.",
            "ShowPopup:interrogatives,complete_level3,final",
            null,
            ref updatedCount,
            clearTrigger: false,
            newEndEvent: "SetObjective_LEVEL III COMPLETE! Head to Plaza: Talk to Kalaw"
        );

        AssetDatabase.SaveAssets();

        // Automatically wire the NPCs in the scene to their respective quest dialogues
        AutomateCalleSetup.RunSetup();

        Debug.Log($"[UpdateMilestones] Completed! Patched {updatedCount} assets and wired NPCs.");
        EditorUtility.DisplayDialog("Success", $"Patched {updatedCount} milestone/level success dialogue nodes with specific next NPC objectives, and wired the NPCs!", "OK");
    }

    private static void UpdateNodeChoiceEvent(string path, string choiceEvent, ref int updatedCount)
    {
        DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        if (node == null)
        {
            Debug.LogWarning($"[UpdateMilestones] Could not load DialogueNode at: {path}");
            return;
        }

        if (node.choices != null && node.choices.Count > 0)
        {
            node.choices[0].choiceEvent = choiceEvent;
            EditorUtility.SetDirty(node);
            updatedCount++;
        }
    }

    private static void UpdateNodeTextAndChoiceEvent(string path, string newText, string choiceEvent, ref int updatedCount)
    {
        DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        if (node == null)
        {
            Debug.LogWarning($"[UpdateMilestones] Could not load DialogueNode at: {path}");
            return;
        }

        node.dialogueText = newText;
        if (node.choices != null && node.choices.Count > 0)
        {
            node.choices[0].choiceText = "Continue";
            node.choices[0].choiceEvent = choiceEvent;
        }
        EditorUtility.SetDirty(node);
        updatedCount++;
    }

    private static void UpdateNodeTextAndSingleChoice(string path, string newText, string choiceEvent, DialogueNode nextNode, ref int updatedCount, bool clearTrigger = false, string newEndEvent = null)
    {
        DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        if (node == null)
        {
            Debug.LogWarning($"[UpdateMilestones] Could not load DialogueNode at: {path}");
            return;
        }

        node.dialogueText = newText;
        if (clearTrigger)
        {
            node.triggerEventName = "";
        }
        if (!string.IsNullOrEmpty(newEndEvent))
        {
            node.endEventName = newEndEvent;
        }

        if (node.choices == null)
        {
            node.choices = new List<DialogueChoice>();
        }
        else
        {
            node.choices.Clear();
        }

        DialogueChoice choice = new DialogueChoice
        {
            choiceText = "Continue",
            nextNode = nextNode,
            isWrong = false,
            choiceEvent = choiceEvent,
            expectedSTTWord = ""
        };
        node.choices.Add(choice);

        EditorUtility.SetDirty(node);
        updatedCount++;
    }
}
