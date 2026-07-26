using UnityEngine;
using UnityEditor;

public class DialogueScriptUpdater : EditorWindow
{
    [MenuItem("Luminang/Generate Master Script Dialogues")]
    public static void GenerateDialogues()
    {
        string folderPath = "Assets/Dialogues/UpdatedMasterScript";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Dialogues"))
            {
                AssetDatabase.CreateFolder("Assets", "Dialogues");
            }
            AssetDatabase.CreateFolder("Assets/Dialogues", "UpdatedMasterScript");
        }

        int count = 0;

        // KALAW
        count += CreateNode(folderPath, "01_Kalaw_Initial", "Kalaw", "Squawk! Oh... is that a fruit in your hand? May I have some? Flying under this hot Vigan sun has completely worn me out...");
        count += CreateNode(folderPath, "02_Kalaw_PostQuest1", "Kalaw", "Mmm, sweet and juicy! Thank you so much, traveler!\n\nAh, where are my manners—I am Kalaw, your companion and guide through these lands!");
        count += CreateNode(folderPath, "02_Kalaw_PostQuest2", "Kalaw", "Wait a second... you carry an ancient anting-anting pendant.\n\nThat pendant is connected to the journey you've begun. Long ago, these lands were filled with voices, stories, and languages that carried the identity of their people.");
        count += CreateNode(folderPath, "02_Kalaw_PostQuest3", "Kalaw", "But something is causing those voices to fade.\n\nIf we're going to understand what happened, you'll need to explore Calle Crisologo, meet the people who live here, and learn how they speak in their everyday lives.");
        count += CreateNode(folderPath, "02_Kalaw_PostQuest4", "Kalaw", "Language isn't something you learn from a book alone.\n\nYou learn it by listening.\nBy speaking.\nBy meeting people.\nBy understanding their stories.");
        count += CreateNode(folderPath, "02_Kalaw_PostQuest5", "Kalaw", "There are 81 Ilokano words and expressions waiting for you to discover across this chapter.\n\nLet's begin with the absolute basics—learning how to greet the people you'll meet along the way.");

        // Word 1
        count += CreateNode(folderPath, "03_Kalaw_Word1_Teach1", "Kalaw", "Let's start with a simple word you'll use often when meeting someone.\n\nIn Ilokano, 'kumusta' means 'hello.' It is a friendly greeting you can use when you meet someone or begin a conversation.");
        count += CreateNode(folderPath, "03_Kalaw_Word1_Teach2", "Kalaw", "Listen carefully: kumusta.\n\nNow, try saying 'kumusta' yourself.");
        count += CreateNode(folderPath, "04_Kalaw_Word1_Success", "Kalaw", "Kumusta! Nailed it! You're ready to start meeting the people of Ilocos.");

        // Word 2
        count += CreateNode(folderPath, "05_Kalaw_Word2_Teach1", "Kalaw", "Once you've greeted someone, you can show more warmth by asking how they are.\n\nIn Ilokano, 'kumusta ka?' means 'how are you?' You can use it when checking on a friend, neighbor, or someone you've just met.");
        count += CreateNode(folderPath, "05_Kalaw_Word2_Teach2", "Kalaw", "Listen carefully: kumusta ka?\n\nNow, try saying 'kumusta ka?' yourself.");
        count += CreateNode(folderPath, "06_Kalaw_Word2_Success", "Kalaw", "Kumusta ka! Ah, see that smile? You're making friends already!");

        // Word 3
        count += CreateNode(folderPath, "07_Kalaw_Word3_Teach1", "Kalaw", "When someone asks you 'kumusta ka?', you can tell them how you're doing.\n\nIn Ilokano, 'nasayaat ak' means 'I'm fine' or 'I'm doing well.'");
        count += CreateNode(folderPath, "07_Kalaw_Word3_Teach2", "Kalaw", "For example, if someone asks how your journey is going, you can answer: 'Nasayaat ak.'\n\nListen carefully: nasayaat ak.\n\nNow, try saying 'nasayaat ak' yourself.");
        count += CreateNode(folderPath, "08_Kalaw_Word3_Success", "Kalaw", "Nasayaat ak! That's the spirit! Keep that energy up!");

        // Word 4
        count += CreateNode(folderPath, "09_Kalaw_Word4_Teach1", "Kalaw", "Look at the morning sun shining over the tiled roofs of Calle Crisologo.\n\nIn Ilokano, 'naimbag a bigat' means 'good morning.' You can use it when greeting someone earlier in the day.");
        count += CreateNode(folderPath, "09_Kalaw_Word4_Teach2", "Kalaw", "For example, you can use it when greeting vendors as they begin their morning.\n\nListen carefully: naimbag a bigat.\n\nNow, try saying 'naimbag a bigat' yourself.");
        count += CreateNode(folderPath, "10_Kalaw_Word4_Success", "Kalaw", "Naimbag a bigat! You've got the morning greeting down! Fly over to Vendor Kyros's souvenir stall for the next greetings!");

        // KYROS
        // Word 5
        count += CreateNode(folderPath, "11_Kyros_Word5_Teach1", "Kyros", "Naimbag nga aldaw, traveler! Kalaw told me a new adventurer was exploring Vigan.\n\nWhen the morning has passed and the afternoon arrives, Ilocanos can greet someone by saying 'naimbag a malem.' It means 'good afternoon.'");
        count += CreateNode(folderPath, "11_Kyros_Word5_Teach2", "Kyros", "You can use it when greeting someone during the afternoon.\n\nListen carefully: naimbag a malem.\n\nNow, try saying 'naimbag a malem' yourself.");
        count += CreateNode(folderPath, "12_Kyros_Word5_Success", "Kyros", "Naimbag a malem! Welcome to my shop, friend!");

        // Word 6
        count += CreateNode(folderPath, "13_Kyros_Word6_Teach", "Kyros", "As the sun sets and the streetlamps begin to glow, the greeting changes with the time of day.\n\nIn Ilokano, 'naimbag a rabii' means 'good evening.' Use it when greeting someone in the evening.\n\nListen carefully: naimbag a rabii.\n\nNow, try saying 'naimbag a rabii' yourself.");
        count += CreateNode(folderPath, "14_Kyros_Word6_Success", "Kyros", "Naimbag a rabii! You've learned another way to greet someone in Ilokano.");

        // Word 7
        count += CreateNode(folderPath, "15_Kyros_Word7_Teach", "Kyros", "Sometimes you want to give someone a general greeting without focusing on morning, afternoon, or evening.\n\nIn Ilokano, 'naimbag nga aldaw' means 'good day.' It is a general greeting that can be used to wish someone a pleasant day.\n\nListen carefully: naimbag nga aldaw.\n\nNow, try saying 'naimbag nga aldaw' yourself.");
        count += CreateNode(folderPath, "16_Kyros_Word7_Success", "Kyros", "Naimbag nga aldaw! Hope you enjoy your stroll down Calle Crisologo!");

        // Word 8
        count += CreateNode(folderPath, "17_Kyros_Word8_Teach", "Kyros", "Every journey eventually continues to another place.\n\nWhen leaving someone, you can say 'agpakada akon' to say 'goodbye.' Use it when ending a conversation or parting ways.\n\nListen carefully: agpakada akon.\n\nNow, try saying 'agpakada akon' yourself.");
        count += CreateNode(folderPath, "18_Kyros_Word8_Success", "Kyros", "Agpakada akon! You've completed the first set of greetings.\n\nGREETINGS MILESTONE UNLOCKED!\n\nGo meet Vendor Irah at the weaving loom for the Gratitude trials!");

        // KALAW FINAL TEST
        count += CreateNode(folderPath, "19_Kalaw_FinalTest_Intro1", "Kalaw", "Squawk! Look at you, traveler!\n\nYou've walked the streets of Vigan, spoken with the people of Ilocos, and learned the words and expressions that carry our everyday conversations.");
        count += CreateNode(folderPath, "19_Kalaw_FinalTest_Intro2", "Kalaw", "But there's something important you must understand.\n\nLearning a language isn't simply about repeating words after someone else.\n\nTo truly speak a language, you must understand what people say, know which words to use, build your own sentences, and communicate when the situation changes.");
        count += CreateNode(folderPath, "19_Kalaw_FinalTest_Intro3", "Kalaw", "So this time, I won't tell you what to say.\n\nInstead, I'll give you situations.\n\nListen carefully.\nThink about what you've learned.\nThen use your own voice.");
        count += CreateNode(folderPath, "19_Kalaw_FinalTest_Intro4", "Kalaw", "Your final evaluation will test how well you can:\n\n- Understand Ilokano.\n- Use vocabulary.\n- Build sentences.\n- Communicate in everyday situations.\n- Use Ilokano naturally.");
        count += CreateNode(folderPath, "19_Kalaw_FinalTest_Intro5", "Kalaw", "This isn't about whether you can memorize every word perfectly.\n\nIt's about whether you can use what you've learned to communicate.\n\nReady, traveler?\n\nLet's see what your voice has truly learned.");

        // SCORES
        count += CreateNode(folderPath, "20_Kalaw_Score_Advanced", "Kalaw", "SQUAWK! INCREDIBLE!\n\nYour voice has become a true voice of Ilocos!\n\nYou didn't simply memorize words. You understood them. You used them. You built your own sentences. And most importantly, you communicated.\n\nYou've demonstrated an impressive understanding of Ilokano and the people who speak it.");
        count += CreateNode(folderPath, "21_Kalaw_Score_Proficient", "Kalaw", "Excellent work, traveler!\n\nYou can understand Ilokano, respond to everyday situations, and communicate your ideas.\n\nYou've built a strong foundation for continuing your journey with the language.");
        count += CreateNode(folderPath, "22_Kalaw_Score_Developing", "Kalaw", "Well done, traveler!\n\nYou've built a strong foundation in Ilokano, but there are still some areas where your voice needs more practice.\n\nKeep practicing with the people you've met in Ilocos. The more you listen and speak, the more confident you'll become.");
        count += CreateNode(folderPath, "23_Kalaw_Score_Beginning", "Kalaw", "Don't be discouraged, traveler.\n\nEvery language journey begins with a single word.\n\nYou've already taken the first step.\n\nReturn to the people you've met, practice their words, and try again when you're ready. There is always more to learn.");

        // FINAL DIALOGUE
        count += CreateNode(folderPath, "24_Kalaw_Final_Dialogue1", "Kalaw", "SQUAWK!\n\nYOUR VOICE HAS BEEN HEARD!\n\nLook at how far you've come, traveler.");
        count += CreateNode(folderPath, "24_Kalaw_Final_Dialogue2", "Kalaw", "You didn't just memorize words.\n\nYou learned to understand. You learned to respond. You learned to build your own sentences. You learned to ask for help, navigate the streets, introduce yourself, and communicate with the people of Ilocos.");
        count += CreateNode(folderPath, "24_Kalaw_Final_Dialogue3", "Kalaw", "More importantly, you discovered that language lives through the people who speak it.\n\nEvery word you learned came from someone. Every phrase carried a meaning. Every conversation connected you to the people and culture of this place.");
        count += CreateNode(folderPath, "24_Kalaw_Final_Dialogue4", "Kalaw", "The language you practiced here is not simply something to collect.\n\nIt is something living.\n\nSomething carried by families, communities, traditions, and stories.");
        count += CreateNode(folderPath, "24_Kalaw_Final_Dialogue5", "Kalaw", "Your journey through Vigan may be complete, but your journey with Ilokano does not have to end here.\n\nThe more you listen, the more you speak. The more you speak, the more you understand. And the more you understand, the closer you become to the people and culture that keep this language alive.");
        count += CreateNode(folderPath, "24_Kalaw_Final_Dialogue6", "Kalaw", "Great job, traveler.\n\nYou've taken another step in restoring the voices of the regions.");

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Generation Complete!", $"Successfully generated {count} dialogue nodes into {folderPath}.\n\nYou can now assign portraits to them and use the Quest Flow Manager to link them up!", "OK");
    }

    private static int CreateNode(string folderPath, string fileName, string speakerName, string text)
    {
        string fullPath = $"{folderPath}/{fileName}.asset";
        
        DialogueNode existing = AssetDatabase.LoadAssetAtPath<DialogueNode>(fullPath);
        if (existing == null)
        {
            DialogueNode newNode = ScriptableObject.CreateInstance<DialogueNode>();
            newNode.speakerName = speakerName;
            newNode.dialogueText = text;
            
            AssetDatabase.CreateAsset(newNode, fullPath);
            return 1;
        }
        else
        {
            existing.speakerName = speakerName;
            existing.dialogueText = text;
            EditorUtility.SetDirty(existing);
            return 1;
        }
    }
}
