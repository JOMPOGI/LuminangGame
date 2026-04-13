using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapTransitionManager : MonoBehaviour
{
    public static MapTransitionManager Instance { get; private set; }

    public enum State { Closed, Open, Panel }

    [System.Serializable]
    public class CloudState
    {
        public string cloudName;
        public Vector2 position;
        public Vector2 size;
        public Vector3 scale;
    }

    [Header("Transition Settings")]
    public float transitionDuration = 2.0f;
    public float staggerStrength = 1.0f; // Max delay for the furthest cloud
    public float fadeDuration = 0.5f;    // Time for transparency changes
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Responsiveness")]
    [Tooltip("Extra scale applied to ALL clouds when CLOSED to ensure full coverage on any aspect ratio.")]
    public float closedStateScale = 1.25f;
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    [Header("Transitions & Handoff")]
    [Tooltip("Check this if this is the SECOND scene in a transition (like Tutorial). It will start visible and fade out.")]
    public bool isHandoffScene = false;

    [Header("Saved States")]
    public List<CloudState> closedState = new List<CloudState>();
    public List<CloudState> openState = new List<CloudState>();
    public List<CloudState> panelState = new List<CloudState>();

    [Header("Idle Animation")]
    public bool enableIdleFloat = true;
    public float floatSpeed = 1f;
    public float floatAmount = 10f;

    private List<RuntimeCloudData> runtimeClouds = new List<RuntimeCloudData>();
    private State currentState = State.Closed;
    private Coroutine transitionCoroutine;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        InitializeRuntimeClouds();
        
        // Start with the map CLOSED
        SetToState(State.Closed);

        // Prepare Transparency
        if (isHandoffScene)
        {
            // Second scene: Start fully visible (Closed) and fade out
            canvasGroup.alpha = 1f;
        }
        else
        {
            // First scene: Start hidden and fade in
            canvasGroup.alpha = 0f;
        }
    }

    private void Start()
    {
        if (isHandoffScene)
        {
            StartCoroutine(HandoffSequence());
        }
        else
        {
            StartCoroutine(EntranceSequence());
        }
    }

    private IEnumerator EntranceSequence()
    {
        // 1. Fade In
        yield return StartCoroutine(FadeAlpha(1f));
        // 2. Part the clouds
        OpenMap();
    }

    private IEnumerator HandoffSequence()
    {
        // 1. Ensure we are closed (already set in Awake)
        // 2. Wait a split second for the scene to settle
        yield return new WaitForSeconds(0.1f);
        
        // 3. Open the clouds AND Fade Out at the same time
        OpenMap();
        yield return StartCoroutine(FadeAlpha(0f));
    }

    private IEnumerator FadeAlpha(float target)
    {
        float start = canvasGroup.alpha;
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    private void InitializeRuntimeClouds()
    {
        runtimeClouds.Clear();
        
        float maxDistance = 0f;
        List<RuntimeCloudData> temp = new List<RuntimeCloudData>();

        // 1. Gather clouds and find the maximum distance from center
        foreach (Transform child in transform)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            Image img = child.GetComponent<Image>();
            if (rt == null) continue;

            if (img != null) img.raycastTarget = false;

            CloudState closedS = closedState.Find(s => s.cloudName == child.name);
            CloudState openS = openState.Find(s => s.cloudName == child.name);
            CloudState panelS = panelState.Find(s => s.cloudName == child.name);

            if (closedS != null && openS != null && panelS != null)
            {
                float dist = closedS.position.magnitude;
                if (dist > maxDistance) maxDistance = dist;

                temp.Add(new RuntimeCloudData
                {
                    rect = rt,
                    closedData = closedS,
                    openData = openS,
                    panelData = panelS,
                    randomOffset = Random.Range(0f, 100f),
                    centerDist = dist
                });
            }
        }

        // 2. Normalize distances (0 = center, 1 = furthest) for the stagger effect
        foreach (var cloud in temp)
        {
            cloud.normalizedDist = (maxDistance > 0) ? (cloud.centerDist / maxDistance) : 0;
            runtimeClouds.Add(cloud);
        }
    }

    [ContextMenu("Capture CURRENT as CLOSED")]
    public void CaptureClosed() { CaptureState(closedState); Debug.Log("Captured CLOSED."); }

    [ContextMenu("Capture CURRENT as OPEN")]
    public void CaptureOpen() { CaptureState(openState); Debug.Log("Captured OPEN."); }

    [ContextMenu("Capture CURRENT as PANEL")]
    public void CapturePanel() { CaptureState(panelState); Debug.Log("Captured PANEL."); }

    private void CaptureState(List<CloudState> targetList)
    {
        targetList.Clear();
        foreach (Transform child in transform)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null)
            {
                targetList.Add(new CloudState
                {
                    cloudName = child.name,
                    position = rt.anchoredPosition,
                    size = rt.sizeDelta,
                    scale = rt.localScale
                });
            }
        }
    }

    public void SetPanelFocus(bool active)
    {
        AnimateToState(active ? State.Panel : State.Open);
    }

    [ContextMenu("Open Map")]
    public void OpenMap() => AnimateToState(State.Open);

    [ContextMenu("Close Map")]
    public void CloseMap() => AnimateToState(State.Closed);

    public void AnimateToState(State target)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(AnimateTransition(target));
    }

    private IEnumerator AnimateTransition(State target)
    {
        float elapsed = 0;
        currentState = target;

        // Capture current positions as the starting point for a fluid transition
        Dictionary<RuntimeCloudData, CloudState> capturedStarts = new Dictionary<RuntimeCloudData, CloudState>();
        foreach (var cloud in runtimeClouds)
        {
            capturedStarts[cloud] = new CloudState
            {
                position = cloud.rect.anchoredPosition,
                size = cloud.rect.sizeDelta,
                scale = cloud.rect.localScale
            };
        }

        // ONLY apply stagger when coming from or going to the CLOSED state
        // (Removing the startState check here simplifies logic: if we're moving TO closed, we stagger)
        bool useStagger = (target == State.Closed);
        float currentStagger = useStagger ? staggerStrength : 0f;
        
        float totalDuration = transitionDuration + currentStagger;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            foreach (var cloud in runtimeClouds)
            {
                float delay = useStagger ? (cloud.normalizedDist * currentStagger) : 0f;
                float normalizedTime = Mathf.Clamp01((elapsed - delay) / transitionDuration);
                float t = transitionCurve.Evaluate(normalizedTime);

                CloudState startData = capturedStarts[cloud];
                CloudState targetData = GetStateData(cloud, target);

                float targetScaleMult = (target == State.Closed) ? closedStateScale : 1.0f;

                cloud.rect.anchoredPosition = Vector2.Lerp(startData.position, targetData.position, t);
                cloud.rect.sizeDelta = Vector2.Lerp(startData.size, targetData.size, t);
                cloud.rect.localScale = Vector3.Lerp(startData.scale, targetData.scale * targetScaleMult, t);
            }
            yield return null;
        }

        SetToState(target);
        transitionCoroutine = null;
    }

    private CloudState GetStateData(RuntimeCloudData cloud, State state)
    {
        return state switch
        {
            State.Closed => cloud.closedData,
            State.Open => cloud.openData,
            State.Panel => cloud.panelData,
            _ => cloud.openData
        };
    }

    private void SetToState(State target)
    {
        currentState = target;
        float scaleMult = (target == State.Closed) ? closedStateScale : 1.0f;

        foreach (var cloud in runtimeClouds)
        {
            CloudState data = GetStateData(cloud, target);
            cloud.rect.anchoredPosition = data.position;
            cloud.rect.sizeDelta = data.size;
            cloud.rect.localScale = data.scale * scaleMult;
        }
    }

    private void Update()
    {
        if (!enableIdleFloat || runtimeClouds.Count == 0 || transitionCoroutine != null) return;

        float time = Time.time * floatSpeed;
        float scaleMult = (currentState == State.Closed) ? closedStateScale : 1.0f;

        foreach (var cloud in runtimeClouds)
        {
            float noiseX = Mathf.PerlinNoise(time + cloud.randomOffset, 0) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0, time + cloud.randomOffset) - 0.5f;
            
            CloudState baseState = GetStateData(cloud, currentState);
            cloud.rect.anchoredPosition = baseState.position + new Vector2(noiseX, noiseY) * floatAmount;
            cloud.rect.localScale = baseState.scale * scaleMult;
        }
    }

    private class RuntimeCloudData
    {
        public RectTransform rect;
        public CloudState closedData;
        public CloudState openData;
        public CloudState panelData;
        public float randomOffset;
        public float centerDist;
        public float normalizedDist;
    }
}
