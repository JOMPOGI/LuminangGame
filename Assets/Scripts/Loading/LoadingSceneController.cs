using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    public UnityEngine.UI.Image loadingFill;
    public TextMeshProUGUI loadingText;
    public CanvasGroup loadingCanvasGroup; // Add this for a foolproof fade fallback
    public Animator loadingAnimator;
    public StartCrystalBounce crystalBounce;

    [Header("Animations")]
    public string introAnimation = "LoadingReveal";
    public string outroAnimation = "LoadingOutro";
    public float transitionTime = 1f;
    public float minimumLoadTime = 3f;

    private string sceneToLoad;
    private string callerScene;

    void Awake()
    {
        // Auto-add CanvasGroup if missing to support the fade fallback
        if (loadingCanvasGroup == null) loadingCanvasGroup = GetComponent<CanvasGroup>();
        if (loadingCanvasGroup == null) loadingCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        // Ensure it starts opaque
        loadingCanvasGroup.alpha = 1f;

        // Ensure target is set early
        if (string.IsNullOrEmpty(sceneToLoad)) sceneToLoad = SceneLoader.targetSceneForLoading;
    }

    void Start()
    {
        Debug.Log("[LoadingScene] Starting Start()...");
        
        // FORCING CANVAS ON TOP: Ensure loading screen is always above joysticks
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Force overlay mode
            canvas.sortingOrder = 100;
            Debug.Log("[LoadingScene] Canvas set to Overlay with sorting order 100");
        }

        // Save the caller scene as early as possible
        callerScene = SceneManager.GetActiveScene().name;
        Debug.Log("[LoadingScene] Caller scene detected: " + callerScene);

        // Get the target scene from SceneLoader
        sceneToLoad = SceneLoader.targetSceneForLoading;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            // If LoadingScene is being pre-loaded by MainLoading, it won't have a target yet
            Debug.Log("[LoadingScene] No target scene yet. This might be a pre-load.");
            return;
        }

        StartCoroutine(LoadProcess());
    }

    public void PrepareAndShow(string targetScene)
    {
        // This is called when we were already in memory (pre-loaded)
        sceneToLoad = targetScene;
        callerScene = SceneManager.GetActiveScene().name;
        Debug.Log("[LoadingScene] PrepareAndShow - Target: " + sceneToLoad + ", Caller: " + callerScene);
        
        // Ensure animator is fresh and ready
        if (loadingAnimator != null)
        {
            loadingAnimator.enabled = true;
            loadingAnimator.Rebind();
            loadingAnimator.Update(0f);
        }

        StopAllCoroutines();
        StartCoroutine(LoadProcess());
    }

    IEnumerator LoadProcess()
    {
        Debug.Log("[LoadingScene] Starting LoadProcess...");

        // Reduce background loading impact to keep UI smooth during animation
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        // 1. Start Crystal Bounce IMMEDIATELY
        if (crystalBounce != null)
        {
            Debug.Log("[LoadingScene] Starting Crystal Bounce early");
            crystalBounce.StartBounce();
        }

        // 2. Expand Animation (Intro)
        if (loadingAnimator != null)
        {
            Debug.Log("[LoadingScene] Playing Intro: " + introAnimation);
            loadingAnimator.Play(introAnimation, 0, 0f);
            
            // CRITICAL: We wait for the FULL animation to finish before starting the heavy load
            // This ensures the "eat up" is 100% smooth without lag spikes.
            yield return new WaitForSeconds(transitionTime);
        }

        // Wait a frame to ensure target is set
        if (string.IsNullOrEmpty(sceneToLoad)) sceneToLoad = SceneLoader.targetSceneForLoading;

        // EXTREME LAG REDUCTION: Use Low priority for the entire loading process
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        // UI OPTIMIZATION: Disable Raycast on non-interactive images
        RaycastOptimization uiOpt = GetComponent<RaycastOptimization>();
        if (uiOpt == null) uiOpt = gameObject.AddComponent<RaycastOptimization>();
        uiOpt.OptimizeHierarchy(gameObject);

        // PREVENT CAMERA CONFLICTS: Untag the old cameras so the new scene doesn't find them
        UntagCamerasInScene(callerScene);
        
        // PREVENT INPUT CONFLICTS: Disable old EventSystems
        DisableEventSystemsInScene(callerScene);

        // 3. Load Target Scene in Background
        Debug.Log("[LoadingScene] Starting Async Load for: " + sceneToLoad);
        float startTime = Time.time;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;
        while (operation.progress < 0.9f || (Time.time - startTime) < minimumLoadTime)
        {
            float realProgress = operation.progress / 0.9f;
            float timeProgress = (Time.time - startTime) / minimumLoadTime;
            float targetProgress = Mathf.Min(realProgress, timeProgress);

            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, 1.5f * Time.deltaTime);

            if (loadingText != null)
            {
                int percent = Mathf.RoundToInt(displayedProgress * 100f);
                loadingText.text = "Loading Assets... " + percent + "%";
                if (loadingFill != null) loadingFill.fillAmount = displayedProgress;
            }
            yield return null;
        }

        if (loadingText != null)
        {
            loadingText.text = "Loading Assets... 100%";
        }

        // Wait for visual bar to catch up smoothly
        while (displayedProgress < 0.99f)
        {
            displayedProgress = Mathf.MoveTowards(displayedProgress, 1f, 2f * Time.deltaTime);
            if (loadingFill != null) loadingFill.fillAmount = displayedProgress;
            yield return null;
        }

        Debug.Log("[LoadingScene] Loading complete, activating scene...");

        // 4. Activate the new scene and cleanup old one
        operation.allowSceneActivation = true;
        
        // Wait for activation to properly complete
        while (!operation.isDone) yield return null;
        
        // Multiple frames to let the new scene's Awake/Start/Physics settle
        for(int i=0; i<5; i++) yield return null;

        Debug.Log("[LoadingScene] Scene activated, setting active...");

        Scene loadedScene = SceneManager.GetSceneByName(sceneToLoad);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
            Debug.Log("[LoadingScene] " + sceneToLoad + " is now active. Enabling roots...");
            
            // CRITICAL FIX: Ensure all objects in the new scene are turned ON
            foreach (GameObject obj in loadedScene.GetRootGameObjects())
            {
                obj.SetActive(true);
            }

            // REFRESH CAMERA REFERENCES: Ensure players in the new scene find the new camera
            RefreshPlayerCameras(loadedScene);
        }

        // Give Unity a moment to settle after activation (prevents lag during outro)
        yield return new WaitForSeconds(0.3f);
        
        // 5. Simple Fade Outro
        Debug.Log("[LoadingScene] Starting Fade Outro...");

        // CLEANUP: Disable own Camera and EventSystem to resolve conflicts
        DisableOwnRedundantObjects();

        // UNLOAD PREVIOUS SCENE BEFORE THE FADE STARTS
        // This ensures you never see the Main Menu "flicker"
        if (!string.IsNullOrEmpty(callerScene) && callerScene != sceneToLoad && callerScene != gameObject.scene.name)
        {
            Debug.Log("[LoadingScene] Unloading caller: " + callerScene);
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

        Debug.Log("[LoadingScene] Transition complete.");
        if (loadingCanvasGroup != null) loadingCanvasGroup.alpha = 0f;
        
        foreach (GameObject obj in gameObject.scene.GetRootGameObjects())
        {
            obj.SetActive(false);
        }
    
        // Reset priority to normal
        Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
        
        // Reset SceneLoader flag 
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
                    Debug.Log("[LoadingScene] Untagging MainCamera in: " + sceneName);
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
                Debug.Log("[LoadingScene] Disabling EventSystem in: " + sceneName);
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
                Debug.Log("[LoadingScene] Refreshing camera for player in: " + targetScene.name);
                controller.RefreshCamera();
            }

            // Refresh FirstPerson
            var fpControllers = obj.GetComponentsInChildren<StarterAssets.FirstPersonController>(true);
            foreach (var controller in fpControllers)
            {
                Debug.Log("[LoadingScene] Refreshing camera for player in: " + targetScene.name);
                controller.RefreshCamera();
            }
        }
    }

    private void DisableOwnRedundantObjects()
    {
        // Disable own camera so we see the target scene's environment immediately
        Camera ownCam = GetComponentInChildren<Camera>();
        if (ownCam == null) ownCam = Camera.main;
        if (ownCam != null && ownCam.gameObject.scene == gameObject.scene)
        {
            Debug.Log("[LoadingScene] Disabling own loading camera.");
            ownCam.enabled = false;
        }

        // Disable own EventSystem to resolve the "2 EventSystems" warning
        UnityEngine.EventSystems.EventSystem ownEv = GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
        if (ownEv != null)
        {
            Debug.Log("[LoadingScene] Disabling own loading EventSystem.");
            ownEv.enabled = false;
        }
    }
}
