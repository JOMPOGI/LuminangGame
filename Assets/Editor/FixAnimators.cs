using UnityEngine;
using UnityEditor;
using System.IO;

public class FixAnimators : EditorWindow
{
    [MenuItem("Tools/Fix Animator Errors")]
    public static void DoWork()
    {
        string kalawPath = "Assets/Scripts/AnimalAnimation/KalawIdleTest.cs";
        if (File.Exists(kalawPath))
        {
            string content = File.ReadAllText(kalawPath);
            // Replace animator.SetTrigger with SafeSetTrigger
            content = content.Replace("animator.SetTrigger(", "SafeSetTrigger(");
            content = content.Replace("animator.ResetTrigger(", "SafeResetTrigger(");
            
            // Add the safe methods
            if (!content.Contains("void SafeSetTrigger"))
            {
                string safeMethods = @"
    private void SafeSetTrigger(string triggerName)
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == triggerName)
                {
                    animator.SetTrigger(triggerName);
                    return;
                }
            }
        }
    }

    private void SafeResetTrigger(string triggerName)
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == triggerName)
                {
                    animator.ResetTrigger(triggerName);
                    return;
                }
            }
        }
    }
}
";
                // Replace the last closing brace with the safe methods
                int lastBrace = content.LastIndexOf('}');
                if (lastBrace > 0)
                {
                    content = content.Remove(lastBrace, 1).Insert(lastBrace, safeMethods);
                }
            }
            File.WriteAllText(kalawPath, content);
        }

        string randomIdlePath = "Assets/Scripts/UI/Interactions/NPCRandomIdle.cs";
        if (File.Exists(randomIdlePath))
        {
            string content = File.ReadAllText(randomIdlePath);
            if (!content.Contains("SafeCrossFade"))
            {
                content = content.Replace("_animator.CrossFadeInFixedTime(defaultIdleState, 0.25f);", "SafeCrossFade(defaultIdleState, 0.25f);");
                content = content.Replace("_animator.CrossFadeInFixedTime(randomAnim, 0.25f);", "SafeCrossFade(randomAnim, 0.25f);");

                string safeMethods = @"
    private void SafeCrossFade(string stateName, float duration)
    {
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            if (_animator.HasState(0, Animator.StringToHash(stateName)))
            {
                _animator.CrossFadeInFixedTime(stateName, duration, 0);
            }
        }
    }
}
";
                int lastBrace = content.LastIndexOf('}');
                if (lastBrace > 0)
                {
                    content = content.Remove(lastBrace, 1).Insert(lastBrace, safeMethods);
                }
                File.WriteAllText(randomIdlePath, content);
            }
        }

        string questIdlePath = "Assets/Scripts/UI/Interactions/QuestIdleManager.cs";
        if (File.Exists(questIdlePath))
        {
            string content = File.ReadAllText(questIdlePath);
            if (!content.Contains("SafeCrossFade"))
            {
                content = content.Replace("targetAnimator.CrossFadeInFixedTime(\"Breathing_Idle\", 0.1f, 0);", "SafeCrossFade(targetAnimator, \"Breathing_Idle\", 0.1f);");

                string safeMethods = @"
    private void SafeCrossFade(Animator anim, string stateName, float duration)
    {
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            if (anim.HasState(0, Animator.StringToHash(stateName)))
            {
                anim.CrossFadeInFixedTime(stateName, duration, 0);
            }
        }
    }
}
";
                int lastBrace = content.LastIndexOf('}');
                if (lastBrace > 0)
                {
                    content = content.Remove(lastBrace, 1).Insert(lastBrace, safeMethods);
                }
                File.WriteAllText(questIdlePath, content);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("<color=green>SUCCESS: Fixed Animator Parameter and CrossFade errors in Scripts!</color>");
    }
}
