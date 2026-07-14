using UnityEngine;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// Plays the regional intro cutscene video between Kalaw's pre-video dialogue
/// and the post-video "So... What did you think?" dialogue chain.
///
/// SETUP:
///   1. Attach this to a GameObject in the scene (e.g., "RegionalCutsceneController").
///   2. On Kalaw's InteractableNPC component → dialogueEvents list:
///      Add entry: eventName = "PlayRegionalIntro", onEventTriggered → PlayCutscene()
///   3. Assign videoPanelRoot (full-screen UI panel on the Canvas).
///   4. Assign videoPlayer (VideoPlayer component, renders to RawImage via RenderTexture).
///   5. Assign postVideoDialogueNode → Kalaw_PostVideo_SoWhatDidYouThink.
///   6. Assign kalawNPC → the Kalaw InteractableNPC in the scene.
///   7. Assign introVideoClip → [Ilocos_Intro_Cutscene.mp4] placeholder when available.
///
/// FLOW:
///   Kalaw_IntroduceSELF5 endEventName fires
///   → HandleDialogueEvent("PlayRegionalIntro")
///   → PlayCutscene()
///   → fade to black → play video → fade back
///   → ForceStartDialogue(postVideoDialogueNode)
/// </summary>
public class RegionalCutsceneController : MonoBehaviour
{
    public static RegionalCutsceneController Instance { get; private set; }

    [Header("Video Settings")]
    [Tooltip("[PLACEHOLDER] Regional intro video clip. Assign Ilocos_Intro_Cutscene.mp4 or Cebu_Intro_Cutscene.mp4.")]
    public VideoClip introVideoClip;

    [Tooltip("The VideoPlayer component. Set its targetTexture to a RenderTexture assigned to the VideoRawImage.")]
    public VideoPlayer videoPlayer;

    [Tooltip("Seconds to wait if no VideoClip is assigned (placeholder mode).")]
    public float fallbackWaitTime = 8f;

    [Header("UI References")]
    [Tooltip("The full-screen panel (black background + VideoRawImage) that covers gameplay during the cutscene. Starts inactive.")]
    public GameObject videoPanelRoot;

    [Tooltip("CanvasGroup on the videoPanelRoot. Auto-fetched if not assigned.")]
    public CanvasGroup videoPanelCanvasGroup;

    [Header("Fade Settings")]
    [Tooltip("Duration of the fade-to-black transition before the video.")]
    public float fadeInDuration  = 0.8f;

    [Tooltip("Duration of the fade-back-to-gameplay transition after the video.")]
    public float fadeOutDuration = 0.8f;

    [Header("Dialogue Continuation")]
    [Tooltip("The first DialogueNode to play automatically after the video ends. Assign Kalaw_PostVideo_SoWhatDidYouThink.")]
    public DialogueNode postVideoDialogueNode;

    [Tooltip("The Kalaw InteractableNPC in the scene that delivers the post-video dialogue.")]
    public InteractableNPC kalawNPC;

    // ── Lifecycle ──────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (videoPanelRoot != null)
        {
            videoPanelRoot.SetActive(false);

            if (videoPanelCanvasGroup == null)
                videoPanelCanvasGroup = videoPanelRoot.GetComponent<CanvasGroup>();
            if (videoPanelCanvasGroup == null)
                videoPanelCanvasGroup = videoPanelRoot.AddComponent<CanvasGroup>();
        }
    }

    // ── Public API ─────────────────────────────────────────────────────

    /// <summary>
    /// Entry point. Wire this to the "PlayRegionalIntro" dialogue event on Kalaw's
    /// InteractableNPC → dialogueEvents list in the Inspector.
    /// </summary>
    public void PlayCutscene()
    {
        StartCoroutine(CutsceneRoutine());
    }

    // ── Internal Sequence ──────────────────────────────────────────────

    private IEnumerator CutsceneRoutine()
    {
        Debug.Log("[RegionalCutsceneController] Starting regional intro cutscene.");

        // ── Step 1: Activate panel and fade to black ─────────────────
        if (videoPanelRoot != null)
        {
            videoPanelRoot.SetActive(true);
            if (videoPanelCanvasGroup != null) videoPanelCanvasGroup.alpha = 0f;
        }

        yield return StartCoroutine(FadePanel(0f, 1f, fadeInDuration));

        // ── Step 2: Play regional intro video ────────────────────────
        if (videoPlayer != null && introVideoClip != null)
        {
            videoPlayer.clip = introVideoClip;

            if (!videoPlayer.isPrepared)
            {
                videoPlayer.Prepare();

                float prepTimeout = 10f;
                float prepElapsed = 0f;
                while (!videoPlayer.isPrepared && prepElapsed < prepTimeout)
                {
                    prepElapsed += Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            videoPlayer.Play();
            Debug.Log("[RegionalCutsceneController] Video playing...");

            while (videoPlayer.isPlaying)
                yield return null;

            // Small buffer to ensure the last frame is visible
            yield return new WaitForSecondsRealtime(0.5f);
        }
        else
        {
            // ── Placeholder mode: no clip assigned yet ──────────────
            Debug.Log($"[RegionalCutsceneController] No VideoClip assigned. " +
                      $"[PLACEHOLDER: Ilocos_Intro_Cutscene.mp4] — waiting {fallbackWaitTime}s.");
            yield return new WaitForSecondsRealtime(fallbackWaitTime);
        }

        // ── Step 3: Fade back to gameplay ────────────────────────────
        yield return StartCoroutine(FadePanel(1f, 0f, fadeOutDuration));

        if (videoPanelRoot != null)
            videoPanelRoot.SetActive(false);

        // ── Step 4: Auto-start post-video dialogue ───────────────────
        if (kalawNPC != null && postVideoDialogueNode != null)
        {
            Debug.Log("[RegionalCutsceneController] Starting post-video dialogue: " + postVideoDialogueNode.name);
            kalawNPC.ForceStartDialogue(postVideoDialogueNode);
        }
        else
        {
            Debug.LogWarning("[RegionalCutsceneController] Cannot start post-video dialogue. " +
                             "Assign kalawNPC and postVideoDialogueNode in the Inspector.");
        }
    }

    /// <summary>
    /// Lerps the video panel's CanvasGroup alpha using an ease-out curve.
    /// Uses unscaled time so it works regardless of Time.timeScale.
    /// </summary>
    private IEnumerator FadePanel(float startAlpha, float targetAlpha, float duration)
    {
        if (videoPanelCanvasGroup == null) yield break;

        float elapsed = 0f;
        videoPanelCanvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t     = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f); // ease-out cubic
            videoPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, eased);
            yield return null;
        }

        videoPanelCanvasGroup.alpha = targetAlpha;
    }
}
