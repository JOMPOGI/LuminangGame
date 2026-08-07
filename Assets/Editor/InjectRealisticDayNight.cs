using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InjectRealisticDayNight : EditorWindow
{
    [MenuItem("Tools/Luminang/Inject Realistic URP Day Night System")]
    public static void Inject()
    {
        string folder = "Assets/Data/LightingPresets";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            string[] paths = folder.Split('/');
            string current = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(current + "/" + paths[i]))
                    AssetDatabase.CreateFolder(current, paths[i]);
                current += "/" + paths[i];
            }
        }

        // 1. Create Presets
        DayNightLightingPreset sunrise = CreatePreset(folder, "Sunrise", 
            0.8f, HexToColor("#FFE5B4"), 0.70f, Color.black, 
            HexToColor("#AEC6CF"), HexToColor("#FFC0A0"), HexToColor("#2C3E50"),
            0.5f,
            HexToColor("#FFF3E0"), 15f, 0.2f, 5f, 0f, 0.15f, 0.2f);

        DayNightLightingPreset sunny = CreatePreset(folder, "Sunny", 
            1.20f, HexToColor("#FFFFF0"), 0.85f, Color.black, 
            HexToColor("#87CEEB"), HexToColor("#ADD8E6"), HexToColor("#778899"),
            1.0f,
            Color.white, 2.5f, 0f, 5f, 0f, 0.05f, 0.2f);

        DayNightLightingPreset sunset = CreatePreset(folder, "Sunset", 
            0.65f, HexToColor("#FF7F50"), 0.80f, Color.black, 
            HexToColor("#FFA07A"), HexToColor("#FF8C00"), HexToColor("#483D8B"),
            0.8f,
            HexToColor("#FFDAB9"), 25f, 0f, 10f, 5f, 0.20f, 0.2f);

        DayNightLightingPreset night = CreatePreset(folder, "Night", 
            0.20f, HexToColor("#AFCBFF"), 1.0f, HexToColor("#0A0A1A"), 
            HexToColor("#000033"), HexToColor("#191970"), HexToColor("#050510"),
            0.1f,
            HexToColor("#E0F0FF"), -30f, -0.2f, 10f, -5f, 0.40f, 1.0f);

        // 2. Scene Setup
        string scenePath = "Assets/Scenes/Environments/Calle_Crisologo.unity";
        Scene currentScene = EditorSceneManager.GetActiveScene();
        if (currentScene.path != scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                currentScene = EditorSceneManager.OpenScene(scenePath);
            else return;
        }

        GameObject worldManager = GameObject.Find("WorldManager");
        if (worldManager == null) worldManager = new GameObject("WorldManager");

        // Clean up old scripts
        var oldWeather = worldManager.GetComponent("WeatherManager");
        if (oldWeather != null) DestroyImmediate(oldWeather);
        var oldLighting = worldManager.GetComponent("LightingController");
        if (oldLighting != null) DestroyImmediate(oldLighting);

        // URP Volume
        GameObject volumeObj = GameObject.Find("GlobalVolume");
        if (volumeObj == null) volumeObj = new GameObject("GlobalVolume");
        
        Volume volume = volumeObj.GetComponent<Volume>();
        if (volume == null) volume = volumeObj.AddComponent<Volume>();
        volume.isGlobal = true;

        if (volume.profile == null)
        {
            volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(volume.profile, "Assets/Data/LightingPresets/GlobalLightingVolumeProfile.asset");
        }

        if (!volume.profile.Has<ColorAdjustments>()) volume.profile.Add<ColorAdjustments>();
        if (!volume.profile.Has<WhiteBalance>()) volume.profile.Add<WhiteBalance>();
        if (!volume.profile.Has<Vignette>()) volume.profile.Add<Vignette>();

        // Enable overrides
        volume.profile.TryGet(out ColorAdjustments ca);
        ca.colorFilter.overrideState = true;
        ca.postExposure.overrideState = true;
        ca.contrast.overrideState = true;
        ca.saturation.overrideState = true;

        volume.profile.TryGet(out WhiteBalance wb);
        wb.temperature.overrideState = true;

        volume.profile.TryGet(out Vignette vig);
        vig.intensity.overrideState = true;
        vig.smoothness.overrideState = true;

        // Add Controller
        URPDayNightCycle controller = worldManager.GetComponent<URPDayNightCycle>();
        if (controller == null) controller = worldManager.AddComponent<URPDayNightCycle>();
        
        controller.sunrisePreset = sunrise;
        controller.sunnyPreset = sunny;
        controller.sunsetPreset = sunset;
        controller.nightPreset = night;
        controller.globalVolume = volume;
        
        Light sun = Object.FindFirstObjectByType<Light>();
        if (sun != null && sun.type == LightType.Directional) controller.directionalLight = sun;

        EditorSceneManager.MarkSceneDirty(currentScene);
        EditorSceneManager.SaveScene(currentScene);
        AssetDatabase.SaveAssets();

        Debug.Log("<color=cyan>SUCCESS: Installed Realistic URP Day/Night System!</color>");
    }

    private static DayNightLightingPreset CreatePreset(string folder, string name, 
        float sunInt, Color sunCol, float shadowStr, Color shadowCol, 
        Color sky, Color eq, Color ground, float reflectInt,
        Color volFilter, float volTemp, float volExp, float volCont, float volSat, float vigInt, float vigSmooth)
    {
        string path = folder + "/" + name + "Preset.asset";
        DayNightLightingPreset preset = AssetDatabase.LoadAssetAtPath<DayNightLightingPreset>(path);
        if (preset == null)
        {
            preset = ScriptableObject.CreateInstance<DayNightLightingPreset>();
            AssetDatabase.CreateAsset(preset, path);
        }

        preset.sunIntensity = sunInt;
        preset.sunColor = sunCol;
        preset.shadowStrength = shadowStr;
        preset.shadowColor = shadowCol;

        preset.skyColor = sky;
        preset.equatorColor = eq;
        preset.groundColor = ground;
        preset.reflectionIntensity = reflectInt;

        preset.colorFilter = volFilter;
        preset.temperature = volTemp;
        preset.postExposure = volExp;
        preset.contrast = volCont;
        preset.saturation = volSat;
        preset.vignetteIntensity = vigInt;
        preset.vignetteSmoothness = vigSmooth;

        EditorUtility.SetDirty(preset);
        return preset;
    }

    private static Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }
}
