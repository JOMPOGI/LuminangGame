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
        
        float resScale = 1.0f;
        float shadowDist = 100f;
        int cascades = 2;
        int textureLimit = 0; // 0 = Full resolution, 1 = Half, 2 = Quarter
        int aaLimit = 8; // Anti-aliasing
        int lightCount = 4; // Pixel light count
        float lodLimit = 2.0f; // High poly models stay longer
        AnisotropicFiltering afMode = AnisotropicFiltering.ForceEnable;

        switch (currentQualityLevel)
        {
            case 0: // LOW (Extreme Speed)
                resScale = 0.5f;
                shadowDist = 10f;
                cascades = 0;
                textureLimit = 2;
                aaLimit = 0;
                lightCount = 0; // Disables expensive real-time lighting
                lodLimit = 0.35f; // Forces low-poly models immediately
                afMode = AnisotropicFiltering.Disable;
                break;
            case 1: // MEDIUM (Balanced)
                resScale = 0.75f;
                shadowDist = 35f;
                cascades = 0;
                textureLimit = 1;
                aaLimit = 2;
                lightCount = 1;
                lodLimit = 0.75f;
                afMode = AnisotropicFiltering.Enable;
                break;
            case 2: // HIGH (Maximum Visuals)
                resScale = 1.0f; 
                shadowDist = 100f;
                cascades = 2;
                textureLimit = 0;
                aaLimit = 8;
                lightCount = 4;
                lodLimit = 2.0f;
                afMode = AnisotropicFiltering.ForceEnable;
                break;
        }

        // Apply Built-in Quality Level
        QualitySettings.SetQualityLevel(currentQualityLevel, true);

        // Apply 3D Asset Optimizations (LODs & Textures)
        QualitySettings.globalTextureMipmapLimit = textureLimit;
        QualitySettings.lodBias = lodLimit;

        // Apply Real-time Lighting Optimization (Pixel Lights)
        QualitySettings.pixelLightCount = lightCount;
        QualitySettings.anisotropicFiltering = afMode;

        // Apply Anti-Aliasing (Smooth edges)
        QualitySettings.antiAliasing = aaLimit;

        // Apply Resolution
        int w = Mathf.RoundToInt(Screen.currentResolution.width * resScale);
        int h = Mathf.RoundToInt(Screen.currentResolution.height * resScale);
        if (Screen.width != w) Screen.SetResolution(w, h, true);

        // Apply Shadows
        QualitySettings.shadowDistance = shadowDist;
        QualitySettings.shadowCascades = cascades;
        
        // Apply VSync
        if (disableVSync) QualitySettings.vSyncCount = 0;

        Debug.Log($"[MobilePerformance] Applied {currentQualityLevel}: Res={resScale}, LOD={lodLimit}, Lights={lightCount}, Shadows={shadowDist}");
    }
}
