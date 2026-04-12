using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MobilePerformance : MonoBehaviour
{
    public static MobilePerformance Instance { get; private set; }

    [Header("Current Settings")]
    public int currentQualityLevel = 2; // 0=Low, 1=Med, 2=High

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

        // Disable sensor alignment — saves CPU
        Input.compensateSensors = false;

        // Force all rigidbodies to Discrete collision (lighter CPU)
#if UNITY_2022_2_OR_NEWER
        Rigidbody[] rbs = FindObjectsByType<Rigidbody>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        Rigidbody[] rbs = FindObjectsOfType<Rigidbody>(true);
#endif
        foreach (var rb in rbs)
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Load saved quality (default High = 2)
        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 2);
        ApplyQualitySettings(savedQuality);
    }

    public void ApplyQualitySettings(int level)
    {
        currentQualityLevel = Mathf.Clamp(level, 0, 2);

        // ── Step 1: Swap URP Asset via Unity Quality Levels ──────────────────
        // This already handles render scale (0.5/0.75/1.0) and URP shadow
        // settings per asset. We do NOT call Screen.SetResolution on top of
        // this — that would compound the scaling and make things too blurry.
        QualitySettings.SetQualityLevel(currentQualityLevel, true);

        // ── Step 2: Runtime settings that the URP Asset doesn't cover ────────
        switch (currentQualityLevel)
        {
            case 0: // ── LOW ── Samsung A-series / weak phones
                // Textures: quarter size (biggest memory/bandwidth saver)
                QualitySettings.globalTextureMipmapLimit = 2;
                // LOD: force cheapest mesh version to appear very close
                QualitySettings.lodBias = 0.3f;
                QualitySettings.maximumLODLevel = 2;
                // Anisotropic filtering: completely off
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                // Skin weights: 1 bone (fastest skinning, less accurate)
                QualitySettings.skinWeights = SkinWeights.OneBone;
                // Particles: skip physics raycasts entirely
                QualitySettings.particleRaycastBudget = 4;
                // FPS cap: 30
                Application.targetFrameRate = 30;
                // Physics: 20 Hz saves significant CPU
                Time.fixedDeltaTime = 0.05f;
                // Sync URP shadow distance to match the quality level
                SetURPShadowDistance(0f);
                break;

            case 1: // ── MEDIUM ── balanced for mid-range phones
                // Textures: half size
                QualitySettings.globalTextureMipmapLimit = 1;
                // LOD: moderate
                QualitySettings.lodBias = 0.6f;
                QualitySettings.maximumLODLevel = 1;
                // Anisotropic filtering: per-texture
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                // Skin weights: 2 bones
                QualitySettings.skinWeights = SkinWeights.TwoBones;
                // Particles: limited budget
                QualitySettings.particleRaycastBudget = 64;
                // FPS cap: 45
                Application.targetFrameRate = 45;
                // Physics: 25 Hz
                Time.fixedDeltaTime = 0.04f;
                // Sync URP shadow distance
                SetURPShadowDistance(20f);
                break;

            case 2: // ── HIGH ── flagship phones
                // Textures: full resolution
                QualitySettings.globalTextureMipmapLimit = 0;
                // LOD: full detail
                QualitySettings.lodBias = 1.0f;
                QualitySettings.maximumLODLevel = 0;
                // Anisotropic filtering: force on for all textures
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                // Skin weights: 4 bones (most accurate)
                QualitySettings.skinWeights = SkinWeights.FourBones;
                // Particles: generous budget
                QualitySettings.particleRaycastBudget = 256;
                // FPS cap: 60
                Application.targetFrameRate = 60;
                // Physics: 30 Hz
                Time.fixedDeltaTime = 0.0333f;
                // Sync URP shadow distance
                SetURPShadowDistance(50f);
                break;
        }

        // ── Step 3: Persist ───────────────────────────────────────────────────
        PlayerPrefs.SetInt("GraphicsQuality", currentQualityLevel);
        PlayerPrefs.Save();

        Debug.Log($"[MobilePerformance] Level {currentQualityLevel} applied | " +
                  $"TexLimit={QualitySettings.globalTextureMipmapLimit} | " +
                  $"LODBias={QualitySettings.lodBias} | " +
                  $"MaxLOD={QualitySettings.maximumLODLevel} | " +
                  $"FPS={Application.targetFrameRate} | " +
                  $"PhysHz={1f / Time.fixedDeltaTime:F0}");
    }

    /// <summary>
    /// Sets the shadow distance directly on the active URP Asset at runtime.
    /// QualitySettings.shadowDistance is ignored by URP, so we must do this.
    /// </summary>
    private void SetURPShadowDistance(float distance)
    {
        var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (pipeline != null)
        {
            pipeline.shadowDistance = distance;
        }
    }
}
