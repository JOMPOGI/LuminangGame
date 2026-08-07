using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class WireCalleCrisologoMinigames : EditorWindow
{
    [MenuItem("Tools/Luminang/Wire Calle Crisologo Minigames")]
    public static void WireMinigames()
    {
        Debug.Log("Starting Minigame Wiring...");

        // 1. GREETINGS -> Word Rush
        WireLinear("Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest1_Greetings/Kyros/Kyros_W06_Success.asset", 
            "Kyros", "Let's review what you've learned!", "WordRush", null);

        // 2. GRATITUDE -> Word Rush
        WireLinear("Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest2_Gratitude/Jom/Jom_W13_Success.asset", 
            "Jom", "Let's review what you've learned!", "WordRush", null);

        // 3. RESPONSES -> Word Rush
        WireLinear("Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest3_Responses/Jom/Jom_W18_Success.asset", 
            "Jom", "Let's review what you've learned!", "WordRush", null);

        // 4. IDENTITY -> Word Rush -> Two Truths
        WireLinear("Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest4_Identity/Sally/Sally_W22_Success.asset", 
            "Sally", "Let's review what you've learned!", "WordRush", "TwoTruths");

        // 5. REQUESTS -> Word Rush
        WireLinear("Assets/Dialogues/CalleCrisologo/Level2_FunctionalNavigational/Quest5_Requests/Lito/Lito_W27_Success.asset", 
            "Lito", "Let's review what you've learned!", "WordRush", null);

        // 6. DIRECTIONS -> Word Rush -> Matching
        WireLinear("Assets/Dialogues/CalleCrisologo/Level2_FunctionalNavigational/Quest6_Directions/Klara/Klara_W37_Success.asset", 
            "Klara", "Let's review what you've learned!", "WordRush", "Matching");

        // 7. COUNT -> Word Rush
        WireLinear("Assets/Dialogues/CalleCrisologo/Level2_FunctionalNavigational/Quest7_Count/MangLance/MangLance_W47_Success.asset", 
            "Mang Lance", "Let's review what you've learned!", "WordRush", null);

        // 8. ACTION VERBS -> Word Rush
        WireLinear("Assets/Dialogues/CalleCrisologo/Level3_GrammaticalFoundations/Quest8_ActionVerbs/AlingRosa/AlingRosa_W56_Success.asset", 
            "Aling Rosa", "Let's review what you've learned!", "WordRush", null);

        // 9. LINKING VERBS -> Word Rush -> Two Truths
        WireLinear("Assets/Dialogues/CalleCrisologo/Level3_GrammaticalFoundations/Quest9_LinkingVerbs/AlingRiza/AlingRiza_W65_Success.asset", 
            "Aling Riza", "Let's review what you've learned!", "WordRush", "TwoTruths");

        // 10. PRONOUNS -> Word Rush -> Two Truths
        WireLinear("Assets/Dialogues/CalleCrisologo/Level3_GrammaticalFoundations/Quest10_Pronouns/AlingRiza/AlingRiza_W74_Success.asset", 
            "Aling Riza", "Let's review what you've learned!", "WordRush", "TwoTruths");

        // 11. INTERROGATIVES -> Word Rush
        WireLinear("Assets/Dialogues/CalleCrisologo/Level3_GrammaticalFoundations/Quest11_Interrogatives/LolaBebang/LolaBebang_W81_Success.asset", 
            "Lola Bebang", "Let's review what you've learned!", "WordRush", null);

        AssetDatabase.SaveAssets();
        Debug.Log("Minigame Wiring Complete!");
    }

    private static void WireLinear(string successNodePath, string npcName, string transitionText, string mini1, string mini2)
    {
        DialogueNode successNode = AssetDatabase.LoadAssetAtPath<DialogueNode>(successNodePath);
        if (successNode == null)
        {
            Debug.LogError($"Could not find node at {successNodePath}");
            return;
        }

        DialogueNode originalNext = null;
        if (successNode.choices.Count > 0)
        {
            originalNext = successNode.choices[0].nextNode;
            
            if (successNode.choices[0].choiceEvent != null && successNode.choices[0].choiceEvent.Contains("ShowPopup"))
            {
                // Has a popup. We need to create a gateway for mini1 to not override the popup!
                string folder = System.IO.Path.GetDirectoryName(successNodePath);
                string mini1GatewayPath = folder + "/" + npcName.Replace(" ", "") + "_MiniGateway1.asset";
                DialogueNode gw1 = AssetDatabase.LoadAssetAtPath<DialogueNode>(mini1GatewayPath);
                if (gw1 == null)
                {
                    gw1 = ScriptableObject.CreateInstance<DialogueNode>();
                    AssetDatabase.CreateAsset(gw1, mini1GatewayPath);
                }
                gw1.speakerName = npcName;
                gw1.dialogueText = transitionText;
                gw1.translatedText = "";
                gw1.choices = new List<DialogueChoice>
                {
                    new DialogueChoice { choiceText = $"Play {mini1}!", choiceEvent = $"StartMinigame:{mini1}", nextNode = originalNext, isWrong = false }
                };
                EditorUtility.SetDirty(gw1);

                successNode.choices[0].nextNode = gw1;
                EditorUtility.SetDirty(successNode);
                
                // Now gw1 is our new success node for chaining mini2!
                successNode = gw1;
            }
            else 
            {
                // No popup, we can safely overwrite choiceEvent
                successNode.choices[0].choiceText = $"Play {mini1}!";
                successNode.choices[0].choiceEvent = $"StartMinigame:{mini1}";
                EditorUtility.SetDirty(successNode);
            }
        }

        if (mini2 != null)
        {
            // We need a gateway node for minigame 2
            string folder = System.IO.Path.GetDirectoryName(successNodePath);
            string gatewayPath = folder + "/" + npcName.Replace(" ", "") + "_MiniGateway2.asset";
            DialogueNode gateway = AssetDatabase.LoadAssetAtPath<DialogueNode>(gatewayPath);
            if (gateway == null)
            {
                gateway = ScriptableObject.CreateInstance<DialogueNode>();
                AssetDatabase.CreateAsset(gateway, gatewayPath);
            }
            gateway.speakerName = npcName;
            gateway.dialogueText = "Excellent! Let's play one more to really test your memory!";
            gateway.translatedText = "";
            gateway.choices = new List<DialogueChoice>
            {
                new DialogueChoice
                {
                    choiceText = $"Play {mini2}!",
                    choiceEvent = $"StartMinigame:{mini2}",
                    nextNode = originalNext,
                    isWrong = false
                }
            };
            EditorUtility.SetDirty(gateway);

            // Now update success node to point to mini2 gateway
            successNode.choices[0].nextNode = gateway;
            EditorUtility.SetDirty(successNode);
        }
    }
}
