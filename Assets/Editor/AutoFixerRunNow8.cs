using UnityEngine;
using UnityEditor;

public class AutoFixerRunNow8
{
    [InitializeOnLoadMethod]
    static void RunOnLoad()
    {
        if (SessionState.GetBool("AutoFixerRunNow8_Executed", false))
            return;

        SessionState.SetBool("AutoFixerRunNow8_Executed", true);

        EditorApplication.delayCall += () =>
        {
            GameObject kalawGo = GameObject.Find("RiggedKalaw") ?? GameObject.Find("Kalaw");
            if (kalawGo != null)
            {
                InteractableNPC kalaw = kalawGo.GetComponent<InteractableNPC>();
                if (kalaw != null)
                {
                    Undo.RecordObject(kalaw, "Force Kalaw Dialogue");
                    
                    if (kalaw.defaultDialogue == null)
                    {
                        DialogueNode kalawTeach = AssetDatabase.LoadAssetAtPath<DialogueNode>("Assets/Dialogues/MassiveImport/01_Kalaw_Teach.asset");
                        if (kalawTeach != null)
                        {
                            kalaw.defaultDialogue = kalawTeach;
                            Debug.Log("Forced Kalaw's defaultDialogue to 01_Kalaw_Teach!");
                        }
                        else
                        {
                            Debug.LogError("COULD NOT FIND 01_Kalaw_Teach.asset!");
                        }
                    }
                    
                    kalaw.interactionEnabled = true;
                    EditorUtility.SetDirty(kalaw);
                }
            }
            else
            {
                Debug.LogError("RiggedKalaw GameObject not found in scene!");
            }
        };
    }
}
