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

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// Spawns and starts a specific minigame prefab.
    /// </summary>
    public void StartMinigame(GameObject prefab)
    {
        if (prefab == null) return;
        
        Debug.Log($"[MinigameManager] Starting Minigame: {prefab.name}");
        
        // Clean up any old instance just in case
        if (_currentInstance != null) Destroy(_currentInstance);

        _currentInstance = Instantiate(prefab, minigameContainer);
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
