using UnityEngine;
using UnityEditor;

public class DebugKalawAnimation : EditorWindow
{
    [MenuItem("Tools/Debug Kalaw Animation")]
    public static void DebugKalaw()
    {
        var kalaw = GameObject.Find("Kalaw");
        if (kalaw != null)
        {
            var anim = kalaw.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                Debug.Log($"Kalaw Animator Controller: {(anim.runtimeAnimatorController != null ? anim.runtimeAnimatorController.name : "NULL")}");
                Debug.Log($"Kalaw Avatar: {(anim.avatar != null ? anim.avatar.name : "NULL")}");
            }
            else
            {
                Debug.Log("Kalaw has NO Animator component!");
            }

            var idle = kalaw.GetComponentInChildren<NPCRandomIdle>();
            if (idle != null)
            {
                Debug.Log($"Kalaw NPCRandomIdle found: {idle.defaultIdleState}");
            }
            else
            {
                Debug.Log("Kalaw has NO NPCRandomIdle component!");
            }
        }
    }
}
