using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainLoading : MonoBehaviour
{
    [Header("UI Elements")]
    public UnityEngine.UI.Image loadingFill;
    public TextMeshProUGUI loadingText;
    public CanvasGroup loadingCanvasGroup; // Add this for a foolproof fade fallback

    [Header("Animations")]
    public Animator loadingAnimator; // Keep this, it's used for playing animations
    public string introAnimation = "LoadingReveal";
    public string outroAnimation = "LoadingOutro";
    public float transitionTime = 1f;

    [Header("Components")]
    public StartCrystalBounce crystalBounce;

    [Header("Settings")]
    public string sceneToLoad = "SampleScene";
    public string[] scenesToPreload = new string[] { 
        "LoadingScene", "LoginScene", "SignupScene", "MainMenuScene", 
        "AboutScene", "OptionScene", "CreateCharacterScene", "PrologueScene", 
        "MapSelectionScene", "TutorialScene", "SampleScene", "hatdog"
    };
    public float minimumLoadTime = 5f;
    public float smoothSpeed = 3f;

    private float displayedProgress = 0f;
    private string callerScene;

    void Awake()
    {
        // Auto-add CanvasGroup if missing to support the fade fallback
        if (loadingCanvasGroup == null) loadingCanvasGroup = GetComponent<CanvasGroup>();
        if (loadingCanvasGroup == null) loadingCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        // Ensure it starts opaque
        loadingCanvasGroup.alpha = 1f;
    }

    void Start()
    {
        Debug.Log("[MainLoading] Starting Start()...");
        
        // MOBILE OPTIMIZATION: Ensure smooth 60 FPS on startup
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Debug.Log("[MainLoading] Mobile FPS locked to 60.");

        // UI OPTIMIZATION: Disable Raycast on non-interactive images
        RaycastOptimization uiOpt = GetComponent<RaycastOptimization>();
        if (uiOpt == null) uiOpt = gameObject.AddComponent<RaycastOptimization>();
        uiOpt.OptimizeHierarchy(gameObject);

        // FORCING CANVAS ON TOP: Ensure loading screen is always above other UIs
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Force overlay mode
            canvas.sortingOrder = 100;
            Debug.Log("[MainLoading] Canvas set to Overlay with sorting order 100");
        }

        // Save the caller scene before we do anything else
        callerScene = SceneManager.GetActiveScene().name;
        Debug.Log("[MainLoading] Caller scene detected: " + callerScene);

        // Try getting the dynamic scene target
        if (!string.IsNullOrEmpty(SceneLoader.targetSceneForLoading))
        {
            sceneToLoad = SceneLoader.targetSceneForLoading;
            Debug.Log("[MainLoading] Dynamic target scene: " + sceneToLoad);
            SceneLoader.targetSceneForLoading = "";
        }
        else
        {
            // If we have no target, we are likely being pre-loaded by another script
            // or we are just the first thing in the hierarchy.
            Debug.Log("[MainLoading] No target scene yet. Starting sequence...");
        }

        StartCoroutine(LoadAsyncSequence());
    }

    public void PrepareAndShow(string targetScene)
    {
        // This is called when we were already in memory (pre-loaded)
        sceneToLoad = targetScene;
        callerScene = SceneManager.GetActiveScene().name;
        Debug.Log("[MainLoading] PrepareAndShow - Target: " + sceneToLoad + ", Caller: " + callerScene);
        
        StopAllCoroutines();
        StartCoroutine(LoadAsyncSequence());
    }

    IEnumerator LoadAsyncSequence()
    {
        Debug.Log("[MainLoading] Starting LoadAsyncSequence...");
        
        // Reduce quality to keep UI smooth during expansion
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        // 1. Start Crystal Bounce IMMEDIATELY
        if (crystalBounce != null)
        {
            Debug.Log("[MainLoading] Starting Crystal Bounce early");
            crystalBounce.StartBounce();
        }

        // 2. Intro Animation (Expand BG)
        if (loadingAnimator != null)
        {
            Debug.Log("[MainLoading] Playing Intro: " + introAnimation);
            loadingAnimator.Play(introAnimation, 0, 0f);
            
            // Wait for full expansion before starting heavy I/O
            yield return new WaitForSeconds(transitionTime); 
        }

        // EXTREME LAG REDUCTION: Use Low priority for pre-loading everything
        // This keeps the UI (crystals) smooth during the entire sequence
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        // PREVENT CAMERA CONFLICTS: Untag the old cameras so the new scene doesn't find them
        UntagCamerasInScene(callerScene);

        // PREVENT INPUT CONFLICTS: If there are multiple EventSystems, inputs can break
        DisableEventSystemsInScene(callerScene);

        // Safety check for target scene
        if (string.IsNullOrEmpty(sceneToLoad)) sceneToLoad = SceneLoader.targetSceneForLoading;
        if (string.IsNullOrEmpty(sceneToLoad)) yield break; 

        // 3. Preload all background scenes
        float totalSteps = (scenesToPreload != null ? scenesToPreload.Length : 0) + 1; // +1 for the target scene
        float progressStep = 1f / totalSteps;
        float currentTargetProgress = 0f;

        if (scenesToPreload != null && scenesToPreload.Length > 0)
        {
            for (int i = 0; i < scenesToPreload.Length; i++)
            {
                string pScene = scenesToPreload[i];
                // CRITICAL: Don't pre-load the target scene twice! 
                // This is why it was lagging (loading the heavy scene at the same time as the others).
                if (pScene == sceneToLoad || IsSceneLoaded(pScene)) 
                {
                    currentTargetProgress += progressStep;
                    continue;
                }

                Debug.Log("[MainLoading] Pre-loading: " + pScene);
                AsyncOperation op = SceneManager.LoadSceneAsync(pScene, LoadSceneMode.Additive);
                
                // OPTIMIZATION: Don't just loop, give the CPU room to breathe
                while (!op.isDone)
                {
                    float stepProgress = currentTargetProgress + (op.progress * progressStep);
                    UpdateProgressUI(stepProgress);
                    yield return null;
                }
                
                // Deactivate roots
                Scene loadedPScene = SceneManager.GetSceneByName(pScene);
                if (loadedPScene.IsValid())
                {
                    foreach (GameObject obj in loadedPScene.GetRootGameObjects())
                        obj.SetActive(false);
                }
                currentTargetProgress += progressStep;

                // MORE BREATHING ROOM: Help the phone/PC recover from the heavy IO
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }

        // 4. Load the final Target Scene
        Debug.Log("[MainLoading] Loading Final Target: " + sceneToLoad);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        float timer = 0f;
        while (operation.progress < 0.9f || timer < minimumLoadTime)
        {
            timer += Time.deltaTime;
            float timeProgress = timer / minimumLoadTime;
            float loadProgress = operation.progress / 0.9f;
            
            float targetStepProgress = currentTargetProgress + (Mathf.Min(timeProgress, loadProgress) * progressStep);
            UpdateProgressUI(targetStepProgress);
            yield return null;
        }

        UpdateProgressUI(1f);
        // Wait for visual bar to catch up
        while (displayedProgress < 0.99f) 
        {
            UpdateProgressUI(1f);
            yield return null;
        }

        Debug.Log("[MainLoading] All scenes loaded, activating target...");

        // 5. Activate and Cleanup
        operation.allowSceneActivation = true;
        while (!operation.isDone) yield return null;
        
        // Wait for activation to properly complete (increased for stability)
        for(int i=0; i<10; i++) yield return null;

        Scene targetScene = SceneManager.GetSceneByName(sceneToLoad);
        if (targetScene.IsValid())
        {
            SceneManager.SetActiveScene(targetScene);
            Debug.Log("[MainLoading] " + sceneToLoad + " is now active. Enabling roots...");
            
            // CRITICAL FIX: Ensure all objects in the new scene are turned ON
            foreach (GameObject obj in targetScene.GetRootGameObjects())
            {
                obj.SetActive(true);
            }

            // REFRESH CAMERA REFERENCES: Ensure players in the new scene find the new camera
            RefreshPlayerCameras(targetScene);
        }

        // Give Unity a moment to settle after activation (prevents lag during outro)
        yield return new WaitForSeconds(0.2f);
        
        // 6. Simple Fade Outro
        Debug.Log("[MainLoading] Starting Fade Outro...");
        
        // CLEANUP: Disable own Camera and EventSystem to prevent conflicts
        // This ensures you see the NEW scene's skybox immediately!
        DisableOwnRedundantObjects();

        // UNLOAD PREVIOUS SCENE BEFORE THE FADE STARTS
        // This ensures you never see the Main Menu "flicker"
        if (!string.IsNullOrEmpty(callerScene) && callerScene != sceneToLoad && callerScene != gameObject.scene.name)
        {
            Debug.Log("[MainLoading] Unloading caller: " + callerScene);
            Scene s = SceneManager.GetSceneByName(callerScene);
            if (s.IsValid() && s.isLoaded) SceneManager.UnloadSceneAsync(s);
        }

        // Wait for the new scene to catch its breath (REDUCED to show player drop)
        yield return new WaitForSecondsRealtime(0.1f);

        // Simple Alpha Fade (FASTER)
        float fastTransition = transitionTime * 0.5f; 
        float timer2 = 0f;
        while(timer2 < fastTransition)
        {
            timer2 += Time.unscaledDeltaTime;
            if (loadingCanvasGroup != null) loadingCanvasGroup.alpha = 1f - (timer2 / fastTransition);
            yield return null;
        }

        Debug.Log("[MainLoading] Transition complete.");
        if (loadingCanvasGroup != null) loadingCanvasGroup.alpha = 0f;
        
        foreach (GameObject obj in gameObject.scene.GetRootGameObjects())
        {
            obj.SetActive(false);
        }
            
        // Reset priority to normal
        Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
        
        // Reset SceneLoader flag so we can load another scene later
        SceneLoader.ResetLoadingFlag();
    }

    private void UntagCamerasInScene(string sceneName)
    {
        Scene s = SceneManager.GetSceneByName(sceneName);
        if (!s.IsValid() || !s.isLoaded) return;

        foreach (GameObject obj in s.GetRootGameObjects())
        {
            Camera[] cameras = obj.GetComponentsInChildren<Camera>(true);
            foreach (Camera cam in cameras)
            {
                if (cam.CompareTag("MainCamera"))
                {
                    Debug.Log("[MainLoading] Untagging MainCamera in: " + sceneName);
                    cam.tag = "Untagged";
                }
            }
        }
    }

    private void DisableEventSystemsInScene(string sceneName)
    {
        Scene s = SceneManager.GetSceneByName(sceneName);
        if (!s.IsValid() || !s.isLoaded) return;

        foreach (GameObject obj in s.GetRootGameObjects())
        {
            UnityEngine.EventSystems.EventSystem[] systems = obj.GetComponentsInChildren<UnityEngine.EventSystems.EventSystem>(true);
            foreach (var system in systems)
            {
                Debug.Log("[MainLoading] Disabling EventSystem in: " + sceneName);
                system.enabled = false;
            }
        }
    }

    private void RefreshPlayerCameras(Scene targetScene)
    {
        foreach (GameObject obj in targetScene.GetRootGameObjects())
        {
            // Refresh ThirdPerson
            var tpControllers = obj.GetComponentsInChildren<StarterAssets.ThirdPersonController>(true);
            foreach (var controller in tpControllers)
            {
                Debug.Log("[MainLoading] Refreshing camera for player in: " + targetScene.name);
                controller.RefreshCamera();
            }

            // Refresh FirstPerson
            var fpControllers = obj.GetComponentsInChildren<StarterAssets.FirstPersonController>(true);
            foreach (var controller in fpControllers)
            {
                Debug.Log("[MainLoading] Refreshing camera for player in: " + targetScene.name);
                controller.RefreshCamera();
            }
        }
    }

    private bool IsSceneLoaded(string name)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).name == name) return true;
        return false;
    }

    void UpdateProgressUI(float targetProgress)
    {
        displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, smoothSpeed * Time.deltaTime);
        if (loadingFill != null) loadingFill.fillAmount = displayedProgress;
        int percent = Mathf.RoundToInt(displayedProgress * 100f);
        if (loadingText != null) loadingText.text = "Initializing World... " + percent + "%";
    }

    private void DisableOwnRedundantObjects()
    {
        // Disable own camera so we see the target scene's environment
        Camera ownCam = GetComponentInChildren<Camera>();
        if (ownCam == null) ownCam = Camera.main; // Fallback to whatever camera is left
        if (ownCam != null && ownCam.gameObject.scene == gameObject.scene)
        {
            Debug.Log("[MainLoading] Disabling own loading camera.");
            ownCam.enabled = false;
        }

        // Disable own EventSystem to resolve the warning
        UnityEngine.EventSystems.EventSystem ownEv = GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
        if (ownEv != null)
        {
            Debug.Log("[MainLoading] Disabling own loading EventSystem.");
            ownEv.enabled = false;
        }
    }
}