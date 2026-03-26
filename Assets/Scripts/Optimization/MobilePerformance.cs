using UnityEngine;

public class MobilePerformance : MonoBehaviour 
{
    public static MobilePerformance Instance { get; private set; }

    [Header("Current Settings")]
    public int currentQualityLevel = 2; // 0=Low, 1=Med, 2=High

    [Header("Frame Rate")]
    public int targetFrameRate = 60;
    public bool disableVSync = true;

    [Header("Physics (CPU Optimization)")]
    public bool forceDiscretePhysics = true;
    public float fixedTimestep = 0.0333f; // 30Hz physics is usually plenty for mobile

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize from saved setting or default to High (2)
        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 2);
        ApplyQualitySettings(savedQuality);

        // General CPU/Physics Optimizations
        Application.targetFrameRate = targetFrameRate;
        Input.compensateSensors = false; // Disables internal sensor alignment to save CPU
        Time.fixedDeltaTime = fixedTimestep;

        if (forceDiscretePhysics)
        {
            // Smart fix: Use newer FindObjectsByType for Unity 2022.2+ and older FindObjectsOfType for others
#if UNITY_2022_2_OR_NEWER
            Rigidbody[] rb = FindObjectsByType<Rigidbody>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            Rigidbody[] rb = FindObjectsOfType<Rigidbody>(true);
#endif
            foreach (var r in rb) r.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
        
        Debug.Log($"[MobilePerformance] Optimized: FPS={targetFrameRate}, Physics={fixedTimestep}");
    }

    public void ApplyQualitySettings(int level)
    {
        currentQualityLevel = Mathf.Clamp(level, 0, 2);
        
        // 1. Apply Built-in Quality Level (This swaps the URP Asset automatically now)
        QualitySettings.SetQualityLevel(currentQualityLevel, true);

        // 2. Apply Resolution Secondary Scaling (Calculated in code for flexibility)
        float resScale = 1.0f;
        switch (currentQualityLevel)
        {
            case 0: resScale = 0.5f; break;   // Half resolution
            case 1: resScale = 0.75f; break;  // 75% resolution
            case 2: resScale = 1.0f; break;   // Full resolution
        }

        int w = Mathf.RoundToInt(Screen.currentResolution.width * resScale);
        int h = Mathf.RoundToInt(Screen.currentResolution.height * resScale);
        
        // Only set if different to avoid flickering
        if (Screen.width != w || Screen.height != h)
        {
            Screen.SetResolution(w, h, true);
        }

        // 3. Save for next session
        PlayerPrefs.SetInt("GraphicsQuality", currentQualityLevel);
        PlayerPrefs.Save();

        Debug.Log($"[MobilePerformance] Applied Level {currentQualityLevel}: ResScale={resScale}, URP Asset Swept.");
    }
}
