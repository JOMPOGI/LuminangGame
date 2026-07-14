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
    public int CurrentLanguageId { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// Starts a minigame and assigns it a specific category and language.
    /// </summary>
    public void StartMinigameWithCategory(GameObject prefab, string category, int languageId)
    {
        CurrentCategory = category;
        CurrentLanguageId = languageId;
        StartMinigame(prefab);
    }

    /// <summary>
    /// Spawns and starts a specific minigame prefab.
    /// </summary>
    public void StartMinigame(GameObject prefab)
    {
        if (prefab == null) return;
        
        // If no category/language was set via the helper, fallback to defaults
        if (string.IsNullOrEmpty(CurrentCategory)) CurrentCategory = "";
        if (CurrentLanguageId <= 0) 
        {
            if (LessonManager.Instance != null) CurrentLanguageId = LessonManager.Instance.languageId;
            else CurrentLanguageId = 1; // Default to Ilokano
        }
        
        Debug.Log($"[MinigameManager] Starting Minigame: {prefab.name}");
        
        // Clean up any old instance just in case
        if (_currentInstance != null) Destroy(_currentInstance);

        _currentInstance = Instantiate(prefab, minigameContainer);
        _currentInstance.SetActive(true); // Ensure it's visible even if the prefab was disabled

        // Professional Fail-safe: Reset UI position and scale so it doesn't spawn 'into the void'
        RectTransform rt = _currentInstance.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
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
