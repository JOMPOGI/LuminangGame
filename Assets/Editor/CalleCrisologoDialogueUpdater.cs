using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CalleCrisologoDialogueUpdater
{
    [MenuItem("Tools/Update Calle Crisologo Dialogues (No STT)")]
    public static void UpdateDialogues()
    {
        string outputFolder = "Assets/Dialogues/CalleCrisologo_New";
        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            string[] parts = outputFolder.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(current + "/" + parts[i]))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current += "/" + parts[i];
            }
        }
        
        int count = 0;
        // --- NPC: Kalaw ---
        {
        DialogueNode Kalaw_start = null;
        DialogueNode Kalaw_prev = null;
        DialogueNode Kalaw_0 = GetOrCreateNode(outputFolder + "/Kalaw_0.asset");
        Kalaw_0.speakerName = "KALAW";
        Kalaw_0.dialogueText = "\"Squawk! Oh... is that a fruit in your hand? May I have some? Flying under this hot Vigan sun has completely worn me out...\"\n\"Mmm, sweet and juicy!";
        EditorUtility.SetDirty(Kalaw_0);
        Kalaw_start = Kalaw_0;
        DialogueNode Kalaw_0_part1 = GetOrCreateNode(outputFolder + "/Kalaw_0_part1.asset");
        Kalaw_0_part1.speakerName = "KALAW";
        Kalaw_0_part1.dialogueText = "Thank you so much, traveler!\nAh, where are my manners-I am Kalaw, your companion and guide through these lands!\nWait a second... look at that ancient anting-anting pendant resting on your chest!";
        EditorUtility.SetDirty(Kalaw_0_part1);
        AddChoice(Kalaw_0, "Continue", Kalaw_0_part1);
        DialogueNode Kalaw_0_part2 = GetOrCreateNode(outputFolder + "/Kalaw_0_part2.asset");
        Kalaw_0_part2.speakerName = "KALAW";
        Kalaw_0_part2.dialogueText = "Feel that faint hum?\nThe Ilocos Language Crystal inside it is sleeping.\nTo charge it up and awaken its power, you've got to explore Calle Crisologo and speak with the locals in their native tongue.\nAs you practice and learn how Ilocanos talk in their daily lives, your voice will help restore the crystal's power.\nSo, let's begin with something simple.\nBefore you can truly connect with the people of Ilocos, you must first learn how to greet them.\"\n";
        EditorUtility.SetDirty(Kalaw_0_part2);
        AddChoice(Kalaw_0_part1, "Continue", Kalaw_0_part2);
        Kalaw_prev = Kalaw_0_part2;
        Kalaw_0_part2.endEventName = "SetObjective_Find Fruit Completed";
        DialogueNode Kalaw_1 = GetOrCreateNode(outputFolder + "/Kalaw_1.asset");
        Kalaw_1.speakerName = "KALAW";
        Kalaw_1.dialogueText = "\"Let's start with a simple word you'll use often when meeting someone.\nIn Ilokano, 'kumusta' means 'hello.'\nIt is a friendly greeting you can use when you meet someone or begin a conversation.\nListen carefully: kumusta.\nNow, try saying 'kumusta' yourself.\"\n";
        EditorUtility.SetDirty(Kalaw_1);
        AddChoice(Kalaw_prev, "Continue", Kalaw_1);
        Kalaw_prev = Kalaw_1;
        DialogueNode Kalaw_1_succ = GetOrCreateNode(outputFolder + "/Kalaw_1_succ.asset");
        Kalaw_1_succ.speakerName = "KALAW";
        Kalaw_1_succ.dialogueText = "\"Kumusta! Nailed it! That's a great way to greet someone!\"\n";
        EditorUtility.SetDirty(Kalaw_1_succ);
        AddChoice(Kalaw_1, "Say: \"hello → kumusta\"", Kalaw_1_succ);
        Kalaw_prev = Kalaw_1_succ;
        DialogueNode Kalaw_2 = GetOrCreateNode(outputFolder + "/Kalaw_2.asset");
        Kalaw_2.speakerName = "KALAW";
        Kalaw_2.dialogueText = "\"Once you've greeted someone, you can show more warmth by asking how they are.\nIn Ilokano, 'kumusta ka?' means 'how are you?'\nYou can use it when checking on a friend, neighbor, or someone you've just met.\nListen carefully: kumusta ka?\nNow, try saying 'kumusta ka?' yourself.\"\n";
        EditorUtility.SetDirty(Kalaw_2);
        AddChoice(Kalaw_prev, "Continue", Kalaw_2);
        Kalaw_prev = Kalaw_2;
        DialogueNode Kalaw_2_succ = GetOrCreateNode(outputFolder + "/Kalaw_2_succ.asset");
        Kalaw_2_succ.speakerName = "KALAW";
        Kalaw_2_succ.dialogueText = "\"Kumusta ka! Ah, see that smile? You're making friends already!\"\n";
        EditorUtility.SetDirty(Kalaw_2_succ);
        AddChoice(Kalaw_2, "Say: \"how are you? → kumusta ka?\"", Kalaw_2_succ);
        Kalaw_prev = Kalaw_2_succ;
        DialogueNode Kalaw_3 = GetOrCreateNode(outputFolder + "/Kalaw_3.asset");
        Kalaw_3.speakerName = "KALAW";
        SplitDialogueNode(Kalaw_3, "\"When someone asks you 'kumusta ka?', you can tell them how you're doing.\nIn Ilokano, 'nasayaat ak' means 'I'm fine' or 'I'm doing well.'\nFor example, if someone asks how your journey is going, you can answer: 'Nasayaat ak.'\nListen carefully: nasayaat ak.\nNow, try saying 'nasayaat ak' yourself.\"\n", outputFolder, "Kalaw_3");
        EditorUtility.SetDirty(Kalaw_3);
        AddChoice(Kalaw_prev, "Continue", Kalaw_3);
        Kalaw_prev = Kalaw_3;
        DialogueNode Kalaw_3_succ = GetOrCreateNode(outputFolder + "/Kalaw_3_succ.asset");
        Kalaw_3_succ.speakerName = "KALAW";
        Kalaw_3_succ.dialogueText = "\"Nasayaat ak! That's the spirit! Keep that energy up!\"\n";
        EditorUtility.SetDirty(Kalaw_3_succ);
        AddChoice(Kalaw_3, "Say: \"i'm fine → nasayaat ak\"", Kalaw_3_succ);
        Kalaw_prev = Kalaw_3_succ;
        DialogueNode Kalaw_4 = GetOrCreateNode(outputFolder + "/Kalaw_4.asset");
        Kalaw_4.speakerName = "KALAW";
        SplitDialogueNode(Kalaw_4, "\"Look at the morning sun shining over the tiled roofs of Calle Crisologo.\nIn Ilokano, 'naimbag a bigat' means 'good morning.'\nYou can use it when greeting someone earlier in the day.\nFor example, you can use it when greeting vendors as they begin their morning.\nListen carefully: naimbag a bigat.\nNow, try saying 'naimbag a bigat' yourself.\"\n", outputFolder, "Kalaw_4");
        EditorUtility.SetDirty(Kalaw_4);
        AddChoice(Kalaw_prev, "Continue", Kalaw_4);
        Kalaw_prev = Kalaw_4;
        DialogueNode Kalaw_4_succ = GetOrCreateNode(outputFolder + "/Kalaw_4_succ.asset");
        Kalaw_4_succ.speakerName = "KALAW";
        Kalaw_4_succ.dialogueText = "\"Naimbag a bigat! You've got the morning greeting down!\nFly over to Vendor Kyros's souvenir stall. He'll teach you how to greet people at other times of the day.\"\n";
        EditorUtility.SetDirty(Kalaw_4_succ);
        AddChoice(Kalaw_4, "Say: \"good morning → naimbag a bigat\"", Kalaw_4_succ);
        Kalaw_prev = Kalaw_4_succ;

        InteractableNPC[] Kalaw_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Kalaw_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Kalaw", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Kalaw_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Kyros ---
        {
        DialogueNode Kyros_start = null;
        DialogueNode Kyros_prev = null;
        DialogueNode Kyros_0 = GetOrCreateNode(outputFolder + "/Kyros_0.asset");
        Kyros_0.speakerName = "VENDOR KYROS";
        Kyros_0.dialogueText = "";
        EditorUtility.SetDirty(Kyros_0);
        Kyros_start = Kyros_0;
        Kyros_prev = Kyros_0;
        DialogueNode Kyros_1 = GetOrCreateNode(outputFolder + "/Kyros_1.asset");
        Kyros_1.speakerName = "VENDOR KYROS";
        Kyros_1.dialogueText = "\"Naimbag nga aldaw, traveler!";
        EditorUtility.SetDirty(Kyros_1);
        AddChoice(Kyros_prev, "Continue", Kyros_1);
        DialogueNode Kyros_1_part1 = GetOrCreateNode(outputFolder + "/Kyros_1_part1.asset");
        Kyros_1_part1.speakerName = "VENDOR KYROS";
        Kyros_1_part1.dialogueText = "Kalaw told me a new adventurer was exploring Vigan.\nWhen the morning has passed and the afternoon arrives, Ilocanos can greet someone by saying 'naimbag a malem.'\nIt means 'good afternoon.'\nYou can use it when greeting someone during the afternoon.\nListen carefully: naimbag a malem.\nNow, try saying 'naimbag a malem' yourself.\"\n";
        EditorUtility.SetDirty(Kyros_1_part1);
        AddChoice(Kyros_1, "Continue", Kyros_1_part1);
        Kyros_prev = Kyros_1_part1;
        DialogueNode Kyros_1_succ = GetOrCreateNode(outputFolder + "/Kyros_1_succ.asset");
        Kyros_1_succ.speakerName = "VENDOR KYROS";
        Kyros_1_succ.dialogueText = "\"Naimbag a malem! Welcome to my shop, friend!\"\n";
        EditorUtility.SetDirty(Kyros_1_succ);
        AddChoice(Kyros_1_part1, "Say: \"good afternoon → naimbag a malem\"", Kyros_1_succ);
        Kyros_prev = Kyros_1_succ;
        DialogueNode Kyros_2 = GetOrCreateNode(outputFolder + "/Kyros_2.asset");
        Kyros_2.speakerName = "VENDOR KYROS";
        Kyros_2.dialogueText = "\"As the sun sets and the streetlamps begin to glow, the greeting changes with the time of day.\nIn Ilokano, 'naimbag a rabii' means 'good evening.'\nUse it when greeting someone in the evening.\nListen carefully: naimbag a rabii.\nNow, try saying 'naimbag a rabii' yourself.\"\n";
        EditorUtility.SetDirty(Kyros_2);
        AddChoice(Kyros_prev, "Continue", Kyros_2);
        Kyros_prev = Kyros_2;
        DialogueNode Kyros_2_succ = GetOrCreateNode(outputFolder + "/Kyros_2_succ.asset");
        Kyros_2_succ.speakerName = "VENDOR KYROS";
        Kyros_2_succ.dialogueText = "\"Naimbag a rabii! Look at your pendant glow!\"\n";
        EditorUtility.SetDirty(Kyros_2_succ);
        AddChoice(Kyros_2, "Say: \"good evening → naimbag a rabii\"", Kyros_2_succ);
        Kyros_prev = Kyros_2_succ;
        DialogueNode Kyros_3 = GetOrCreateNode(outputFolder + "/Kyros_3.asset");
        Kyros_3.speakerName = "VENDOR KYROS";
        SplitDialogueNode(Kyros_3, "\"Sometimes you want to give someone a general greeting without focusing on morning, afternoon, or evening.\nIn Ilokano, 'naimbag nga aldaw' means 'good day.'\nIt is a general greeting that can be used to wish someone a pleasant day.\nListen carefully: naimbag nga aldaw.\nNow, try saying 'naimbag nga aldaw' yourself.\"\n", outputFolder, "Kyros_3");
        EditorUtility.SetDirty(Kyros_3);
        AddChoice(Kyros_prev, "Continue", Kyros_3);
        Kyros_prev = Kyros_3;
        DialogueNode Kyros_3_succ = GetOrCreateNode(outputFolder + "/Kyros_3_succ.asset");
        Kyros_3_succ.speakerName = "VENDOR KYROS";
        Kyros_3_succ.dialogueText = "\"Naimbag nga aldaw! Hope you enjoy your stroll down Calle Crisologo!\"\n";
        EditorUtility.SetDirty(Kyros_3_succ);
        AddChoice(Kyros_3, "Say: \"good day → naimbag nga aldaw\"", Kyros_3_succ);
        Kyros_prev = Kyros_3_succ;
        DialogueNode Kyros_4 = GetOrCreateNode(outputFolder + "/Kyros_4.asset");
        Kyros_4.speakerName = "VENDOR KYROS";
        Kyros_4.dialogueText = "\"Every journey eventually continues to another place.\nWhen leaving someone, you can say 'agpakada akon' to say 'goodbye.'\nUse it when ending a conversation or parting ways.\nListen carefully: agpakada akon.\nNow, try saying 'agpakada akon' yourself.\"\n";
        EditorUtility.SetDirty(Kyros_4);
        AddChoice(Kyros_prev, "Continue", Kyros_4);
        Kyros_prev = Kyros_4;
        DialogueNode Kyros_4_succ = GetOrCreateNode(outputFolder + "/Kyros_4_succ.asset");
        Kyros_4_succ.speakerName = "VENDOR KYROS";
        Kyros_4_succ.dialogueText = "\"Agpakada akon! ✨ GREETINGS MILESTONE UNLOCKED! ✨\nGo meet Vendor Irah at the weaving loom. She has something important to teach you about showing gratitude.\"\n";
        EditorUtility.SetDirty(Kyros_4_succ);
        AddChoice(Kyros_4, "Say: \"goodbye → agpakada akon\"", Kyros_4_succ);
        Kyros_prev = Kyros_4_succ;

        InteractableNPC[] Kyros_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Kyros_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Kyros", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Kyros_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Irah ---
        {
        DialogueNode Irah_start = null;
        DialogueNode Irah_prev = null;
        DialogueNode Irah_0 = GetOrCreateNode(outputFolder + "/Irah_0.asset");
        Irah_0.speakerName = "VENDOR IRAH";
        Irah_0.dialogueText = "";
        EditorUtility.SetDirty(Irah_0);
        Irah_start = Irah_0;
        Irah_prev = Irah_0;
        DialogueNode Irah_1 = GetOrCreateNode(outputFolder + "/Irah_1.asset");
        Irah_1.speakerName = "VENDOR IRAH";
        Irah_1.dialogueText = "\"Clack-clack-clack goes the loom!";
        EditorUtility.SetDirty(Irah_1);
        AddChoice(Irah_prev, "Continue", Irah_1);
        DialogueNode Irah_1_part1 = GetOrCreateNode(outputFolder + "/Irah_1_part1.asset");
        Irah_1_part1.speakerName = "VENDOR IRAH";
        Irah_1_part1.dialogueText = "Welcome, traveler!\nWhen someone helps you, gives you something, or does something kind for you, it is important to show appreciation.\nIn Ilokano, 'agyamanak' means 'thank you.'\nYou can use it whenever you want to express gratitude.\nListen carefully: agyamanak.\nNow, try saying 'agyamanak' yourself.\"\n";
        EditorUtility.SetDirty(Irah_1_part1);
        AddChoice(Irah_1, "Continue", Irah_1_part1);
        Irah_prev = Irah_1_part1;
        DialogueNode Irah_1_succ = GetOrCreateNode(outputFolder + "/Irah_1_succ.asset");
        Irah_1_succ.speakerName = "VENDOR IRAH";
        Irah_1_succ.dialogueText = "\"Agyamanak! Showing gratitude helps keep our community close-knit!\"\n";
        EditorUtility.SetDirty(Irah_1_succ);
        AddChoice(Irah_1_part1, "Say: \"thank you → agyamanak\"", Irah_1_succ);
        Irah_prev = Irah_1_succ;
        DialogueNode Irah_2 = GetOrCreateNode(outputFolder + "/Irah_2.asset");
        Irah_2.speakerName = "VENDOR IRAH";
        Irah_2.dialogueText = "\"Sometimes a simple thank you isn't enough to express how grateful you feel.\nIn Ilokano, 'agyamanak unay' means 'thank you very much.'\nYou can use it when someone does something especially kind or generous for you.\nListen carefully: agyamanak unay.\nNow, try saying 'agyamanak unay' yourself.\"\n";
        EditorUtility.SetDirty(Irah_2);
        AddChoice(Irah_prev, "Continue", Irah_2);
        Irah_prev = Irah_2;
        DialogueNode Irah_2_succ = GetOrCreateNode(outputFolder + "/Irah_2_succ.asset");
        Irah_2_succ.speakerName = "VENDOR IRAH";
        Irah_2_succ.dialogueText = "\"Agyamanak unay! May your journey be smooth and bright!\"\n";
        EditorUtility.SetDirty(Irah_2_succ);
        AddChoice(Irah_2, "Say: \"thank you very much → agyamanak unay\"", Irah_2_succ);
        Irah_prev = Irah_2_succ;
        DialogueNode Irah_3 = GetOrCreateNode(outputFolder + "/Irah_3.asset");
        Irah_3.speakerName = "VENDOR IRAH";
        SplitDialogueNode(Irah_3, "\"When someone gives you a helping hand, you can thank them specifically for their assistance.\nIn Ilokano, 'agyamanak iti tulong mo' means 'thank you for your help.'\nYou can use it when someone helps you complete a task or solve a problem.\nListen carefully: agyamanak iti tulong mo.\nNow, try saying 'agyamanak iti tulong mo' yourself.\"\n", outputFolder, "Irah_3");
        EditorUtility.SetDirty(Irah_3);
        AddChoice(Irah_prev, "Continue", Irah_3);
        Irah_prev = Irah_3;
        DialogueNode Irah_3_succ = GetOrCreateNode(outputFolder + "/Irah_3_succ.asset");
        Irah_3_succ.speakerName = "VENDOR IRAH";
        Irah_3_succ.dialogueText = "\"Agyamanak iti tulong mo! We Ilocanos love helping one another!\"\n";
        EditorUtility.SetDirty(Irah_3_succ);
        AddChoice(Irah_3, "Say: \"thank you for your help → agyamanak iti tulong mo\"", Irah_3_succ);
        Irah_prev = Irah_3_succ;
        DialogueNode Irah_4 = GetOrCreateNode(outputFolder + "/Irah_4.asset");
        Irah_4.speakerName = "VENDOR IRAH";
        Irah_4.dialogueText = "\"Oops!";
        EditorUtility.SetDirty(Irah_4);
        AddChoice(Irah_prev, "Continue", Irah_4);
        DialogueNode Irah_4_part1 = GetOrCreateNode(outputFolder + "/Irah_4_part1.asset");
        Irah_4_part1.speakerName = "VENDOR IRAH";
        Irah_4_part1.dialogueText = "Be careful around these delicate threads.\nIf you accidentally bump into someone or make a mistake, you can apologize.\nIn Ilokano, 'pakawanen nak' means 'I am sorry.'\nUse it when you want to apologize for something you've done.\nListen carefully: pakawanen nak.\nNow, try saying 'pakawanen nak' yourself.\"\n";
        EditorUtility.SetDirty(Irah_4_part1);
        AddChoice(Irah_4, "Continue", Irah_4_part1);
        Irah_prev = Irah_4_part1;
        DialogueNode Irah_4_succ = GetOrCreateNode(outputFolder + "/Irah_4_succ.asset");
        Irah_4_succ.speakerName = "VENDOR IRAH";
        Irah_4_succ.dialogueText = "\"Pakawanen nak... No harm done!\nPop over to Vendor Jom's empanada stall for the last gratitude phrase!\"\n";
        EditorUtility.SetDirty(Irah_4_succ);
        AddChoice(Irah_4_part1, "Say: \"i am sorry → pakawanen nak\"", Irah_4_succ);
        Irah_prev = Irah_4_succ;

        InteractableNPC[] Irah_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Irah_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Irah", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Irah_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Jom ---
        {
        DialogueNode Jom_start = null;
        DialogueNode Jom_prev = null;
        DialogueNode Jom_0 = GetOrCreateNode(outputFolder + "/Jom_0.asset");
        Jom_0.speakerName = "VENDOR JOM";
        Jom_0.dialogueText = "";
        EditorUtility.SetDirty(Jom_0);
        Jom_start = Jom_0;
        Jom_prev = Jom_0;
        DialogueNode Jom_1 = GetOrCreateNode(outputFolder + "/Jom_1.asset");
        Jom_1.speakerName = "VENDOR JOM";
        Jom_1.dialogueText = "\"Smell that sizzling longganisa?";
        EditorUtility.SetDirty(Jom_1);
        AddChoice(Jom_prev, "Continue", Jom_1);
        DialogueNode Jom_1_part1 = GetOrCreateNode(outputFolder + "/Jom_1_part1.asset");
        Jom_1_part1.speakerName = "VENDOR JOM";
        Jom_1_part1.dialogueText = "My counter is getting crowded!\nWhen you need to politely get someone's attention or pass through a crowded space, you can say 'dispensaren nak.'\nIt means 'excuse me.'\nListen carefully: dispensaren nak.\nNow, try saying 'dispensaren nak' yourself.\"\n";
        EditorUtility.SetDirty(Jom_1_part1);
        AddChoice(Jom_1, "Continue", Jom_1_part1);
        Jom_prev = Jom_1_part1;
        DialogueNode Jom_1_succ = GetOrCreateNode(outputFolder + "/Jom_1_succ.asset");
        Jom_1_succ.speakerName = "VENDOR JOM";
        Jom_1_succ.dialogueText = "\"Dispensaren nak! Sharp manners!\n✨ GRATITUDE MILESTONE UNLOCKED! ✨\nStick around for the Response trials!\"\n";
        EditorUtility.SetDirty(Jom_1_succ);
        AddChoice(Jom_1_part1, "Say: \"excuse me → dispensaren nak\"", Jom_1_succ);
        Jom_prev = Jom_1_succ;
        DialogueNode Jom_2 = GetOrCreateNode(outputFolder + "/Jom_2.asset");
        Jom_2.speakerName = "VENDOR JOM";
        Jom_2.dialogueText = "\"Let's practice a simple response.\nWhen someone asks you a question and you want to agree or answer 'yes,' Ilokano uses the word 'wen.'\nListen carefully: wen.\nNow, try saying 'wen' yourself.\"\n";
        EditorUtility.SetDirty(Jom_2);
        AddChoice(Jom_prev, "Continue", Jom_2);
        Jom_prev = Jom_2;
        DialogueNode Jom_2_succ = GetOrCreateNode(outputFolder + "/Jom_2_succ.asset");
        Jom_2_succ.speakerName = "VENDOR JOM";
        Jom_2_succ.dialogueText = "\"Wen! That's what I like to hear! Sizzling away!\"\n";
        EditorUtility.SetDirty(Jom_2_succ);
        AddChoice(Jom_2, "Say: \"yes → wen\"", Jom_2_succ);
        Jom_prev = Jom_2_succ;
        DialogueNode Jom_3 = GetOrCreateNode(outputFolder + "/Jom_3.asset");
        Jom_3.speakerName = "VENDOR JOM";
        Jom_3.dialogueText = "\"Of course, sometimes you need to politely disagree or say 'no.'\nIn Ilokano, 'saan' means 'no.'\nYou can use it when you want to give a negative response.\nListen carefully: saan.\nNow, try saying 'saan' yourself.\"\n";
        EditorUtility.SetDirty(Jom_3);
        AddChoice(Jom_prev, "Continue", Jom_3);
        Jom_prev = Jom_3;
        DialogueNode Jom_3_succ = GetOrCreateNode(outputFolder + "/Jom_3_succ.asset");
        Jom_3_succ.speakerName = "VENDOR JOM";
        Jom_3_succ.dialogueText = "\"Saan! Got it! I'll keep that garlic vinegar ready for another time!\"\n";
        EditorUtility.SetDirty(Jom_3_succ);
        AddChoice(Jom_3, "Say: \"no → saan\"", Jom_3_succ);
        Jom_prev = Jom_3_succ;
        DialogueNode Jom_4 = GetOrCreateNode(outputFolder + "/Jom_4.asset");
        Jom_4.speakerName = "VENDOR JOM";
        Jom_4.dialogueText = "\"Sometimes you simply want to show that you agree or understand.\nIn Ilokano, 'okay' can also be used to say 'okay.'\nIt is a simple response used to acknowledge something or show agreement.\nListen carefully: okay.\nNow, try saying 'okay' yourself.\"\n";
        EditorUtility.SetDirty(Jom_4);
        AddChoice(Jom_prev, "Continue", Jom_4);
        Jom_prev = Jom_4;
        DialogueNode Jom_4_succ = GetOrCreateNode(outputFolder + "/Jom_4_succ.asset");
        Jom_4_succ.speakerName = "VENDOR JOM";
        Jom_4_succ.dialogueText = "\"Okay! Easy enough! You're picking this up quickly!\"\n";
        EditorUtility.SetDirty(Jom_4_succ);
        AddChoice(Jom_4, "Say: \"okay → okay\"", Jom_4_succ);
        Jom_prev = Jom_4_succ;
        DialogueNode Jom_5 = GetOrCreateNode(outputFolder + "/Jom_5.asset");
        Jom_5.speakerName = "VENDOR JOM";
        Jom_5.dialogueText = "\"When someone explains something and you want to tell them that you understand, you can use 'maawatan ko.'\nIt means 'I understand.'\nListen carefully: maawatan ko.\nNow, try saying 'maawatan ko' yourself.\"\n";
        EditorUtility.SetDirty(Jom_5);
        AddChoice(Jom_prev, "Continue", Jom_5);
        Jom_prev = Jom_5;
        DialogueNode Jom_5_succ = GetOrCreateNode(outputFolder + "/Jom_5_succ.asset");
        Jom_5_succ.speakerName = "VENDOR JOM";
        Jom_5_succ.dialogueText = "\"Maawatan ko! Excellent! You're understanding more and more!\"\n";
        EditorUtility.SetDirty(Jom_5_succ);
        AddChoice(Jom_5, "Say: \"i understand → mawatan ko\"", Jom_5_succ);
        Jom_prev = Jom_5_succ;
        DialogueNode Jom_6 = GetOrCreateNode(outputFolder + "/Jom_6.asset");
        Jom_6.speakerName = "VENDOR JOM";
        Jom_6.dialogueText = "\"If someone says something you don't understand, it's useful to let them know.\nIn Ilokano, 'diak maawatan' means 'I don't understand.'\nYou can use it when you need someone to explain something again.\nListen carefully: diak maawatan.\nNow, try saying 'diak maawatan' yourself.\"\n";
        EditorUtility.SetDirty(Jom_6);
        AddChoice(Jom_prev, "Continue", Jom_6);
        Jom_prev = Jom_6;
        DialogueNode Jom_6_succ = GetOrCreateNode(outputFolder + "/Jom_6_succ.asset");
        Jom_6_succ.speakerName = "VENDOR JOM";
        Jom_6_succ.dialogueText = "\"Diak maawatan! No worries-learning takes practice!\n✨ RESPONSES MILESTONE UNLOCKED! ✨\"\n";
        EditorUtility.SetDirty(Jom_6_succ);
        AddChoice(Jom_6, "Say: \"i don't understand → diak mawatan\"", Jom_6_succ);
        Jom_prev = Jom_6_succ;

        InteractableNPC[] Jom_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Jom_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Jom", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Jom_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Ronnie ---
        {
        DialogueNode Ronnie_start = null;
        DialogueNode Ronnie_prev = null;
        DialogueNode Ronnie_0 = GetOrCreateNode(outputFolder + "/Ronnie_0.asset");
        Ronnie_0.speakerName = "RONNIE";
        Ronnie_0.dialogueText = "";
        EditorUtility.SetDirty(Ronnie_0);
        Ronnie_start = Ronnie_0;
        Ronnie_prev = Ronnie_0;
        DialogueNode Ronnie_1 = GetOrCreateNode(outputFolder + "/Ronnie_1.asset");
        Ronnie_1.speakerName = "RONNIE";
        SplitDialogueNode(Ronnie_1, "\"When meeting someone new, one of the first things you may want to know is their name.\nIn Ilokano, 'ania ti nagan mo?' means 'what is your name?'\nYou can use this phrase when introducing yourself to someone or asking for their name.\nListen carefully: ania ti nagan mo?\nNow, try saying 'ania ti nagan mo?' yourself.\"\n", outputFolder, "Ronnie_1");
        EditorUtility.SetDirty(Ronnie_1);
        AddChoice(Ronnie_prev, "Continue", Ronnie_1);
        Ronnie_prev = Ronnie_1;
        DialogueNode Ronnie_1_succ = GetOrCreateNode(outputFolder + "/Ronnie_1_succ.asset");
        Ronnie_1_succ.speakerName = "RONNIE";
        Ronnie_1_succ.dialogueText = "\"Ania ti nagan mo? That's a great question to ask when meeting someone new!\"\n";
        EditorUtility.SetDirty(Ronnie_1_succ);
        AddChoice(Ronnie_1, "Say: \"what is your name? → ania ti nagan mo\"", Ronnie_1_succ);
        Ronnie_prev = Ronnie_1_succ;
        DialogueNode Ronnie_2 = GetOrCreateNode(outputFolder + "/Ronnie_2.asset");
        Ronnie_2.speakerName = "RONNIE";
        Ronnie_2.dialogueText = "\"Now it's your turn to introduce yourself.\nIn Ilokano, 'ti nagan ko ket...' means 'my name is...'\nThe words 'ti nagan ko ket' form the fixed part of the phrase.";
        EditorUtility.SetDirty(Ronnie_2);
        AddChoice(Ronnie_prev, "Continue", Ronnie_2);
        DialogueNode Ronnie_2_part1 = GetOrCreateNode(outputFolder + "/Ronnie_2_part1.asset");
        Ronnie_2_part1.speakerName = "RONNIE";
        Ronnie_2_part1.dialogueText = "You then add your own name.\nFor example: 'Ti nagan ko ket Jom.'\nListen carefully: ti nagan ko ket Jom.\nNow, try introducing yourself using 'ti nagan ko ket...' and your own name.\"\n";
        EditorUtility.SetDirty(Ronnie_2_part1);
        AddChoice(Ronnie_2, "Continue", Ronnie_2_part1);
        Ronnie_prev = Ronnie_2_part1;
        DialogueNode Ronnie_2_succ = GetOrCreateNode(outputFolder + "/Ronnie_2_succ.asset");
        Ronnie_2_succ.speakerName = "RONNIE";
        Ronnie_2_succ.dialogueText = "\"Awesome to meet you! You just introduced yourself in Ilokano!\nGo meet Sally near the brick arch to complete Level I!\"\n";
        EditorUtility.SetDirty(Ronnie_2_succ);
        AddChoice(Ronnie_2_part1, "Say: \"my name is ___ → ti nagan ko ket ___\"", Ronnie_2_succ);
        Ronnie_prev = Ronnie_2_succ;

        InteractableNPC[] Ronnie_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Ronnie_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Ronnie", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Ronnie_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Sally ---
        {
        DialogueNode Sally_start = null;
        DialogueNode Sally_prev = null;
        DialogueNode Sally_0 = GetOrCreateNode(outputFolder + "/Sally_0.asset");
        Sally_0.speakerName = "SALLY";
        Sally_0.dialogueText = "";
        EditorUtility.SetDirty(Sally_0);
        Sally_start = Sally_0;
        Sally_prev = Sally_0;
        DialogueNode Sally_1 = GetOrCreateNode(outputFolder + "/Sally_1.asset");
        Sally_1.speakerName = "SALLY";
        Sally_1.dialogueText = "\"When meeting someone new, you may also want to learn where they come from.\nIn Ilokano, 'taga sadino ka?' means 'where are you from?'\nUse it when asking someone about their hometown or place of origin.\nListen carefully: taga sadino ka?\nNow, try saying 'taga sadino ka?' yourself.\"\n";
        EditorUtility.SetDirty(Sally_1);
        AddChoice(Sally_prev, "Continue", Sally_1);
        Sally_prev = Sally_1;
        DialogueNode Sally_1_succ = GetOrCreateNode(outputFolder + "/Sally_1_succ.asset");
        Sally_1_succ.speakerName = "SALLY";
        Sally_1_succ.dialogueText = "\"Taga sadino ka? That's how you ask where someone comes from!\"\n";
        EditorUtility.SetDirty(Sally_1_succ);
        AddChoice(Sally_1, "Say: \"where are you from? → taga sadino ka\"", Sally_1_succ);
        Sally_prev = Sally_1_succ;
        DialogueNode Sally_2 = GetOrCreateNode(outputFolder + "/Sally_2.asset");
        Sally_2.speakerName = "SALLY";
        SplitDialogueNode(Sally_2, "\"Now you can answer that question yourself.\nIn Ilokano, 'taga ___ ak' means 'I am from ___.'\nThe phrase 'taga' introduces the place you come from, while the location changes depending on your answer.\nFor example: 'taga Vigan ak.'\nListen carefully: taga Vigan ak.\nNow, try saying 'taga' followed by your hometown.\"\n", outputFolder, "Sally_2");
        EditorUtility.SetDirty(Sally_2);
        AddChoice(Sally_prev, "Continue", Sally_2);
        Sally_prev = Sally_2;
        DialogueNode Sally_2_succ = GetOrCreateNode(outputFolder + "/Sally_2_succ.asset");
        Sally_2_succ.speakerName = "SALLY";
        Sally_2_succ.dialogueText = "\"Excellent! You just told me where you're from!\n🏆 LEVEL I: CONVERSATIONAL & SOCIAL COMPLETE!";
        EditorUtility.SetDirty(Sally_2_succ);
        AddChoice(Sally_2, "Say: \"i am from ___ → taga ___ ak\"", Sally_2_succ);
        DialogueNode Sally_2_succ_part1 = GetOrCreateNode(outputFolder + "/Sally_2_succ_part1.asset");
        Sally_2_succ_part1.speakerName = "SALLY";
        Sally_2_succ_part1.dialogueText = "🏆\nYou've learned how to greet people, express gratitude, respond to others, and introduce yourself.\nYour journey now takes you deeper into everyday communication.\nWelcome to Level II: Functional & Navigational!\"\n";
        EditorUtility.SetDirty(Sally_2_succ_part1);
        AddChoice(Sally_2_succ, "Continue", Sally_2_succ_part1);
        Sally_prev = Sally_2_succ_part1;
        DialogueNode Sally_3 = GetOrCreateNode(outputFolder + "/Sally_3.asset");
        Sally_3.speakerName = "SALLY";
        Sally_3.dialogueText = "\"If you are in trouble or need assistance, you need a direct way to ask for help.\nIn Ilokano, 'tulunganak' means 'help me.'\nUse it when you urgently need someone to assist you.\nListen carefully: tulunganak.\nNow, try saying 'tulunganak' yourself.\"\n";
        EditorUtility.SetDirty(Sally_3);
        AddChoice(Sally_prev, "Continue", Sally_3);
        Sally_prev = Sally_3;
        DialogueNode Sally_3_succ = GetOrCreateNode(outputFolder + "/Sally_3_succ.asset");
        Sally_3_succ.speakerName = "SALLY";
        Sally_3_succ.dialogueText = "\"Tulunganak! Don't worry-you're never alone!\"\n";
        EditorUtility.SetDirty(Sally_3_succ);
        AddChoice(Sally_3, "Say: \"help me → tulongannak\"", Sally_3_succ);
        Sally_prev = Sally_3_succ;
        DialogueNode Sally_4 = GetOrCreateNode(outputFolder + "/Sally_4.asset");
        Sally_4.speakerName = "SALLY";
        Sally_4.dialogueText = "\"Sometimes you want to ask for help more politely.\nIn Ilokano, 'mabalin kadi a tulunganak?' means 'can you help me?'\nThis is useful when politely requesting someone's assistance.\nListen carefully: mabalin kadi a tulunganak?\nNow, try saying 'mabalin kadi a tulunganak?' yourself.\"\n";
        EditorUtility.SetDirty(Sally_4);
        AddChoice(Sally_prev, "Continue", Sally_4);
        Sally_prev = Sally_4;
        DialogueNode Sally_4_succ = GetOrCreateNode(outputFolder + "/Sally_4_succ.asset");
        Sally_4_succ.speakerName = "SALLY";
        Sally_4_succ.dialogueText = "\"Mabalin kadi a tulunganak! Perfect!\nFind Tour Guide Lito for the rest of these requests!\"\n";
        EditorUtility.SetDirty(Sally_4_succ);
        AddChoice(Sally_4, "Say: \"can you help me → mabalin kadi a tulongannak\"", Sally_4_succ);
        Sally_prev = Sally_4_succ;

        InteractableNPC[] Sally_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Sally_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Sally", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Sally_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Lito ---
        {
        DialogueNode Lito_start = null;
        DialogueNode Lito_prev = null;
        DialogueNode Lito_0 = GetOrCreateNode(outputFolder + "/Lito_0.asset");
        Lito_0.speakerName = "LITO";
        Lito_0.dialogueText = "";
        EditorUtility.SetDirty(Lito_0);
        Lito_start = Lito_0;
        Lito_prev = Lito_0;
        DialogueNode Lito_1 = GetOrCreateNode(outputFolder + "/Lito_1.asset");
        Lito_1.speakerName = "LITO";
        Lito_1.dialogueText = "\"Look out! A horse carriage is passing through!\nWhen you need someone to pause and wait for you, you can say 'urayennak.'\nIt means 'please wait' or 'wait for me.'\nListen carefully: urayennak.\nNow, try saying 'urayennak' yourself.\"\n";
        EditorUtility.SetDirty(Lito_1);
        AddChoice(Lito_prev, "Continue", Lito_1);
        Lito_prev = Lito_1;
        DialogueNode Lito_1_succ = GetOrCreateNode(outputFolder + "/Lito_1_succ.asset");
        Lito_1_succ.speakerName = "LITO";
        Lito_1_succ.dialogueText = "\"Urayennak! Safety first on these narrow streets!\"\n";
        EditorUtility.SetDirty(Lito_1_succ);
        AddChoice(Lito_1, "Say: \"please wait → urayennak\"", Lito_1_succ);
        Lito_prev = Lito_1_succ;
        DialogueNode Lito_2 = GetOrCreateNode(outputFolder + "/Lito_2.asset");
        Lito_2.speakerName = "LITO";
        Lito_2.dialogueText = "\"When asking someone to hand you an object, you can use 'ikanmo man...'\nIt means 'give me...' and you add the item you want afterward.\nFor example: 'ikanmo man map' - 'give me the map.'\nListen carefully: ikanmo man map.\nNow, try saying 'ikanmo man' followed by an object you want.\"\n";
        EditorUtility.SetDirty(Lito_2);
        AddChoice(Lito_prev, "Continue", Lito_2);
        Lito_prev = Lito_2;
        DialogueNode Lito_2_succ = GetOrCreateNode(outputFolder + "/Lito_2_succ.asset");
        Lito_2_succ.speakerName = "LITO";
        Lito_2_succ.dialogueText = "\"Ikanmo man! Well done! Here is your map of Vigan!\"\n";
        EditorUtility.SetDirty(Lito_2_succ);
        AddChoice(Lito_2, "Say: \"give me ___ → ikanmo man ___\"", Lito_2_succ);
        Lito_prev = Lito_2_succ;
        DialogueNode Lito_3 = GetOrCreateNode(outputFolder + "/Lito_3.asset");
        Lito_3.speakerName = "LITO";
        Lito_3.dialogueText = "\"Before asking someone a question, it's polite to ask for permission.\nIn Ilokano, 'mabalin kadi agsaludsod?' means 'can I ask?'\nYou can use it before asking someone for information.\nListen carefully: mabalin kadi agsaludsod?\nNow, try saying 'mabalin kadi agsaludsod?' yourself.\"\n";
        EditorUtility.SetDirty(Lito_3);
        AddChoice(Lito_prev, "Continue", Lito_3);
        Lito_prev = Lito_3;
        DialogueNode Lito_3_succ = GetOrCreateNode(outputFolder + "/Lito_3_succ.asset");
        Lito_3_succ.speakerName = "LITO";
        Lito_3_succ.dialogueText = "\"Mabalin kadi agsaludsod! Ask away!\n✨ REQUESTS MILESTONE UNLOCKED! ✨\"\n";
        EditorUtility.SetDirty(Lito_3_succ);
        AddChoice(Lito_3, "Say: \"can i ask → mabalin kadi agsaludsod\"", Lito_3_succ);
        Lito_prev = Lito_3_succ;
        DialogueNode Lito_4 = GetOrCreateNode(outputFolder + "/Lito_4.asset");
        Lito_4.speakerName = "LITO";
        Lito_4.dialogueText = "\"You're heading toward Plaza Salcedo and don't want to make any turns.\nIn Ilokano, 'agdiretso' means 'go straight.'\nYou can use it when giving directions or telling someone to continue forward.\nListen carefully: agdiretso.\nNow, try saying 'agdiretso' yourself.\"\n";
        EditorUtility.SetDirty(Lito_4);
        AddChoice(Lito_prev, "Continue", Lito_4);
        Lito_prev = Lito_4;
        DialogueNode Lito_4_succ = GetOrCreateNode(outputFolder + "/Lito_4_succ.asset");
        Lito_4_succ.speakerName = "LITO";
        Lito_4_succ.dialogueText = "\"Agdiretso! Straight ahead!\nGo find Apo Lakay by the stone well for more pathfinding!\"\n";
        EditorUtility.SetDirty(Lito_4_succ);
        AddChoice(Lito_4, "Say: \"go straight → agdiretso\"", Lito_4_succ);
        Lito_prev = Lito_4_succ;

        InteractableNPC[] Lito_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Lito_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Lito", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Lito_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: ApoLakay ---
        {
        DialogueNode ApoLakay_start = null;
        DialogueNode ApoLakay_prev = null;
        DialogueNode ApoLakay_0 = GetOrCreateNode(outputFolder + "/ApoLakay_0.asset");
        ApoLakay_0.speakerName = "APO LAKAY";
        ApoLakay_0.dialogueText = "";
        EditorUtility.SetDirty(ApoLakay_0);
        ApoLakay_start = ApoLakay_0;
        ApoLakay_prev = ApoLakay_0;
        DialogueNode ApoLakay_1 = GetOrCreateNode(outputFolder + "/ApoLakay_1.asset");
        ApoLakay_1.speakerName = "APO LAKAY";
        ApoLakay_1.dialogueText = "\"To reach the Burnay pottery yard, you need to tell someone which direction to take.\nIn Ilokano, 'agliko iti kannigid' means 'turn left.'\nUse it when directing someone to turn toward the left side.\nListen carefully: agliko iti kannigid.\nNow, try saying 'agliko iti kannigid' yourself.\"\n";
        EditorUtility.SetDirty(ApoLakay_1);
        AddChoice(ApoLakay_prev, "Continue", ApoLakay_1);
        ApoLakay_prev = ApoLakay_1;
        DialogueNode ApoLakay_1_succ = GetOrCreateNode(outputFolder + "/ApoLakay_1_succ.asset");
        ApoLakay_1_succ.speakerName = "APO LAKAY";
        ApoLakay_1_succ.dialogueText = "\"Agliko iti kannigid! Turn left-the clay kilns are right there!\"\n";
        EditorUtility.SetDirty(ApoLakay_1_succ);
        AddChoice(ApoLakay_1, "Say: \"turn left → agliko iti kannigid\"", ApoLakay_1_succ);
        ApoLakay_prev = ApoLakay_1_succ;
        DialogueNode ApoLakay_2 = GetOrCreateNode(outputFolder + "/ApoLakay_2.asset");
        ApoLakay_2.speakerName = "APO LAKAY";
        ApoLakay_2.dialogueText = "\"If you want someone to take the opposite direction, you can tell them to turn right.\nIn Ilokano, 'agliko iti kannawan' means 'turn right.'\nListen carefully: agliko iti kannawan.\nNow, try saying 'agliko iti kannawan' yourself.\"\n";
        EditorUtility.SetDirty(ApoLakay_2);
        AddChoice(ApoLakay_prev, "Continue", ApoLakay_2);
        ApoLakay_prev = ApoLakay_2;
        DialogueNode ApoLakay_2_succ = GetOrCreateNode(outputFolder + "/ApoLakay_2_succ.asset");
        ApoLakay_2_succ.speakerName = "APO LAKAY";
        ApoLakay_2_succ.dialogueText = "\"Agliko iti kannawan! Turn right at the corner!\"\n";
        EditorUtility.SetDirty(ApoLakay_2_succ);
        AddChoice(ApoLakay_2, "Say: \"turn right → agliko iti kannawan\"", ApoLakay_2_succ);
        ApoLakay_prev = ApoLakay_2_succ;
        DialogueNode ApoLakay_3 = GetOrCreateNode(outputFolder + "/ApoLakay_3.asset");
        ApoLakay_3.speakerName = "APO LAKAY";
        ApoLakay_3.dialogueText = "\"To reach the top of the tower and see the province below, you need to tell someone to move upward.\nIn Ilokano, 'umuli iti ngato' means 'go up.'\nUse it when directing someone toward a higher place.\nListen carefully: umuli iti ngato.\nNow, try saying 'umuli iti ngato' yourself.\"\n";
        EditorUtility.SetDirty(ApoLakay_3);
        AddChoice(ApoLakay_prev, "Continue", ApoLakay_3);
        ApoLakay_prev = ApoLakay_3;
        DialogueNode ApoLakay_3_succ = GetOrCreateNode(outputFolder + "/ApoLakay_3_succ.asset");
        ApoLakay_3_succ.speakerName = "APO LAKAY";
        ApoLakay_3_succ.dialogueText = "\"Umuli iti ngato! Climb up to the top!\"\n";
        EditorUtility.SetDirty(ApoLakay_3_succ);
        AddChoice(ApoLakay_3, "Say: \"go up → umuli iti ngato\"", ApoLakay_3_succ);
        ApoLakay_prev = ApoLakay_3_succ;
        DialogueNode ApoLakay_4 = GetOrCreateNode(outputFolder + "/ApoLakay_4.asset");
        ApoLakay_4.speakerName = "APO LAKAY";
        ApoLakay_4.dialogueText = "\"After enjoying the view, you'll need to return to the ground.\nIn Ilokano, 'bumaba' means 'go down.'\nUse it when telling someone to move toward a lower place.\nListen carefully: bumaba.\nNow, try saying 'bumaba' yourself.\"\n";
        EditorUtility.SetDirty(ApoLakay_4);
        AddChoice(ApoLakay_prev, "Continue", ApoLakay_4);
        ApoLakay_prev = ApoLakay_4;
        DialogueNode ApoLakay_4_succ = GetOrCreateNode(outputFolder + "/ApoLakay_4_succ.asset");
        ApoLakay_4_succ.speakerName = "APO LAKAY";
        ApoLakay_4_succ.dialogueText = "\"Bumaba! Mind your step!\nGo see Tomas at the pottery yard!\"\n";
        EditorUtility.SetDirty(ApoLakay_4_succ);
        AddChoice(ApoLakay_4, "Say: \"go down → bumaba\"", ApoLakay_4_succ);
        ApoLakay_prev = ApoLakay_4_succ;

        InteractableNPC[] ApoLakay_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in ApoLakay_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("ApoLakay", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = ApoLakay_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Tomas ---
        {
        DialogueNode Tomas_start = null;
        DialogueNode Tomas_prev = null;
        DialogueNode Tomas_0 = GetOrCreateNode(outputFolder + "/Tomas_0.asset");
        Tomas_0.speakerName = "TOMAS";
        Tomas_0.dialogueText = "";
        EditorUtility.SetDirty(Tomas_0);
        Tomas_start = Tomas_0;
        Tomas_prev = Tomas_0;
        DialogueNode Tomas_1 = GetOrCreateNode(outputFolder + "/Tomas_1.asset");
        Tomas_1.speakerName = "TOMAS";
        Tomas_1.dialogueText = "\"Welcome to the clay yard!\nIf you want someone to stop at the place where they are standing, you can say 'agsardeng ditoy.'\nIt means 'stop here.'\nListen carefully: agsardeng ditoy.\nNow, try saying 'agsardeng ditoy' yourself.\"\n";
        EditorUtility.SetDirty(Tomas_1);
        AddChoice(Tomas_prev, "Continue", Tomas_1);
        Tomas_prev = Tomas_1;
        DialogueNode Tomas_1_succ = GetOrCreateNode(outputFolder + "/Tomas_1_succ.asset");
        Tomas_1_succ.speakerName = "TOMAS";
        Tomas_1_succ.dialogueText = "\"Agsardeng ditoy! Perfect landing spot!\"\n";
        EditorUtility.SetDirty(Tomas_1_succ);
        AddChoice(Tomas_1, "Say: \"stop here → agsardeng ditoy\"", Tomas_1_succ);
        Tomas_prev = Tomas_1_succ;
        DialogueNode Tomas_2 = GetOrCreateNode(outputFolder + "/Tomas_2.asset");
        Tomas_2.speakerName = "TOMAS";
        Tomas_2.dialogueText = "\"Sometimes you need to call someone toward you.\nIn Ilokano, 'umay ditoy' means 'come here.'\nUse it when asking someone to move toward your location.\nListen carefully: umay ditoy.\nNow, try saying 'umay ditoy' yourself.\"\n";
        EditorUtility.SetDirty(Tomas_2);
        AddChoice(Tomas_prev, "Continue", Tomas_2);
        Tomas_prev = Tomas_2;
        DialogueNode Tomas_2_succ = GetOrCreateNode(outputFolder + "/Tomas_2_succ.asset");
        Tomas_2_succ.speakerName = "TOMAS";
        Tomas_2_succ.dialogueText = "\"Umay ditoy! Come take a look at this pottery!\"\n";
        EditorUtility.SetDirty(Tomas_2_succ);
        AddChoice(Tomas_2, "Say: \"come here → umay ditoy\"", Tomas_2_succ);
        Tomas_prev = Tomas_2_succ;
        DialogueNode Tomas_3 = GetOrCreateNode(outputFolder + "/Tomas_3.asset");
        Tomas_3.speakerName = "TOMAS";
        Tomas_3.dialogueText = "\"If you want someone to move toward another location, you can direct them away from where you are.\nIn Ilokano, 'mapan idiay' means 'go there.'\nListen carefully: mapan idiay.\nNow, try saying 'mapan idiay' yourself.\"\n";
        EditorUtility.SetDirty(Tomas_3);
        AddChoice(Tomas_prev, "Continue", Tomas_3);
        Tomas_prev = Tomas_3;
        DialogueNode Tomas_3_succ = GetOrCreateNode(outputFolder + "/Tomas_3_succ.asset");
        Tomas_3_succ.speakerName = "TOMAS";
        Tomas_3_succ.dialogueText = "\"Mapan idiay! Cool shade is much safer!\"\n";
        EditorUtility.SetDirty(Tomas_3_succ);
        AddChoice(Tomas_3, "Say: \"go there → mapan idiay\"", Tomas_3_succ);
        Tomas_prev = Tomas_3_succ;
        DialogueNode Tomas_4 = GetOrCreateNode(outputFolder + "/Tomas_4.asset");
        Tomas_4.speakerName = "TOMAS";
        Tomas_4.dialogueText = "\"When you want someone to come along with you, you can tell them to follow.\nIn Ilokano, 'surotennak' means 'follow me.'\nUse it when leading someone to another place.\nListen carefully: surotennak.\nNow, try saying 'surotennak' yourself.\"\n";
        EditorUtility.SetDirty(Tomas_4);
        AddChoice(Tomas_prev, "Continue", Tomas_4);
        Tomas_prev = Tomas_4;
        DialogueNode Tomas_4_succ = GetOrCreateNode(outputFolder + "/Tomas_4_succ.asset");
        Tomas_4_succ.speakerName = "TOMAS";
        Tomas_4_succ.dialogueText = "\"Surotennak! Follow me to the jar drying racks!\"\n";
        EditorUtility.SetDirty(Tomas_4_succ);
        AddChoice(Tomas_4, "Say: \"follow me → surotennak\"", Tomas_4_succ);
        Tomas_prev = Tomas_4_succ;

        InteractableNPC[] Tomas_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Tomas_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Tomas", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Tomas_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Klara ---
        {
        DialogueNode Klara_start = null;
        DialogueNode Klara_prev = null;
        DialogueNode Klara_0 = GetOrCreateNode(outputFolder + "/Klara_0.asset");
        Klara_0.speakerName = "KLARA";
        Klara_0.dialogueText = "";
        EditorUtility.SetDirty(Klara_0);
        Klara_start = Klara_0;
        Klara_prev = Klara_0;
        DialogueNode Klara_1 = GetOrCreateNode(outputFolder + "/Klara_1.asset");
        Klara_1.speakerName = "KLARA";
        Klara_1.dialogueText = "\"If you need someone to stay in their current location while you do something else, you can tell them to wait.\nIn Ilokano, 'uray ditoy' means 'wait here.'\nListen carefully: uray ditoy.\nNow, try saying 'uray ditoy' yourself.\"\n";
        EditorUtility.SetDirty(Klara_1);
        AddChoice(Klara_prev, "Continue", Klara_1);
        Klara_prev = Klara_1;
        DialogueNode Klara_1_succ = GetOrCreateNode(outputFolder + "/Klara_1_succ.asset");
        Klara_1_succ.speakerName = "KLARA";
        Klara_1_succ.dialogueText = "\"Uray ditoy!\n✨ DIRECTIONS MILESTONE UNLOCKED! ✨\nTalk to me again for the Counting lessons!\"\n";
        EditorUtility.SetDirty(Klara_1_succ);
        AddChoice(Klara_1, "Say: \"wait here → uray ditoy\"", Klara_1_succ);
        Klara_prev = Klara_1_succ;

        InteractableNPC[] Klara_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Klara_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Klara", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Klara_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Klara ---
        {
        DialogueNode Klara_start = null;
        DialogueNode Klara_prev = null;
        DialogueNode Klara_0 = GetOrCreateNode(outputFolder + "/Klara_0.asset");
        Klara_0.speakerName = "KLARA";
        Klara_0.dialogueText = "";
        EditorUtility.SetDirty(Klara_0);
        Klara_start = Klara_0;
        Klara_prev = Klara_0;
        DialogueNode Klara_1 = GetOrCreateNode(outputFolder + "/Klara_1.asset");
        Klara_1.speakerName = "KLARA";
        Klara_1.dialogueText = "\"Let's begin counting in Ilokano.\nThe word 'maysa' means 'one.'\nYou use it when counting a single person or object.\nFor example, if you see one chest, you can say 'maysa.'\nListen carefully: maysa.\nNow, try saying 'maysa' yourself.\"\n";
        EditorUtility.SetDirty(Klara_1);
        AddChoice(Klara_prev, "Continue", Klara_1);
        Klara_prev = Klara_1;
        DialogueNode Klara_1_succ = GetOrCreateNode(outputFolder + "/Klara_1_succ.asset");
        Klara_1_succ.speakerName = "KLARA";
        Klara_1_succ.dialogueText = "\"Maysa! Just one rare chest!\"\n";
        EditorUtility.SetDirty(Klara_1_succ);
        AddChoice(Klara_1, "Say: \"one → maysa\"", Klara_1_succ);
        Klara_prev = Klara_1_succ;
        DialogueNode Klara_2 = GetOrCreateNode(outputFolder + "/Klara_2.asset");
        Klara_2.speakerName = "KLARA";
        Klara_2.dialogueText = "\"When you count two objects, people, or things, Ilokano uses 'dua.'\nIt means 'two.'\nListen carefully: dua.\nNow, try saying 'dua' yourself.\"\n";
        EditorUtility.SetDirty(Klara_2);
        AddChoice(Klara_prev, "Continue", Klara_2);
        Klara_prev = Klara_2;
        DialogueNode Klara_2_succ = GetOrCreateNode(outputFolder + "/Klara_2_succ.asset");
        Klara_2_succ.speakerName = "KLARA";
        Klara_2_succ.dialogueText = "\"Dua! Two matching chairs!\"\n";
        EditorUtility.SetDirty(Klara_2_succ);
        AddChoice(Klara_2, "Say: \"two → dua\"", Klara_2_succ);
        Klara_prev = Klara_2_succ;
        DialogueNode Klara_3 = GetOrCreateNode(outputFolder + "/Klara_3.asset");
        Klara_3.speakerName = "KLARA";
        Klara_3.dialogueText = "\"The next number is 'tallo.'\nIn Ilokano, 'tallo' means 'three.'\nFor example, you can use it when counting three objects.\nListen carefully: tallo.\nNow, try saying 'tallo' yourself.\"\n";
        EditorUtility.SetDirty(Klara_3);
        AddChoice(Klara_prev, "Continue", Klara_3);
        Klara_prev = Klara_3;
        DialogueNode Klara_3_succ = GetOrCreateNode(outputFolder + "/Klara_3_succ.asset");
        Klara_3_succ.speakerName = "KLARA";
        Klara_3_succ.dialogueText = "\"Tallo! Three oil lamps!\nHead over to Tala the Bagnet seller for more numbers!\"\n";
        EditorUtility.SetDirty(Klara_3_succ);
        AddChoice(Klara_3, "Say: \"three → tallo\"", Klara_3_succ);
        Klara_prev = Klara_3_succ;

        InteractableNPC[] Klara_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Klara_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Klara", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Klara_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Tala ---
        {
        DialogueNode Tala_start = null;
        DialogueNode Tala_prev = null;
        DialogueNode Tala_0 = GetOrCreateNode(outputFolder + "/Tala_0.asset");
        Tala_0.speakerName = "TALA";
        Tala_0.dialogueText = "";
        EditorUtility.SetDirty(Tala_0);
        Tala_start = Tala_0;
        Tala_prev = Tala_0;
        DialogueNode Tala_1 = GetOrCreateNode(outputFolder + "/Tala_1.asset");
        Tala_1.speakerName = "TALA";
        Tala_1.dialogueText = "\"Let's keep counting.\nIn Ilokano, 'uppat' means 'four.'\nYou can use it when counting four objects, such as four pieces of Bagnet.\nListen carefully: uppat.\nNow, try saying 'uppat' yourself.\"\n";
        EditorUtility.SetDirty(Tala_1);
        AddChoice(Tala_prev, "Continue", Tala_1);
        Tala_prev = Tala_1;
        DialogueNode Tala_1_succ = GetOrCreateNode(outputFolder + "/Tala_1_succ.asset");
        Tala_1_succ.speakerName = "TALA";
        Tala_1_succ.dialogueText = "\"Uppat! Four crispy slabs!\"\n";
        EditorUtility.SetDirty(Tala_1_succ);
        AddChoice(Tala_1, "Say: \"four → uppat\"", Tala_1_succ);
        Tala_prev = Tala_1_succ;
        DialogueNode Tala_2 = GetOrCreateNode(outputFolder + "/Tala_2.asset");
        Tala_2.speakerName = "TALA";
        Tala_2.dialogueText = "\"The next number is 'lima.'\nIn Ilokano, 'lima' means 'five.'\nListen carefully: lima.\nNow, try saying 'lima' yourself.\"\n";
        EditorUtility.SetDirty(Tala_2);
        AddChoice(Tala_prev, "Continue", Tala_2);
        Tala_prev = Tala_2;
        DialogueNode Tala_2_succ = GetOrCreateNode(outputFolder + "/Tala_2_succ.asset");
        Tala_2_succ.speakerName = "TALA";
        Tala_2_succ.dialogueText = "\"Lima! Five kilos of delicious Bagnet!\"\n";
        EditorUtility.SetDirty(Tala_2_succ);
        AddChoice(Tala_2, "Say: \"five → lima\"", Tala_2_succ);
        Tala_prev = Tala_2_succ;
        DialogueNode Tala_3 = GetOrCreateNode(outputFolder + "/Tala_3.asset");
        Tala_3.speakerName = "TALA";
        Tala_3.dialogueText = "\"In Ilokano, the number 'six' is 'innem.'\nListen carefully: innem.\nNow, try saying 'innem' yourself.\"\n";
        EditorUtility.SetDirty(Tala_3);
        AddChoice(Tala_prev, "Continue", Tala_3);
        Tala_prev = Tala_3;
        DialogueNode Tala_3_succ = GetOrCreateNode(outputFolder + "/Tala_3_succ.asset");
        Tala_3_succ.speakerName = "TALA";
        Tala_3_succ.dialogueText = "\"Innem! Six family recipes!\"\n";
        EditorUtility.SetDirty(Tala_3_succ);
        AddChoice(Tala_3, "Say: \"six → innem\"", Tala_3_succ);
        Tala_prev = Tala_3_succ;
        DialogueNode Tala_4 = GetOrCreateNode(outputFolder + "/Tala_4.asset");
        Tala_4.speakerName = "TALA";
        Tala_4.dialogueText = "\"The Ilokano word for 'seven' is 'pito.'\nListen carefully: pito.\nNow, try saying 'pito' yourself.\"\n";
        EditorUtility.SetDirty(Tala_4);
        AddChoice(Tala_prev, "Continue", Tala_4);
        Tala_prev = Tala_4;
        DialogueNode Tala_4_succ = GetOrCreateNode(outputFolder + "/Tala_4_succ.asset");
        Tala_4_succ.speakerName = "TALA";
        Tala_4_succ.dialogueText = "\"Pito! Seven!\nRun to Mang Lance the Kalesa driver to finish counting!\"\n";
        EditorUtility.SetDirty(Tala_4_succ);
        AddChoice(Tala_4, "Say: \"seven → pito\"", Tala_4_succ);
        Tala_prev = Tala_4_succ;

        InteractableNPC[] Tala_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Tala_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Tala", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Tala_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: MangLance ---
        {
        DialogueNode MangLance_start = null;
        DialogueNode MangLance_prev = null;
        DialogueNode MangLance_0 = GetOrCreateNode(outputFolder + "/MangLance_0.asset");
        MangLance_0.speakerName = "MANG LANCE";
        MangLance_0.dialogueText = "\"Whoa, hold on! My carriage wheel pin popped out!\nPlease find the wheel pin so my horse Barnaby and I can ride safely!\"\n";
        EditorUtility.SetDirty(MangLance_0);
        MangLance_start = MangLance_0;
        MangLance_prev = MangLance_0;
        DialogueNode MangLance_1 = GetOrCreateNode(outputFolder + "/MangLance_1.asset");
        MangLance_1.speakerName = "MANG LANCE";
        MangLance_1.dialogueText = "\"Whew! Thanks for fixing my wheel!\nLet's continue counting.\nIn Ilokano, 'walo' means 'eight.'\nListen carefully: walo.\nNow, try saying 'walo' yourself.\"\n";
        EditorUtility.SetDirty(MangLance_1);
        AddChoice(MangLance_prev, "Continue", MangLance_1);
        MangLance_prev = MangLance_1;
        DialogueNode MangLance_1_succ = GetOrCreateNode(outputFolder + "/MangLance_1_succ.asset");
        MangLance_1_succ.speakerName = "MANG LANCE";
        MangLance_1_succ.dialogueText = "\"Walo! Eight treats for Barnaby!\"\n";
        EditorUtility.SetDirty(MangLance_1_succ);
        AddChoice(MangLance_1, "Say: \"eight → walo\"", MangLance_1_succ);
        MangLance_prev = MangLance_1_succ;
        DialogueNode MangLance_2 = GetOrCreateNode(outputFolder + "/MangLance_2.asset");
        MangLance_2.speakerName = "MANG LANCE";
        MangLance_2.dialogueText = "\"The next number is 'siam.'\nIn Ilokano, 'siam' means 'nine.'\nListen carefully: siam.\nNow, try saying 'siam' yourself.\"\n";
        EditorUtility.SetDirty(MangLance_2);
        AddChoice(MangLance_prev, "Continue", MangLance_2);
        MangLance_prev = MangLance_2;
        DialogueNode MangLance_2_succ = GetOrCreateNode(outputFolder + "/MangLance_2_succ.asset");
        MangLance_2_succ.speakerName = "MANG LANCE";
        MangLance_2_succ.dialogueText = "\"Siam! Nine! You're almost at the end of the count!\"\n";
        EditorUtility.SetDirty(MangLance_2_succ);
        AddChoice(MangLance_2, "Say: \"nine → siam\"", MangLance_2_succ);
        MangLance_prev = MangLance_2_succ;
        DialogueNode MangLance_3 = GetOrCreateNode(outputFolder + "/MangLance_3.asset");
        MangLance_3.speakerName = "MANG LANCE";
        MangLance_3.dialogueText = "\"And now, we've reached ten!\nIn Ilokano, 'sangapulo' means 'ten.'\nListen carefully: sangapulo.\nNow, try saying 'sangapulo' yourself.\"\n";
        EditorUtility.SetDirty(MangLance_3);
        AddChoice(MangLance_prev, "Continue", MangLance_3);
        MangLance_prev = MangLance_3;
        DialogueNode MangLance_3_succ = GetOrCreateNode(outputFolder + "/MangLance_3_succ.asset");
        MangLance_3_succ.speakerName = "MANG LANCE";
        MangLance_3_succ.dialogueText = "\"Sangapulo! You made it to ten!\n🏆 LEVEL II: FUNCTIONAL & NAVIGATIONAL COMPLETE!";
        EditorUtility.SetDirty(MangLance_3_succ);
        AddChoice(MangLance_3, "Say: \"ten → sangapulo\"", MangLance_3_succ);
        DialogueNode MangLance_3_succ_part1 = GetOrCreateNode(outputFolder + "/MangLance_3_succ_part1.asset");
        MangLance_3_succ_part1.speakerName = "MANG LANCE";
        MangLance_3_succ_part1.dialogueText = "🏆\nYou've learned how to ask for help, navigate the streets, give directions, and use numbers in everyday situations.\nNow let's explore how Ilokano describes actions, people, and ideas.\nWelcome to Level III: Grammatical Foundations!\"\n";
        EditorUtility.SetDirty(MangLance_3_succ_part1);
        AddChoice(MangLance_3_succ, "Continue", MangLance_3_succ_part1);
        MangLance_prev = MangLance_3_succ_part1;

        InteractableNPC[] MangLance_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in MangLance_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("MangLance", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = MangLance_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Rayo ---
        {
        DialogueNode Rayo_start = null;
        DialogueNode Rayo_prev = null;
        DialogueNode Rayo_0 = GetOrCreateNode(outputFolder + "/Rayo_0.asset");
        Rayo_0.speakerName = "RAYO";
        Rayo_0.dialogueText = "";
        EditorUtility.SetDirty(Rayo_0);
        Rayo_start = Rayo_0;
        Rayo_prev = Rayo_0;
        DialogueNode Rayo_1 = GetOrCreateNode(outputFolder + "/Rayo_1.asset");
        Rayo_1.speakerName = "RAYO";
        Rayo_1.dialogueText = "\"After all that walking, you'll probably be hungry!\nIn Ilokano, 'mangan' means 'eat.'\nIt is an action verb used when talking about the act of eating.\nFor example, when it's time for lunch, you can say 'mangan.'\nListen carefully: mangan.\nNow, try saying 'mangan' yourself.\"\n";
        EditorUtility.SetDirty(Rayo_1);
        AddChoice(Rayo_prev, "Continue", Rayo_1);
        Rayo_prev = Rayo_1;
        DialogueNode Rayo_1_succ = GetOrCreateNode(outputFolder + "/Rayo_1_succ.asset");
        Rayo_1_succ.speakerName = "RAYO";
        Rayo_1_succ.dialogueText = "\"Mangan! Now you're making me hungry!\nGo see Rayo the photographer for more action verbs!\"\n";
        EditorUtility.SetDirty(Rayo_1_succ);
        AddChoice(Rayo_1, "Say: \"eat → mangan\"", Rayo_1_succ);
        Rayo_prev = Rayo_1_succ;

        InteractableNPC[] Rayo_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Rayo_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Rayo", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Rayo_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Rayo ---
        {
        DialogueNode Rayo_start = null;
        DialogueNode Rayo_prev = null;
        DialogueNode Rayo_0 = GetOrCreateNode(outputFolder + "/Rayo_0.asset");
        Rayo_0.speakerName = "RAYO";
        Rayo_0.dialogueText = "";
        EditorUtility.SetDirty(Rayo_0);
        Rayo_start = Rayo_0;
        Rayo_prev = Rayo_0;
        DialogueNode Rayo_1 = GetOrCreateNode(outputFolder + "/Rayo_1.asset");
        Rayo_1.speakerName = "RAYO";
        Rayo_1.dialogueText = "\"Click! Great pose!\nIt's warm today, and you might want something refreshing.\nIn Ilokano, 'uminom' means 'drink.'\nIt is an action verb used when talking about drinking something.\nListen carefully: uminom.\nNow, try saying 'uminom' yourself.\"\n";
        EditorUtility.SetDirty(Rayo_1);
        AddChoice(Rayo_prev, "Continue", Rayo_1);
        Rayo_prev = Rayo_1;
        DialogueNode Rayo_1_succ = GetOrCreateNode(outputFolder + "/Rayo_1_succ.asset");
        Rayo_1_succ.speakerName = "RAYO";
        Rayo_1_succ.dialogueText = "\"Uminom! Refreshing!\"\n";
        EditorUtility.SetDirty(Rayo_1_succ);
        AddChoice(Rayo_1, "Say: \"drink → uminom\"", Rayo_1_succ);
        Rayo_prev = Rayo_1_succ;
        DialogueNode Rayo_2 = GetOrCreateNode(outputFolder + "/Rayo_2.asset");
        Rayo_2.speakerName = "RAYO";
        Rayo_2.dialogueText = "\"When you want to talk about moving from one place to another, you can use the verb 'mapan.'\nIt means 'go.'\nFor example, if you're heading toward a scenic photo spot, you can say 'mapan.'\nListen carefully: mapan.\nNow, try saying 'mapan' yourself.\"\n";
        EditorUtility.SetDirty(Rayo_2);
        AddChoice(Rayo_prev, "Continue", Rayo_2);
        Rayo_prev = Rayo_2;
        DialogueNode Rayo_2_succ = GetOrCreateNode(outputFolder + "/Rayo_2_succ.asset");
        Rayo_2_succ.speakerName = "RAYO";
        Rayo_2_succ.dialogueText = "\"Mapan! Let's head over to the arch!\"\n";
        EditorUtility.SetDirty(Rayo_2_succ);
        AddChoice(Rayo_2, "Say: \"go → mapan\"", Rayo_2_succ);
        Rayo_prev = Rayo_2_succ;
        DialogueNode Rayo_3 = GetOrCreateNode(outputFolder + "/Rayo_3.asset");
        Rayo_3.speakerName = "RAYO";
        Rayo_3.dialogueText = "\"Now let's learn the opposite direction.\nIn Ilokano, 'umay' means 'come.'\nYou can use it when asking someone to move toward you or toward a particular place.\nListen carefully: umay.\nNow, try saying 'umay' yourself.\"\n";
        EditorUtility.SetDirty(Rayo_3);
        AddChoice(Rayo_prev, "Continue", Rayo_3);
        Rayo_prev = Rayo_3;
        DialogueNode Rayo_3_succ = GetOrCreateNode(outputFolder + "/Rayo_3_succ.asset");
        Rayo_3_succ.speakerName = "RAYO";
        Rayo_3_succ.dialogueText = "\"Umay! Come stand right by the window!\"\n";
        EditorUtility.SetDirty(Rayo_3_succ);
        AddChoice(Rayo_3, "Say: \"come → umay\"", Rayo_3_succ);
        Rayo_prev = Rayo_3_succ;
        DialogueNode Rayo_4 = GetOrCreateNode(outputFolder + "/Rayo_4.asset");
        Rayo_4.speakerName = "RAYO";
        Rayo_4.dialogueText = "\"After exploring all day, everyone needs time to rest.\nIn Ilokano, 'maturog' means 'sleep.'\nIt is the action of resting while asleep.\nListen carefully: maturog.\nNow, try saying 'maturog' yourself.\"\n";
        EditorUtility.SetDirty(Rayo_4);
        AddChoice(Rayo_prev, "Continue", Rayo_4);
        Rayo_prev = Rayo_4;
        DialogueNode Rayo_4_succ = GetOrCreateNode(outputFolder + "/Rayo_4_succ.asset");
        Rayo_4_succ.speakerName = "RAYO";
        Rayo_4_succ.dialogueText = "\"Maturog well!\nCatch up with Aling Rosa to wrap up Action Verbs!\"\n";
        EditorUtility.SetDirty(Rayo_4_succ);
        AddChoice(Rayo_4, "Say: \"sleep → maturog\"", Rayo_4_succ);
        Rayo_prev = Rayo_4_succ;

        InteractableNPC[] Rayo_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Rayo_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Rayo", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Rayo_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: AlingRosa ---
        {
        DialogueNode AlingRosa_start = null;
        DialogueNode AlingRosa_prev = null;
        DialogueNode AlingRosa_0 = GetOrCreateNode(outputFolder + "/AlingRosa_0.asset");
        AlingRosa_0.speakerName = "ALING ROSA";
        AlingRosa_0.dialogueText = "\"Ay, my thread broke!\nPlease find some thread so I can finish weaving these colorful souvenirs!\"\n";
        EditorUtility.SetDirty(AlingRosa_0);
        AlingRosa_start = AlingRosa_0;
        AlingRosa_prev = AlingRosa_0;
        DialogueNode AlingRosa_1 = GetOrCreateNode(outputFolder + "/AlingRosa_1.asset");
        AlingRosa_1.speakerName = "ALING ROSA";
        AlingRosa_1.dialogueText = "\"Aww, thank you!\nWhen you look at something or notice something with your eyes, the Ilokano verb 'makita' means 'see.'\nFor example, you can use it when looking at these bright textiles.\nListen carefully: makita.\nNow, try saying 'makita' yourself.\"\n";
        EditorUtility.SetDirty(AlingRosa_1);
        AddChoice(AlingRosa_prev, "Continue", AlingRosa_1);
        AlingRosa_prev = AlingRosa_1;
        DialogueNode AlingRosa_1_succ = GetOrCreateNode(outputFolder + "/AlingRosa_1_succ.asset");
        AlingRosa_1_succ.speakerName = "ALING ROSA";
        AlingRosa_1_succ.dialogueText = "\"Makita! Look at these lovely patterns!\"\n";
        EditorUtility.SetDirty(AlingRosa_1_succ);
        AddChoice(AlingRosa_1, "Say: \"see → makita\"", AlingRosa_1_succ);
        AlingRosa_prev = AlingRosa_1_succ;
        DialogueNode AlingRosa_2 = GetOrCreateNode(outputFolder + "/AlingRosa_2.asset");
        AlingRosa_2.speakerName = "ALING ROSA";
        AlingRosa_2.dialogueText = "\"Listen closely. Can you hear the horse hooves clattering down Calle Crisologo?\nIn Ilokano, 'mangngeg' means 'hear.'\nListen carefully: mangngeg.\nNow, try saying 'mangngeg' yourself.\"\n";
        EditorUtility.SetDirty(AlingRosa_2);
        AddChoice(AlingRosa_prev, "Continue", AlingRosa_2);
        AlingRosa_prev = AlingRosa_2;
        DialogueNode AlingRosa_2_succ = GetOrCreateNode(outputFolder + "/AlingRosa_2_succ.asset");
        AlingRosa_2_succ.speakerName = "ALING ROSA";
        AlingRosa_2_succ.dialogueText = "\"Mangngeg! You can hear the kalesas coming!\"\n";
        EditorUtility.SetDirty(AlingRosa_2_succ);
        AddChoice(AlingRosa_2, "Say: \"hear → mangngeg\"", AlingRosa_2_succ);
        AlingRosa_prev = AlingRosa_2_succ;
        DialogueNode AlingRosa_3 = GetOrCreateNode(outputFolder + "/AlingRosa_3.asset");
        AlingRosa_3.speakerName = "ALING ROSA";
        AlingRosa_3.dialogueText = "\"You're doing something very important right now-you are using your voice.\nIn Ilokano, 'agsao' means 'speak.'\nYou can use it when talking about speaking with another person or practicing a language.\nListen carefully: agsao.\nNow, try saying 'agsao' yourself.\"\n";
        EditorUtility.SetDirty(AlingRosa_3);
        AddChoice(AlingRosa_prev, "Continue", AlingRosa_3);
        AlingRosa_prev = AlingRosa_3;
        DialogueNode AlingRosa_3_succ = GetOrCreateNode(outputFolder + "/AlingRosa_3_succ.asset");
        AlingRosa_3_succ.speakerName = "ALING ROSA";
        AlingRosa_3_succ.dialogueText = "\"Agsao!\n✨ ACTION VERBS MILESTONE UNLOCKED! ✨\nStay here for Linking Verbs!\"\n";
        EditorUtility.SetDirty(AlingRosa_3_succ);
        AddChoice(AlingRosa_3, "Say: \"speak → agsao\"", AlingRosa_3_succ);
        AlingRosa_prev = AlingRosa_3_succ;
        DialogueNode AlingRosa_4 = GetOrCreateNode(outputFolder + "/AlingRosa_4.asset");
        AlingRosa_4.speakerName = "ALING ROSA";
        SplitDialogueNode(AlingRosa_4, "\"Now we're moving into a more advanced part of Ilokano.\nIn a sentence such as 'Siak ket weaver,' the word 'ket' connects the subject with the information that describes it.\nThink of 'ket' as a connector that helps link the two parts of the sentence.\nListen carefully: ket.\nNow, try saying 'ket' yourself.\"\n", outputFolder, "AlingRosa_4");
        EditorUtility.SetDirty(AlingRosa_4);
        AddChoice(AlingRosa_prev, "Continue", AlingRosa_4);
        AlingRosa_prev = AlingRosa_4;
        DialogueNode AlingRosa_4_succ = GetOrCreateNode(outputFolder + "/AlingRosa_4_succ.asset");
        AlingRosa_4_succ.speakerName = "ALING ROSA";
        AlingRosa_4_succ.dialogueText = "\"Ket! Spot on!\nGo see Lola Nida for more connector words!\"\n";
        EditorUtility.SetDirty(AlingRosa_4_succ);
        AddChoice(AlingRosa_4, "Say: \"am / subject connector → ket\"", AlingRosa_4_succ);
        AlingRosa_prev = AlingRosa_4_succ;

        InteractableNPC[] AlingRosa_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in AlingRosa_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("AlingRosa", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = AlingRosa_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: LolaNida ---
        {
        DialogueNode LolaNida_start = null;
        DialogueNode LolaNida_prev = null;
        DialogueNode LolaNida_0 = GetOrCreateNode(outputFolder + "/LolaNida_0.asset");
        LolaNida_0.speakerName = "LOLA NIDA";
        LolaNida_0.dialogueText = "";
        EditorUtility.SetDirty(LolaNida_0);
        LolaNida_start = LolaNida_0;
        LolaNida_prev = LolaNida_0;
        DialogueNode LolaNida_1 = GetOrCreateNode(outputFolder + "/LolaNida_1.asset");
        LolaNida_1.speakerName = "LOLA NIDA";
        LolaNida_1.dialogueText = "\"When identifying or describing something specific, Ilokano can use 'isu ti' as part of the sentence structure.\nFor example, it can help express an idea similar to 'is the' in English.\nListen carefully: isu ti.\nNow, try saying 'isu ti' yourself.\"\n";
        EditorUtility.SetDirty(LolaNida_1);
        AddChoice(LolaNida_prev, "Continue", LolaNida_1);
        LolaNida_prev = LolaNida_1;
        DialogueNode LolaNida_1_succ = GetOrCreateNode(outputFolder + "/LolaNida_1_succ.asset");
        LolaNida_1_succ.speakerName = "LOLA NIDA";
        LolaNida_1_succ.dialogueText = "\"Isu ti! You're learning how Ilokano connects ideas!\"\n";
        EditorUtility.SetDirty(LolaNida_1_succ);
        AddChoice(LolaNida_1, "Say: \"is / specific marker → isu ti\"", LolaNida_1_succ);
        LolaNida_prev = LolaNida_1_succ;
        DialogueNode LolaNida_2 = GetOrCreateNode(outputFolder + "/LolaNida_2.asset");
        LolaNida_2.speakerName = "LOLA NIDA";
        LolaNida_2.dialogueText = "\"When talking about a group of people, Ilokano can use 'da' as a plural marker.\nFor example, when referring to a group such as the weavers, 'da' can help indicate that you're talking about more than one person.\nListen carefully: da.\nNow, try saying 'da' yourself.\"\n";
        EditorUtility.SetDirty(LolaNida_2);
        AddChoice(LolaNida_prev, "Continue", LolaNida_2);
        LolaNida_prev = LolaNida_2;
        DialogueNode LolaNida_2_succ = GetOrCreateNode(outputFolder + "/LolaNida_2_succ.asset");
        LolaNida_2_succ.speakerName = "LOLA NIDA";
        LolaNida_2_succ.dialogueText = "\"Da! You're learning how Ilokano expresses groups!\"\n";
        EditorUtility.SetDirty(LolaNida_2_succ);
        AddChoice(LolaNida_2, "Say: \"plural subject marker → da\"", LolaNida_2_succ);
        LolaNida_prev = LolaNida_2_succ;
        DialogueNode LolaNida_3 = GetOrCreateNode(outputFolder + "/LolaNida_3.asset");
        LolaNida_3.speakerName = "LOLA NIDA";
        SplitDialogueNode(LolaNida_3, "\"When talking about something that existed or was true in the past, you can use 'ket idi' in a historical context.\nFor example, when describing how a place was in the past, this phrase can help connect the subject with its former state.\nListen carefully: ket idi.\nNow, try saying 'ket idi' yourself.\"\n", outputFolder, "LolaNida_3");
        EditorUtility.SetDirty(LolaNida_3);
        AddChoice(LolaNida_prev, "Continue", LolaNida_3);
        LolaNida_prev = LolaNida_3;
        DialogueNode LolaNida_3_succ = GetOrCreateNode(outputFolder + "/LolaNida_3_succ.asset");
        LolaNida_3_succ.speakerName = "LOLA NIDA";
        LolaNida_3_succ.dialogueText = "\"Ket idi! You're connecting language with the history of Ilocos!\"\n";
        EditorUtility.SetDirty(LolaNida_3_succ);
        AddChoice(LolaNida_3, "Say: \"was / past state → ket idi\"", LolaNida_3_succ);
        LolaNida_prev = LolaNida_3_succ;
        DialogueNode LolaNida_4 = GetOrCreateNode(outputFolder + "/LolaNida_4.asset");
        LolaNida_4.speakerName = "LOLA NIDA";
        LolaNida_4.dialogueText = "\"The same past expression can also appear when talking about a group in a historical context.\nFor example, when describing what people were like in the past, 'ket idi' can be part of the sentence structure.\nListen carefully: ket idi.\nNow, try saying 'ket idi' yourself.\"\n";
        EditorUtility.SetDirty(LolaNida_4);
        AddChoice(LolaNida_prev, "Continue", LolaNida_4);
        LolaNida_prev = LolaNida_4;
        DialogueNode LolaNida_4_succ = GetOrCreateNode(outputFolder + "/LolaNida_4_succ.asset");
        LolaNida_4_succ.speakerName = "LOLA NIDA";
        LolaNida_4_succ.dialogueText = "\"Ket idi! The past lives on through the stories we tell!\"\n";
        EditorUtility.SetDirty(LolaNida_4_succ);
        AddChoice(LolaNida_4, "Say: \"were / plural past state → ket idi\"", LolaNida_4_succ);
        LolaNida_prev = LolaNida_4_succ;

        InteractableNPC[] LolaNida_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in LolaNida_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("LolaNida", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = LolaNida_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: Neneng ---
        {
        DialogueNode Neneng_start = null;
        DialogueNode Neneng_prev = null;
        DialogueNode Neneng_0 = GetOrCreateNode(outputFolder + "/Neneng_0.asset");
        Neneng_0.speakerName = "NENENG";
        Neneng_0.dialogueText = "";
        EditorUtility.SetDirty(Neneng_0);
        Neneng_start = Neneng_0;
        Neneng_prev = Neneng_0;
        DialogueNode Neneng_1 = GetOrCreateNode(outputFolder + "/Neneng_1.asset");
        Neneng_1.speakerName = "NENENG";
        Neneng_1.dialogueText = "\"Hey!";
        EditorUtility.SetDirty(Neneng_1);
        AddChoice(Neneng_prev, "Continue", Neneng_1);
        DialogueNode Neneng_1_part1 = GetOrCreateNode(outputFolder + "/Neneng_1_part1.asset");
        Neneng_1_part1.speakerName = "NENENG";
        Neneng_1_part1.dialogueText = "Here's an interesting one.\nIn Ilokano, 'agbalin' means 'become.'\nIt describes a change from one state or condition into another.\nFor example, with practice, you can become more confident in speaking Ilokano.\nListen carefully: agbalin.\nNow, try saying 'agbalin' yourself.\"\n";
        EditorUtility.SetDirty(Neneng_1_part1);
        AddChoice(Neneng_1, "Continue", Neneng_1_part1);
        Neneng_prev = Neneng_1_part1;
        DialogueNode Neneng_1_succ = GetOrCreateNode(outputFolder + "/Neneng_1_succ.asset");
        Neneng_1_succ.speakerName = "NENENG";
        Neneng_1_succ.dialogueText = "\"Agbalin! With practice, you can become a confident Ilokano speaker!\"\n";
        EditorUtility.SetDirty(Neneng_1_succ);
        AddChoice(Neneng_1_part1, "Say: \"become → agbalin\"", Neneng_1_succ);
        Neneng_prev = Neneng_1_succ;
        DialogueNode Neneng_2 = GetOrCreateNode(outputFolder + "/Neneng_2.asset");
        Neneng_2.speakerName = "NENENG";
        Neneng_2.dialogueText = "\"Sometimes we want to describe how something appears or feels to us.\nIn Ilokano, 'kasla' can express the idea of 'seem' or 'like.'\nFor example, you might say something seems like a journey through time.\nListen carefully: kasla.\nNow, try saying 'kasla' yourself.\"\n";
        EditorUtility.SetDirty(Neneng_2);
        AddChoice(Neneng_prev, "Continue", Neneng_2);
        Neneng_prev = Neneng_2;
        DialogueNode Neneng_2_succ = GetOrCreateNode(outputFolder + "/Neneng_2_succ.asset");
        Neneng_2_succ.speakerName = "NENENG";
        Neneng_2_succ.dialogueText = "\"Kasla! It really does feel like traveling back through history!\"\n";
        EditorUtility.SetDirty(Neneng_2_succ);
        AddChoice(Neneng_2, "Say: \"seem / like → kasla\"", Neneng_2_succ);
        Neneng_prev = Neneng_2_succ;
        DialogueNode Neneng_3 = GetOrCreateNode(outputFolder + "/Neneng_3.asset");
        Neneng_3.speakerName = "NENENG";
        Neneng_3.dialogueText = "\"Some things change, but others continue to stay strong.\nIn Ilokano, 'agtalinaed' means 'remain.'\nYou can use it when talking about something continuing to exist or stay in a certain state.\nListen carefully: agtalinaed.\nNow, try saying 'agtalinaed' yourself.\"\n";
        EditorUtility.SetDirty(Neneng_3);
        AddChoice(Neneng_prev, "Continue", Neneng_3);
        Neneng_prev = Neneng_3;
        DialogueNode Neneng_3_succ = GetOrCreateNode(outputFolder + "/Neneng_3_succ.asset");
        Neneng_3_succ.speakerName = "NENENG";
        Neneng_3_succ.dialogueText = "\"Agtalinaed! May our culture remain strong for generations!\"\n";
        EditorUtility.SetDirty(Neneng_3_succ);
        AddChoice(Neneng_3, "Say: \"remain → agtalinaed\"", Neneng_3_succ);
        Neneng_prev = Neneng_3_succ;
        DialogueNode Neneng_4 = GetOrCreateNode(outputFolder + "/Neneng_4.asset");
        Neneng_4.speakerName = "NENENG";
        Neneng_4.dialogueText = "\"When talking about staying or residing in a place, Ilokano uses 'agyan.'\nYou can use it when talking about where someone stays or lives.\nListen carefully: agyan.\nNow, try saying 'agyan' yourself.\"\n";
        EditorUtility.SetDirty(Neneng_4);
        AddChoice(Neneng_prev, "Continue", Neneng_4);
        Neneng_prev = Neneng_4;
        DialogueNode Neneng_4_succ = GetOrCreateNode(outputFolder + "/Neneng_4_succ.asset");
        Neneng_4_succ.speakerName = "NENENG";
        Neneng_4_succ.dialogueText = "\"Agyan! I hope you stay in Vigan a little longer!\nGo see Aling Riza at the restaurant!\"\n";
        EditorUtility.SetDirty(Neneng_4_succ);
        AddChoice(Neneng_4, "Say: \"stay / reside → agyan\"", Neneng_4_succ);
        Neneng_prev = Neneng_4_succ;

        InteractableNPC[] Neneng_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in Neneng_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("Neneng", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = Neneng_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: AlingRiza ---
        {
        DialogueNode AlingRiza_start = null;
        DialogueNode AlingRiza_prev = null;
        DialogueNode AlingRiza_0 = GetOrCreateNode(outputFolder + "/AlingRiza_0.asset");
        AlingRiza_0.speakerName = "ALING RIZA";
        AlingRiza_0.dialogueText = "";
        EditorUtility.SetDirty(AlingRiza_0);
        AlingRiza_start = AlingRiza_0;
        AlingRiza_prev = AlingRiza_0;
        DialogueNode AlingRiza_1 = GetOrCreateNode(outputFolder + "/AlingRiza_1.asset");
        AlingRiza_1.speakerName = "ALING RIZA";
        AlingRiza_1.dialogueText = "\"Language is more than words.";
        EditorUtility.SetDirty(AlingRiza_1);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_1);
        DialogueNode AlingRiza_1_part1 = GetOrCreateNode(outputFolder + "/AlingRiza_1_part1.asset");
        AlingRiza_1_part1.speakerName = "ALING RIZA";
        AlingRiza_1_part1.dialogueText = "Sometimes, it is about what you experience.\nIn Ilokano, 'marikna' means 'feel' or 'perceive.'\nYou can use it when talking about experiencing something through your senses or emotions.\nListen carefully: marikna.\nNow, try saying 'marikna' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_1_part1);
        AddChoice(AlingRiza_1, "Continue", AlingRiza_1_part1);
        AlingRiza_prev = AlingRiza_1_part1;
        DialogueNode AlingRiza_1_succ = GetOrCreateNode(outputFolder + "/AlingRiza_1_succ.asset");
        AlingRiza_1_succ.speakerName = "ALING RIZA";
        AlingRiza_1_succ.dialogueText = "\"Marikna!\n✨ LINKING VERBS MILESTONE UNLOCKED! ✨\nTalk to me again for Pronouns!\"\n";
        EditorUtility.SetDirty(AlingRiza_1_succ);
        AddChoice(AlingRiza_1_part1, "Say: \"feel / perceive → marikna\"", AlingRiza_1_succ);
        AlingRiza_prev = AlingRiza_1_succ;
        DialogueNode AlingRiza_2 = GetOrCreateNode(outputFolder + "/AlingRiza_2.asset");
        AlingRiza_2.speakerName = "ALING RIZA";
        AlingRiza_2.dialogueText = "\"Let's learn words that help us talk about people.\nIn Ilokano, 'siak' means 'I.'\nYou use it when referring to yourself.\nListen carefully: siak.\nNow, try saying 'siak' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_2);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_2);
        AlingRiza_prev = AlingRiza_2;
        DialogueNode AlingRiza_2_succ = GetOrCreateNode(outputFolder + "/AlingRiza_2_succ.asset");
        AlingRiza_2_succ.speakerName = "ALING RIZA";
        AlingRiza_2_succ.dialogueText = "\"Siak! That's you!\"\n";
        EditorUtility.SetDirty(AlingRiza_2_succ);
        AddChoice(AlingRiza_2, "Say: \"i → siak\"", AlingRiza_2_succ);
        AlingRiza_prev = AlingRiza_2_succ;
        DialogueNode AlingRiza_3 = GetOrCreateNode(outputFolder + "/AlingRiza_3.asset");
        AlingRiza_3.speakerName = "ALING RIZA";
        AlingRiza_3.dialogueText = "\"When speaking directly to another person, Ilokano uses 'sika' for 'you.'\nListen carefully: sika.\nNow, try saying 'sika' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_3);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_3);
        AlingRiza_prev = AlingRiza_3;
        DialogueNode AlingRiza_3_succ = GetOrCreateNode(outputFolder + "/AlingRiza_3_succ.asset");
        AlingRiza_3_succ.speakerName = "ALING RIZA";
        AlingRiza_3_succ.dialogueText = "\"Sika! That's the word for the person you're speaking to!\"\n";
        EditorUtility.SetDirty(AlingRiza_3_succ);
        AddChoice(AlingRiza_3, "Say: \"you → sika\"", AlingRiza_3_succ);
        AlingRiza_prev = AlingRiza_3_succ;
        DialogueNode AlingRiza_4 = GetOrCreateNode(outputFolder + "/AlingRiza_4.asset");
        AlingRiza_4.speakerName = "ALING RIZA";
        AlingRiza_4.dialogueText = "\"When talking about a male person who is not the speaker or listener, Ilokano uses 'isuna' for 'he.'\nListen carefully: isuna.\nNow, try saying 'isuna' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_4);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_4);
        AlingRiza_prev = AlingRiza_4;
        DialogueNode AlingRiza_4_succ = GetOrCreateNode(outputFolder + "/AlingRiza_4_succ.asset");
        AlingRiza_4_succ.speakerName = "ALING RIZA";
        AlingRiza_4_succ.dialogueText = "\"Isuna! You're talking about someone else!\"\n";
        EditorUtility.SetDirty(AlingRiza_4_succ);
        AddChoice(AlingRiza_4, "Say: \"he → isuna\"", AlingRiza_4_succ);
        AlingRiza_prev = AlingRiza_4_succ;
        DialogueNode AlingRiza_5 = GetOrCreateNode(outputFolder + "/AlingRiza_5.asset");
        AlingRiza_5.speakerName = "ALING RIZA";
        AlingRiza_5.dialogueText = "\"Interestingly, the same Ilokano pronoun 'isuna' can refer to 'she' as well.\nThe meaning depends on the person you're talking about and the context.\nListen carefully: isuna.\nNow, try saying 'isuna' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_5);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_5);
        AlingRiza_prev = AlingRiza_5;
        DialogueNode AlingRiza_5_succ = GetOrCreateNode(outputFolder + "/AlingRiza_5_succ.asset");
        AlingRiza_5_succ.speakerName = "ALING RIZA";
        AlingRiza_5_succ.dialogueText = "\"Isuna! Context helps us understand who we're talking about!\"\n";
        EditorUtility.SetDirty(AlingRiza_5_succ);
        AddChoice(AlingRiza_5, "Say: \"she → isuna\"", AlingRiza_5_succ);
        AlingRiza_prev = AlingRiza_5_succ;
        DialogueNode AlingRiza_6 = GetOrCreateNode(outputFolder + "/AlingRiza_6.asset");
        AlingRiza_6.speakerName = "ALING RIZA";
        AlingRiza_6.dialogueText = "\"When talking about yourself together with other people, you can use 'dakkami' for 'we' in the context taught here.\nListen carefully: dakkami.\nNow, try saying 'dakkami' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_6);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_6);
        AlingRiza_prev = AlingRiza_6;
        DialogueNode AlingRiza_6_succ = GetOrCreateNode(outputFolder + "/AlingRiza_6_succ.asset");
        AlingRiza_6_succ.speakerName = "ALING RIZA";
        AlingRiza_6_succ.dialogueText = "\"Dakkami! Now you're speaking as part of a group!\"\n";
        EditorUtility.SetDirty(AlingRiza_6_succ);
        AddChoice(AlingRiza_6, "Say: \"we → dakkami\"", AlingRiza_6_succ);
        AlingRiza_prev = AlingRiza_6_succ;
        DialogueNode AlingRiza_7 = GetOrCreateNode(outputFolder + "/AlingRiza_7.asset");
        AlingRiza_7.speakerName = "ALING RIZA";
        AlingRiza_7.dialogueText = "\"When talking about a group of other people, Ilokano uses 'isuda' for 'they.'\nListen carefully: isuda.\nNow, try saying 'isuda' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_7);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_7);
        AlingRiza_prev = AlingRiza_7;
        DialogueNode AlingRiza_7_succ = GetOrCreateNode(outputFolder + "/AlingRiza_7_succ.asset");
        AlingRiza_7_succ.speakerName = "ALING RIZA";
        AlingRiza_7_succ.dialogueText = "\"Isuda! You're talking about them!\"\n\"Ilokano can use 'siak' when referring to 'me' as well, depending on the sentence structure.\nListen carefully: siak.\nNow, try saying 'siak' yourself.\"\n\"Siak!";
        EditorUtility.SetDirty(AlingRiza_7_succ);
        AddChoice(AlingRiza_7, "Say: \"they → isuda\"", AlingRiza_7_succ);
        DialogueNode AlingRiza_7_succ_part1 = GetOrCreateNode(outputFolder + "/AlingRiza_7_succ_part1.asset");
        AlingRiza_7_succ_part1.speakerName = "ALING RIZA";
        AlingRiza_7_succ_part1.dialogueText = "You're getting comfortable with these pronouns!\"\n";
        EditorUtility.SetDirty(AlingRiza_7_succ_part1);
        AddChoice(AlingRiza_7_succ, "Continue", AlingRiza_7_succ_part1);
        AlingRiza_prev = AlingRiza_7_succ_part1;
        DialogueNode AlingRiza_8 = GetOrCreateNode(outputFolder + "/AlingRiza_8.asset");
        AlingRiza_8.speakerName = "ALING RIZA";
        AlingRiza_8.dialogueText = "\"When referring to yourself together with others, 'dakkami' can represent 'us' in the context we're learning.\nListen carefully: dakkami.\nNow, try saying 'dakkami' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_8);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_8);
        AlingRiza_prev = AlingRiza_8;
        DialogueNode AlingRiza_8_succ = GetOrCreateNode(outputFolder + "/AlingRiza_8_succ.asset");
        AlingRiza_8_succ.speakerName = "ALING RIZA";
        AlingRiza_8_succ.dialogueText = "\"Dakkami! Language helps us describe who belongs together!\"\n";
        EditorUtility.SetDirty(AlingRiza_8_succ);
        AddChoice(AlingRiza_8, "Say: \"us → dakkami\"", AlingRiza_8_succ);
        AlingRiza_prev = AlingRiza_8_succ;
        DialogueNode AlingRiza_9 = GetOrCreateNode(outputFolder + "/AlingRiza_9.asset");
        AlingRiza_9.speakerName = "ALING RIZA";
        AlingRiza_9.dialogueText = "\"When referring to other people as the object of an action, 'isuda' is used in the context we're learning for 'them.'\nListen carefully: isuda.\nNow, try saying 'isuda' yourself.\"\n";
        EditorUtility.SetDirty(AlingRiza_9);
        AddChoice(AlingRiza_prev, "Continue", AlingRiza_9);
        AlingRiza_prev = AlingRiza_9;
        DialogueNode AlingRiza_9_succ = GetOrCreateNode(outputFolder + "/AlingRiza_9_succ.asset");
        AlingRiza_9_succ.speakerName = "ALING RIZA";
        AlingRiza_9_succ.dialogueText = "\"Isuda! You've learned how Ilokano can talk about different people!\"\n";
        EditorUtility.SetDirty(AlingRiza_9_succ);
        AddChoice(AlingRiza_9, "Say: \"them → isuda\"", AlingRiza_9_succ);
        AlingRiza_prev = AlingRiza_9_succ;

        InteractableNPC[] AlingRiza_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in AlingRiza_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("AlingRiza", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = AlingRiza_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block
        // --- NPC: LolaBebang ---
        {
        DialogueNode LolaBebang_start = null;
        DialogueNode LolaBebang_prev = null;
        DialogueNode LolaBebang_0 = GetOrCreateNode(outputFolder + "/LolaBebang_0.asset");
        LolaBebang_0.speakerName = "LOLA BEBANG";
        LolaBebang_0.dialogueText = "";
        EditorUtility.SetDirty(LolaBebang_0);
        LolaBebang_start = LolaBebang_0;
        LolaBebang_prev = LolaBebang_0;
        DialogueNode LolaBebang_1 = GetOrCreateNode(outputFolder + "/LolaBebang_1.asset");
        LolaBebang_1.speakerName = "LOLA BEBANG";
        LolaBebang_1.dialogueText = "\"Questions help us learn about the world around us.\nIn Ilokano, 'ania' means 'what.'\nUse it when asking about an object, thing, or information.\nListen carefully: ania.\nNow, try saying 'ania' yourself.\"\n";
        EditorUtility.SetDirty(LolaBebang_1);
        AddChoice(LolaBebang_prev, "Continue", LolaBebang_1);
        LolaBebang_prev = LolaBebang_1;
        DialogueNode LolaBebang_1_succ = GetOrCreateNode(outputFolder + "/LolaBebang_1_succ.asset");
        LolaBebang_1_succ.speakerName = "LOLA BEBANG";
        LolaBebang_1_succ.dialogueText = "\"Ania! You're ready to ask questions!\"\n";
        EditorUtility.SetDirty(LolaBebang_1_succ);
        AddChoice(LolaBebang_1, "Say: \"what → ania\"", LolaBebang_1_succ);
        LolaBebang_prev = LolaBebang_1_succ;
        DialogueNode LolaBebang_2 = GetOrCreateNode(outputFolder + "/LolaBebang_2.asset");
        LolaBebang_2.speakerName = "LOLA BEBANG";
        LolaBebang_2.dialogueText = "\"When you want to know which person you are talking about, use 'asino.'\nIt means 'who.'\nListen carefully: asino.\nNow, try saying 'asino' yourself.\"\n";
        EditorUtility.SetDirty(LolaBebang_2);
        AddChoice(LolaBebang_prev, "Continue", LolaBebang_2);
        LolaBebang_prev = LolaBebang_2;
        DialogueNode LolaBebang_2_succ = GetOrCreateNode(outputFolder + "/LolaBebang_2_succ.asset");
        LolaBebang_2_succ.speakerName = "LOLA BEBANG";
        LolaBebang_2_succ.dialogueText = "\"Asino! Now you can ask who someone is!\"\n";
        EditorUtility.SetDirty(LolaBebang_2_succ);
        AddChoice(LolaBebang_2, "Say: \"who → asino\"", LolaBebang_2_succ);
        LolaBebang_prev = LolaBebang_2_succ;
        DialogueNode LolaBebang_3 = GetOrCreateNode(outputFolder + "/LolaBebang_3.asset");
        LolaBebang_3.speakerName = "LOLA BEBANG";
        LolaBebang_3.dialogueText = "\"When you need to find a place or location, you can ask 'where?'\nIn Ilokano, 'sadino' means 'where.'\nListen carefully: sadino.\nNow, try saying 'sadino' yourself.\"\n";
        EditorUtility.SetDirty(LolaBebang_3);
        AddChoice(LolaBebang_prev, "Continue", LolaBebang_3);
        LolaBebang_prev = LolaBebang_3;
        DialogueNode LolaBebang_3_succ = GetOrCreateNode(outputFolder + "/LolaBebang_3_succ.asset");
        LolaBebang_3_succ.speakerName = "LOLA BEBANG";
        LolaBebang_3_succ.dialogueText = "\"Sadino! Now you can ask where something is!\"\n";
        EditorUtility.SetDirty(LolaBebang_3_succ);
        AddChoice(LolaBebang_3, "Say: \"where → sadino\"", LolaBebang_3_succ);
        LolaBebang_prev = LolaBebang_3_succ;
        DialogueNode LolaBebang_4 = GetOrCreateNode(outputFolder + "/LolaBebang_4.asset");
        LolaBebang_4.speakerName = "LOLA BEBANG";
        LolaBebang_4.dialogueText = "\"When asking about time, you can use 'kaano.'\nIt means 'when.'\nListen carefully: kaano.\nNow, try saying 'kaano' yourself.\"\n";
        EditorUtility.SetDirty(LolaBebang_4);
        AddChoice(LolaBebang_prev, "Continue", LolaBebang_4);
        LolaBebang_prev = LolaBebang_4;
        DialogueNode LolaBebang_4_succ = GetOrCreateNode(outputFolder + "/LolaBebang_4_succ.asset");
        LolaBebang_4_succ.speakerName = "LOLA BEBANG";
        LolaBebang_4_succ.dialogueText = "\"Kaano! Now you can ask about when something happened!\"\n";
        EditorUtility.SetDirty(LolaBebang_4_succ);
        AddChoice(LolaBebang_4, "Say: \"when → kaano\"", LolaBebang_4_succ);
        LolaBebang_prev = LolaBebang_4_succ;
        DialogueNode LolaBebang_5 = GetOrCreateNode(outputFolder + "/LolaBebang_5.asset");
        LolaBebang_5.speakerName = "LOLA BEBANG";
        LolaBebang_5.dialogueText = "\"Sometimes we want to understand the reason behind something.\nIn Ilokano, 'apay' means 'why.'\nUse it when asking for a reason or explanation.\nListen carefully: apay.\nNow, try saying 'apay' yourself.\"\n";
        EditorUtility.SetDirty(LolaBebang_5);
        AddChoice(LolaBebang_prev, "Continue", LolaBebang_5);
        LolaBebang_prev = LolaBebang_5;
        DialogueNode LolaBebang_5_succ = GetOrCreateNode(outputFolder + "/LolaBebang_5_succ.asset");
        LolaBebang_5_succ.speakerName = "LOLA BEBANG";
        LolaBebang_5_succ.dialogueText = "\"Apay! Now you can ask why!\"\n";
        EditorUtility.SetDirty(LolaBebang_5_succ);
        AddChoice(LolaBebang_5, "Say: \"why → apay\"", LolaBebang_5_succ);
        LolaBebang_prev = LolaBebang_5_succ;
        DialogueNode LolaBebang_6 = GetOrCreateNode(outputFolder + "/LolaBebang_6.asset");
        LolaBebang_6.speakerName = "LOLA BEBANG";
        LolaBebang_6.dialogueText = "\"When you want to ask about the way something is done, use 'kasano.'\nIt means 'how.'\nListen carefully: kasano.\nNow, try saying 'kasano' yourself.\"\n";
        EditorUtility.SetDirty(LolaBebang_6);
        AddChoice(LolaBebang_prev, "Continue", LolaBebang_6);
        LolaBebang_prev = LolaBebang_6;
        DialogueNode LolaBebang_6_succ = GetOrCreateNode(outputFolder + "/LolaBebang_6_succ.asset");
        LolaBebang_6_succ.speakerName = "LOLA BEBANG";
        LolaBebang_6_succ.dialogueText = "\"Kasano! Look how far you've come!\"\n";
        EditorUtility.SetDirty(LolaBebang_6_succ);
        AddChoice(LolaBebang_6, "Say: \"how → kasano\"", LolaBebang_6_succ);
        LolaBebang_prev = LolaBebang_6_succ;
        DialogueNode LolaBebang_7 = GetOrCreateNode(outputFolder + "/LolaBebang_7.asset");
        LolaBebang_7.speakerName = "LOLA BEBANG";
        LolaBebang_7.dialogueText = "\"And now, you've reached another important question.\nWhen you want to ask about quantity, Ilokano uses 'mano' for 'how many.'\nYou can use it when asking about the number of people or objects.\nListen carefully: mano.\nNow, try saying 'mano' yourself.\"\n";
        EditorUtility.SetDirty(LolaBebang_7);
        AddChoice(LolaBebang_prev, "Continue", LolaBebang_7);
        LolaBebang_prev = LolaBebang_7;
        DialogueNode LolaBebang_7_succ = GetOrCreateNode(outputFolder + "/LolaBebang_7_succ.asset");
        LolaBebang_7_succ.speakerName = "LOLA BEBANG";
        LolaBebang_7_succ.dialogueText = "\"Mano!";
        EditorUtility.SetDirty(LolaBebang_7_succ);
        AddChoice(LolaBebang_7, "Say: \"how many → mano\"", LolaBebang_7_succ);
        DialogueNode LolaBebang_7_succ_part1 = GetOrCreateNode(outputFolder + "/LolaBebang_7_succ_part1.asset");
        LolaBebang_7_succ_part1.speakerName = "LOLA BEBANG";
        LolaBebang_7_succ_part1.dialogueText = "Now you can ask about quantity and numbers.\nYou've learned how to communicate in many everyday situations throughout your journey.\nBut learning doesn't end with repeating what you've heard.\nHead back to Kalaw at the plaza.\nYour final challenge awaits.\"\n";
        EditorUtility.SetDirty(LolaBebang_7_succ_part1);
        AddChoice(LolaBebang_7_succ, "Continue", LolaBebang_7_succ_part1);
        LolaBebang_prev = LolaBebang_7_succ_part1;

        InteractableNPC[] LolaBebang_npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in LolaBebang_npcs)
        {
            string n = npc.gameObject.name.Replace("_Rigged", "");
            if (n.Contains("LolaBebang", System.StringComparison.OrdinalIgnoreCase))
            {

                npc.defaultDialogue = LolaBebang_start;
                var so = new UnityEditor.SerializedObject(npc);
                var qdProp = so.FindProperty("questDialogues");
                if (qdProp != null)
                {
                    qdProp.ClearArray();
                    so.ApplyModifiedProperties();
                }

                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(npc);
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        } // End scope block

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"Successfully updated dialogues for {count} NPCs!");
    }
    
    private static DialogueNode GetOrCreateNode(string path)
    {
        DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        if (node == null)
        {
            node = ScriptableObject.CreateInstance<DialogueNode>();
            AssetDatabase.CreateAsset(node, path);
        }
        node.choices = new System.Collections.Generic.List<DialogueChoice>();
        node.endEventName = "";
        return node;
    }
    
    private static void AddChoice(DialogueNode node, string text, DialogueNode next)
    {
        if (node == null) return;
        node.choices.Add(new DialogueChoice { choiceText = text, nextNode = next });
        EditorUtility.SetDirty(node);
    }
    
    private static void SplitDialogueNode(DialogueNode startNode, string fullText, string outputFolder, string baseName)
    {
        string[] sentences = fullText.Split(new[] { '.', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<string> chunks = new System.Collections.Generic.List<string>();
        string currentChunk = "";

        foreach (string sentence in sentences)
        {
            string s = sentence.Trim() + ".";
            if (currentChunk.Length + s.Length > 250 && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.Trim());
                currentChunk = s + " ";
            }
            else
            {
                currentChunk += s + " ";
            }
        }
        if (currentChunk.Trim().Length > 0)
            chunks.Add(currentChunk.Trim());

        DialogueNode prevNode = null;

        for (int i = 0; i < chunks.Count; i++)
        {
            DialogueNode node;
            if (i == 0)
            {
                node = startNode;
            }
            else
            {
                string newPath = outputFolder + "/" + baseName + "_part" + i + ".asset";
                node = GetOrCreateNode(newPath);
                node.speakerName = startNode.speakerName;
            }

            node.dialogueText = chunks[i];
            EditorUtility.SetDirty(node);

            if (prevNode != null)
            {
                AddChoice(prevNode, "Continue", node);
            }
            prevNode = node;
        }
    }
}
