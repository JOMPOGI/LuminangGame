using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DebugInteraction : EditorWindow
{
    [MenuItem("Tools/Debug Interaction & UI")]
    public static void ShowWindow()
    {
        Debug.Log("--- DEBUG INTERACTION ---");
        
        var interactionManager = FindFirstObjectByType<InteractionManager>();
        if (interactionManager != null)
        {
            var button = interactionManager.talkButton;
            Debug.Log($"InteractionManager found. TalkButton: {(button != null ? button.name : "NULL")}");
            if (button != null)
            {
                Debug.Log($"Button ActiveInHierarchy: {button.gameObject.activeInHierarchy}, Interactable: {button.interactable}");
            }
        }
        else
        {
            Debug.LogWarning("No InteractionManager found.");
        }

        // Test UI Raycast at center of screen
        if (EventSystem.current != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Screen.width / 2f, Screen.height / 2f)
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            
            Debug.Log($"UI Raycast at center ({pointerData.position}) hit {results.Count} objects:");
            foreach (var result in results)
            {
                Debug.Log($"- {result.gameObject.name} (Layer: {LayerMask.LayerToName(result.gameObject.layer)}) [RaycastTarget: {result.gameObject.GetComponent<Graphic>()?.raycastTarget}]");
            }
        }
        else
        {
            Debug.LogWarning("No EventSystem found.");
        }
        
        // Find "hatdog"
        var hatdog = GameObject.Find("hatdog");
        if (hatdog != null)
        {
            Debug.Log($"Found 'hatdog' GameObject at {hatdog.transform.position}. Components:");
            foreach (var comp in hatdog.GetComponents<Component>())
            {
                if (comp != null) Debug.Log($"  - {comp.GetType().Name}");
            }
        }
        else
        {
            Debug.Log("No GameObject named 'hatdog' found in scene.");
        }
        
        Debug.Log("-------------------------");
    }
}
