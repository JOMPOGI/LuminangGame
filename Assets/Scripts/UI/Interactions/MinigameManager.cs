using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A placeholder manager for Minigames. 
/// Use this to block gameplay and show a UI overlay when a minigame is triggered.
/// </summary>
public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("Dynamic Spawning")]
    [Tooltip("The container where minigame prefabs will be spawned (usually a Canvas).")]
    public Transform minigameContainer;
    
    [Header("Events")]
    public UnityEvent onMinigameComplete;
    
    private GameObject _currentInstance;
    public bool IsMinigameActive => _currentInstance != null;
    public string CurrentCategory { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// Starts a minigame and assigns it a specific category (useful for dynamic content).
    /// </summary>
    public void StartMinigameWithCategory(GameObject prefab, string category)
    {
        CurrentCategory = category;
        StartMinigame(prefab);
    }

    /// <summary>
    /// Spawns and starts a specific minigame prefab.
    /// </summary>
    public void StartMinigame(GameObject prefab)
    {
        if (prefab == null) return;
        
        // If no category was set via the helper, clear the old one
        if (string.IsNullOrEmpty(CurrentCategory)) CurrentCategory = "";
        
        Debug.Log($"[MinigameManager] Starting Minigame: {prefab.name}");
        
        // Clean up any old instance just in case
        if (_currentInstance != null) Destroy(_currentInstance);

        _currentInstance = Instantiate(prefab, minigameContainer);

        // Professional Fail-safe: Reset UI position and scale so it doesn't spawn 'into the void'
        RectTransform rt = _currentInstance.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            
            // Ensure it stretches to fill the container
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
        }
    }

    /// <summary>
    /// Destroys the current minigame and restores the HUD.
    /// </summary>
    public void HideMinigame()
    {
        if (_currentInstance != null) 
        {
            Destroy(_currentInstance);
            _currentInstance = null;
        }
        
        Debug.Log("[MinigameManager] Minigame Finished.");
        onMinigameComplete?.Invoke();
    }
}
