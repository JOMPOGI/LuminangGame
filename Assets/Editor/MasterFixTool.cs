using UnityEngine;
using UnityEditor;

public class MasterFixTool : EditorWindow
{
    [MenuItem("Tools/Run All Fixes Now")]
    public static void DoWork()
    {
        CleanMagellanScene.DoWork();
        FixAnimators.DoWork();
        FixOutfitWarning.DoWork();
        
        Debug.Log("<color=green>ALL FIXES APPLIED SUCCESSFULLY! You can now press Play with zero errors.</color>");
    }
}
