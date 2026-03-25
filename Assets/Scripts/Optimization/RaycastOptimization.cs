using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaycastOptimization : MonoBehaviour
{
    [Tooltip("If true, optimizes the entire scene on Awake. If false, only optimizes this object and its children.")]
    public bool optimizeEntireScene = false;

    void Awake()
    {
        if (optimizeEntireScene)
            OptimizeScene();
        else
            OptimizeHierarchy(gameObject);
    }

    public void OptimizeScene()
    {
        Debug.Log("[RaycastOptimization] Starting Scene-wide UI optimization...");
        // Smart fix: Use newer FindObjectsByType for Unity 2022.2+ and older FindObjectsOfType for others
#if UNITY_2022_2_OR_NEWER
        Graphic[] allGraphics = FindObjectsByType<Graphic>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        Graphic[] allGraphics = FindObjectsOfType<Graphic>(true);
#endif
        int count = 0;

        foreach (var graphic in allGraphics)
        {
            if (ShouldDisableRaycast(graphic))
            {
                graphic.raycastTarget = false;
                count++;
            }
        }
        Debug.Log($"[RaycastOptimization] Disabled raycast on {count} UI elements.");
    }

    public void OptimizeHierarchy(GameObject root)
    {
        Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);
        int count = 0;

        foreach (var graphic in graphics)
        {
            if (ShouldDisableRaycast(graphic))
            {
                graphic.raycastTarget = false;
                count++;
            }
        }
        Debug.Log($"[RaycastOptimization] Optimized {root.name}: Disabled {count} raycast targets.");
    }

    private bool ShouldDisableRaycast(Graphic graphic)
    {
        // NEVER disable raycast on buttons, toggles, or sliders
        if (graphic.GetComponentInParent<Selectable>() != null) return false;
        
        // NEVER disable if there's a custom event trigger
        if (graphic.GetComponent<UnityEngine.EventSystems.EventTrigger>() != null) return false;

        // NEVER disable if the name suggests it's a background blocker
        string name = graphic.gameObject.name.ToLower();
        if (name.Contains("background") || name.Contains("blocker") || name.Contains("dimmer") || name.Contains("fill")) 
            return false;

        // Otherwise, it's just an ornamental image or label - SAFE TO DISABLE
        return true;
    }
}
