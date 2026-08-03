using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SplitKalawIntro
{
    [InitializeOnLoadMethod]
    public static void SplitDialogue()
    {
        string basePath = "Assets/Dialogues/CalleCrisologo/Level1_ConversationalSocial/Quest1_Greetings/Kalaw";
        string kalawIntroPath = $"{basePath}/Kalaw_Intro.asset";
        string kalawIntroYesPath = $"{basePath}/Kalaw_Intro_Yes.asset";
        string kalawIntroNoPath = $"{basePath}/Kalaw_Intro_No.asset";
        
        DialogueNode kalawIntro = AssetDatabase.LoadAssetAtPath<DialogueNode>(kalawIntroPath);
        if (kalawIntro == null)
        {
            Debug.LogError("Could not find Kalaw_Intro.asset");
            return;
        }

        // We only want to run this once. If it has choices already (e.g. Yes/No), we can skip.
        if (kalawIntro.choices != null && kalawIntro.choices.Count > 1 && kalawIntro.choices[0].choiceText == "Wen")
        {
            Debug.Log("Kalaw_Intro already split!");
            return;
        }

        // Extract original next node from Kalaw_Intro
        DialogueNode originalNextNode = null;
        if (kalawIntro.choices != null && kalawIntro.choices.Count > 0)
        {
            originalNextNode = kalawIntro.choices[0].nextNode;
        }

        // --- Create Yes Node ---
        DialogueNode nodeYes = AssetDatabase.LoadAssetAtPath<DialogueNode>(kalawIntroYesPath);
        if (nodeYes == null)
        {
            nodeYes = ScriptableObject.CreateInstance<DialogueNode>();
            AssetDatabase.CreateAsset(nodeYes, kalawIntroYesPath);
        }

        nodeYes.speakerName = kalawIntro.speakerName;
        nodeYes.speakerPortrait = kalawIntro.speakerPortrait;
        nodeYes.dialogueText = "Mmm, nasam-it ken naimas! Agyamanak unay, biyahero!\n\nAy, sadinoman ti manners ko—siak ni Kalaw, ti kadwam ken guide mo kadagitoy a lugar!\n\nUray bassit... kitaem dayta daan nga anting-anting a pendant nga aginana iti barukongmo! Mariknam kadi dayta bassit a panaguni?\n\nTi Ilocos Language Crystal nga adda iti uneg dayta ket matmaturog.\n\nTapno mapno iti pigsa ken mapukaw ti turogna, masapul nga agpasyar ka iti Calle Crisologo ken makisarita kadagiti umili iti nasao a pagsasaoda.\n\nBayat ti panagsanaymo ken panagsursurom no kasano ti panagsao dagiti Ilocano iti inaldaw-aldaw a panagbiagda, ti timekmo ti makatulong a mangisubli iti pigsana ti crystal.\n\nIsu a, rugiantayo iti nalaka.\n\nSakbay a pudno a makikadua ka kadagiti tattao ti Ilocos, masapul nga umuna a sursuruem no kasano ti panangabla kadakuada.";
        
        nodeYes.translatedText = "Mmm, sweet and juicy! Thank you so much, traveler!\n\nAh, where are my manners—I am Kalaw, your companion and guide through these lands!\n\nWait a second... look at that ancient anting-anting pendant resting on your chest! Feel that faint hum?\n\nThe Ilocos Language Crystal inside it is sleeping.\n\nTo charge it up and awaken its power, you've got to explore Calle Crisologo and speak with the locals in their native tongue.\n\nAs you practice and learn how Ilocanos talk in their daily lives, your voice will help restore the crystal's power.\n\nSo, let's begin with something simple.\n\nBefore you can truly connect with the people of Ilocos, you must first learn how to greet them.";
        
        nodeYes.choices = new List<DialogueChoice>();
        if (originalNextNode != null)
        {
            nodeYes.choices.Add(new DialogueChoice { choiceText = "", nextNode = originalNextNode, choiceEvent = "ShowPopup:welcome_level1" });
        }
        EditorUtility.SetDirty(nodeYes);

        // --- Create No Node ---
        DialogueNode nodeNo = AssetDatabase.LoadAssetAtPath<DialogueNode>(kalawIntroNoPath);
        if (nodeNo == null)
        {
            nodeNo = ScriptableObject.CreateInstance<DialogueNode>();
            AssetDatabase.CreateAsset(nodeNo, kalawIntroNoPath);
        }

        nodeNo.speakerName = kalawIntro.speakerName;
        nodeNo.speakerPortrait = kalawIntro.speakerPortrait;
        nodeNo.dialogueText = "Ay, kasta kadi... Saan a bale.\n\nAy, sadinoman ti manners ko—siak ni Kalaw, ti kadwam ken guide mo kadagitoy a lugar!\n\nUray bassit... kitaem dayta daan nga anting-anting a pendant nga aginana iti barukongmo! Mariknam kadi dayta bassit a panaguni?\n\nTi Ilocos Language Crystal nga adda iti uneg dayta ket matmaturog.\n\nTapno mapno iti pigsa ken mapukaw ti turogna, masapul nga agpasyar ka iti Calle Crisologo ken makisarita kadagiti umili iti nasao a pagsasaoda.\n\nBayat ti panagsanaymo ken panagsursurom no kasano ti panagsao dagiti Ilocano iti inaldaw-aldaw a panagbiagda, ti timekmo ti makatulong a mangisubli iti pigsana ti crystal.\n\nIsu a, rugiantayo iti nalaka.\n\nSakbay a pudno a makikadua ka kadagiti tattao ti Ilocos, masapul nga umuna a sursuruem no kasano ti panangabla kadakuada.";
        
        nodeNo.translatedText = "Oh, is that so... Nevermind.\n\nAh, where are my manners—I am Kalaw, your companion and guide through these lands!\n\nWait a second... look at that ancient anting-anting pendant resting on your chest! Feel that faint hum?\n\nThe Ilocos Language Crystal inside it is sleeping.\n\nTo charge it up and awaken its power, you've got to explore Calle Crisologo and speak with the locals in their native tongue.\n\nAs you practice and learn how Ilocanos talk in their daily lives, your voice will help restore the crystal's power.\n\nSo, let's begin with something simple.\n\nBefore you can truly connect with the people of Ilocos, you must first learn how to greet them.";
        
        nodeNo.choices = new List<DialogueChoice>();
        if (originalNextNode != null)
        {
            nodeNo.choices.Add(new DialogueChoice { choiceText = "", nextNode = originalNextNode, choiceEvent = "ShowPopup:welcome_level1" });
        }
        EditorUtility.SetDirty(nodeNo);

        // --- Update Original Intro ---
        kalawIntro.dialogueText = "Squawk! Oh... prutas kadi dayta nga adda iti imam? Mabalin kadi nga mangalaak iti bassit? Ti panaglayap iti babaen daytoy napudot a init ti Vigan ket nakapoy unayak...";
        kalawIntro.translatedText = "Squawk! Oh... is that a fruit in your hand? May I have some? Flying under this hot Vigan sun has completely worn me out...";
        
        kalawIntro.choices = new List<DialogueChoice>
        {
            new DialogueChoice { choiceText = "Wen", nextNode = nodeYes },
            new DialogueChoice { choiceText = "Saan", nextNode = nodeNo }
        };

        // Clear out the endEvent/choiceEvent since it's moved to the Yes/No nodes
        kalawIntro.endEventName = "";
        
        EditorUtility.SetDirty(kalawIntro);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[Kalaw Intro Split] Successfully created Yes and No choices!");
    }
}
