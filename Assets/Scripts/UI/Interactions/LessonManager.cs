using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luminang.Database;

/// <summary>
/// Manages the visibility and state of the Lesson Panel.
/// Can be triggered by Dialogue Events or other game logic.
/// Fetches real vocabulary data from Supabase and populates the UI dynamically.
/// </summary>
public class LessonManager : MonoBehaviour
{
    public static LessonManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("The root object of the Lesson Panel prefab.")]
    public GameObject lessonPanel;
    [Tooltip("The text component for the lesson category title (e.g. 'Greetings').")]
    public TextMeshProUGUI categoryText;
    [Tooltip("The text component for the category description.")]
    public TextMeshProUGUI categoryDescriptionText;
    [Tooltip("The CanvasGroup for smooth fading (optional).")]
    public CanvasGroup lessonCanvasGroup;

    [Header("Dynamic Row Spawning")]
    [Tooltip("The Content transform inside the Scroll View where LessonRow prefabs will be spawned.")]
    public Transform rowContainer;
    [Tooltip("The LessonRow prefab to instantiate for each vocabulary item.")]
    public GameObject lessonRowPrefab;

    [Header("Buttons")]
    [Tooltip("The X/Close button on the header.")]
    public Button closeButton;
    [Tooltip("The Continue button at the bottom.")]
    public Button continueButton;

    [Header("Visual Enhancements")]
    [Tooltip("How long it takes to fade in/out (seconds).")]
    public float fadeDuration = 0.4f;
    [Tooltip("A dark semi-transparent image that covers the game world during lessons.")]
    public GameObject dimmerBackground;
    [Tooltip("The small loading animation (bouncing crystals) shown while data loads.")]
    public GameObject smallLoading;
    
    [Header("Language Settings")]
    [Tooltip("The language ID to fetch translations for. 1 = Ilokano, 2 = Cebuano, 3 = Maranao.")]
    public int languageId = 1;

    [Header("Quest Integration")]
    [Tooltip("Events to fire when the lesson is successfully closed.")]
    public UnityEngine.Events.UnityEvent onLessonComplete;

    private CanvasGroup _dimmerCG;
#pragma warning disable 0414
    private bool _isLessonActive = false;
#pragma warning restore 0414
    private List<GameObject> _spawnedRows = new List<GameObject>();

    void Awake()
    {
        Instance = this;
        SetupReferences();
    }

    private void SetupReferences()
    {
        if (lessonPanel != null)
        {
            lessonPanel.SetActive(false);
            if (lessonCanvasGroup == null) lessonCanvasGroup = lessonPanel.GetComponent<CanvasGroup>();
            if (lessonCanvasGroup != null) lessonCanvasGroup.alpha = 0f;
        }

        if (dimmerBackground != null)
        {
            dimmerBackground.SetActive(false);
            _dimmerCG = dimmerBackground.GetComponent<CanvasGroup>();
            if (_dimmerCG == null) _dimmerCG = dimmerBackground.AddComponent<CanvasGroup>();
            _dimmerCG.alpha = 0f;
        }

        // Wire up buttons
        if (closeButton != null)
            closeButton.onClick.AddListener(HideLesson);
        if (continueButton != null)
            continueButton.onClick.AddListener(HideLesson);
    }

    public void ShowLessonWithCategory(string category)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Calle_Crisologo")
        {
            Debug.Log("[LessonManager] Bypassing LessonManager in Calle Crisologo");
            _isLessonActive = false;
            if (lessonPanel != null) lessonPanel.SetActive(false);
            if (onLessonComplete != null)
                onLessonComplete.Invoke();
            if (MinigameManager.Instance != null)
                MinigameManager.Instance.HideMinigame();
            return;
        }

        if (lessonPanel == null) return;
        
        Debug.Log($"[LessonManager] ShowLesson requested for category: {category}");
        _isLessonActive = true;

        // Set the title immediately while data loads
        if (categoryText != null && !string.IsNullOrEmpty(category))
            categoryText.text = category;

        // Clear old rows
        ClearRows();

        // Show the loading crystals, hide the actual panel content
        if (smallLoading != null) smallLoading.SetActive(true);
        
        StopAllCoroutines();
        // Keep the lesson panel active but invisible (alpha=0) so HUD watchdog still fires
        lessonPanel.SetActive(true); // THE TRIGGER FOR THE WATCHDOG
        if (lessonCanvasGroup != null) lessonCanvasGroup.alpha = 0f;

        // Fetch data, then fade in once ready
        _ = FetchAndPopulate(category);
    }

    /// <summary>
    /// Fetches vocabulary data from Supabase and populates the lesson rows.
    /// </summary>
    private async Task FetchAndPopulate(string categoryName)
    {
        try
        {
            if (SupabaseManager.Instance == null || SupabaseManager.Instance.client == null)
            {
                Debug.LogError("[LessonManager] SupabaseManager not available!");
                return;
            }

            // 1. Find the category by name
            Debug.Log($"[LessonManager] Fetching category: {categoryName}");
            var categoryResponse = await SupabaseManager.Instance.client
                .From<LessonCategoryModel>()
                .Filter("name", Postgrest.Constants.Operator.Equals, categoryName)
                .Get();

            if (categoryResponse.Models.Count == 0)
            {
                Debug.LogError($"[LessonManager] Category '{categoryName}' not found in database!");
                return;
            }

            var category = categoryResponse.Models[0];
            string categoryId = category.Id;

            // Set the description
            if (categoryDescriptionText != null && !string.IsNullOrEmpty(category.Description))
                categoryDescriptionText.text = category.Description;

            // 2. Fetch all vocabulary for this category
            Debug.Log($"[LessonManager] Fetching vocabulary for category ID: {categoryId}");
            var vocabResponse = await SupabaseManager.Instance.client
                .From<VocabularyModel>()
                .Filter("category_id", Postgrest.Constants.Operator.Equals, categoryId)
                .Get();

            if (vocabResponse.Models.Count == 0)
            {
                Debug.LogWarning($"[LessonManager] No vocabulary found for category '{categoryName}'.");
                return;
            }

            Debug.Log($"[LessonManager] Found {vocabResponse.Models.Count} vocabulary items.");

            // 3. For each vocabulary item, fetch its translation and spawn a row
            foreach (var vocab in vocabResponse.Models)
            {
                // Fetch the translation for this vocab + language
                var translationResponse = await SupabaseManager.Instance.client
                    .From<VocabularyTranslationModel>()
                    .Filter("vocabulary_id", Postgrest.Constants.Operator.Equals, vocab.Id)
                    .Filter("language_id", Postgrest.Constants.Operator.Equals, languageId)
                    .Get();

                string translatedText = "";
                string audioUrl = "";

                if (translationResponse.Models.Count > 0)
                {
                    translatedText = translationResponse.Models[0].TranslatedText;
                    audioUrl = translationResponse.Models[0].AudioUrl;
                }

                // Spawn a LessonRow on the main thread
                SpawnRow(vocab, translatedText, audioUrl);
            }

            Debug.Log($"<color=green>[LessonManager] Successfully populated {vocabResponse.Models.Count} lesson rows!</color>");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[LessonManager] Error fetching lesson data: {ex.Message}");
        }

        // Data is ready (or failed) — hide loading, fade in the panel
        if (smallLoading != null) smallLoading.SetActive(false);
        StartCoroutine(FadeRoutine(true));
    }

    /// <summary>
    /// Instantiates a LessonRow prefab and fills it with data.
    /// </summary>
    private void SpawnRow(VocabularyModel vocab, string translatedText, string audioUrl)
    {
        if (lessonRowPrefab == null || rowContainer == null) return;

        GameObject row = Instantiate(lessonRowPrefab, rowContainer);
        row.SetActive(true);
        _spawnedRows.Add(row);

        // Find child text components by name
        // EnglishCell > EnglishText
        var englishText = FindChildTMP(row, "EnglishText");
        if (englishText != null) englishText.text = vocab.EnglishTerm ?? "";

        // IlokanoCell > IlokanoText
        var ilokanoText = FindChildTMP(row, "IlokanoText");
        if (ilokanoText != null) ilokanoText.text = translatedText;

        // MeaningText
        var meaningText = FindChildTMP(row, "MeaningText");
        if (meaningText != null) meaningText.text = vocab.MeaningEn ?? "";

        // UsesText
        var usesText = FindChildTMP(row, "UsesText");
        if (usesText != null) usesText.text = vocab.UsageEn ?? "";

        // Icon
        if (!string.IsNullOrEmpty(vocab.IconUrl) && vocab.IconUrl != "icons/none")
        {
            var iconTransform = row.transform.Find("EnglishCell/Icon");
            if (iconTransform != null)
            {
                var iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                    StartCoroutine(LoadIcon(iconImage, vocab.IconUrl));
            }
        }

        // AudioButton — wire up to play audio with press animation
        var audioButtonTransform = FindChildTransform(row, "AudioButton");
        if (audioButtonTransform != null)
        {
            var btn = audioButtonTransform.GetComponent<Button>();
            if (btn == null) btn = audioButtonTransform.gameObject.AddComponent<Button>();

            string capturedAudioUrl = audioUrl; // Capture for lambda
            Transform capturedTransform = audioButtonTransform; // Capture for animation
            btn.onClick.RemoveAllListeners();
            
            if (!string.IsNullOrEmpty(capturedAudioUrl) && capturedAudioUrl != "audio/none")
            {
                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(ButtonPressAnim(capturedTransform));
                    PlayAudio(capturedAudioUrl);
                });
                btn.interactable = true;
            }
            else
            {
                btn.interactable = false; // Grey out if no audio
            }
        }
    }

    /// <summary>
    /// Finds a TextMeshProUGUI component in any child by name (recursive).
    /// </summary>
    private TextMeshProUGUI FindChildTMP(GameObject parent, string childName)
    {
        var allTMP = parent.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var tmp in allTMP)
        {
            if (tmp.gameObject.name == childName) return tmp;
        }
        return null;
    }

    /// <summary>
    /// Finds a Transform in any child by name (recursive).
    /// </summary>
    private Transform FindChildTransform(GameObject parent, string childName)
    {
        var allTransforms = parent.GetComponentsInChildren<Transform>(true);
        foreach (var t in allTransforms)
        {
            if (t.gameObject.name == childName) return t;
        }
        return null;
    }

    /// <summary>
    /// Downloads an icon from a URL and sets it on an Image component.
    /// </summary>
    private IEnumerator LoadIcon(Image targetImage, string url)
    {
        // If it's a relative path, build the full Supabase storage URL
        if (!url.StartsWith("http"))
        {
            url = $"{SupabaseManager.Instance.supabaseUrl}/storage/v1/object/public/{url}";
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                targetImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogWarning($"[LessonManager] Failed to load icon: {url} — {request.error}");
            }
        }
    }

    /// <summary>
    /// Plays an audio clip from a URL.
    /// </summary>
    private void PlayAudio(string url)
    {
        // If it's a relative path, build the full Supabase storage URL
        if (!url.StartsWith("http"))
        {
            url = $"{SupabaseManager.Instance.supabaseUrl}/storage/v1/object/public/{url}";
        }

        StartCoroutine(PlayAudioCoroutine(url));
    }

    private IEnumerator PlayAudioCoroutine(string url)
    {
        AudioType audioType = AudioType.MPEG; // Default to MP3
        if (url.EndsWith(".wav")) audioType = AudioType.WAV;
        else if (url.EndsWith(".ogg")) audioType = AudioType.OGGVORBIS;

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                if (clip != null)
                {
                    // Play on a temporary AudioSource
                    AudioSource tempSource = gameObject.AddComponent<AudioSource>();
                    tempSource.clip = clip;
                    tempSource.Play();
                    Destroy(tempSource, clip.length + 0.5f);
                }
            }
            else
            {
                Debug.LogWarning($"[LessonManager] Failed to load audio: {url} — {request.error}");
            }
        }
    }

    /// <summary>
    /// Destroys all currently spawned lesson rows.
    /// </summary>
    private void ClearRows()
    {
        foreach (var row in _spawnedRows)
        {
            if (row != null) Destroy(row);
        }
        _spawnedRows.Clear();
    }

    /// <summary>
    /// Quick squish-and-bounce animation for button presses.
    /// </summary>
    private IEnumerator ButtonPressAnim(Transform btn)
    {
        if (btn == null) yield break;

        Vector3 original = btn.localScale;
        Vector3 squish = original * 0.85f;
        float half = 0.06f;
        float elapsed = 0f;

        while (elapsed < half)
        {
            if (btn == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            btn.localScale = Vector3.Lerp(original, squish, elapsed / half);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            if (btn == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            btn.localScale = Vector3.Lerp(squish, original, elapsed / half);
            yield return null;
        }

        if (btn != null) btn.localScale = original;
    }

    public void HideLesson()
    {
        _isLessonActive = false;

        if (lessonPanel != null && lessonPanel.activeSelf)
        {
            Debug.Log("[LessonManager] HideLesson requested.");
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(false));
        }
    }

    private IEnumerator FadeRoutine(bool show)
    {
        float targetAlpha = show ? 1f : 0f;
        float elapsed = 0f;

        float startLessonAlpha = (lessonCanvasGroup != null) ? lessonCanvasGroup.alpha : (show ? 0f : 1f);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;

            if (lessonCanvasGroup != null)
                lessonCanvasGroup.alpha = Mathf.Lerp(startLessonAlpha, targetAlpha, t);
            
            yield return null;
        }

        if (lessonCanvasGroup != null) lessonCanvasGroup.alpha = targetAlpha;

        if (!show)
        {
            ClearRows();
            lessonPanel.SetActive(false); // THE TRIGGER FOR THE WATCHDOG

            // Let MinigameManager.HideMinigame() handle CompleteMinigame() in all cases.
            // (MinigameManager now always calls CompleteMinigame when it cleans up.)
            if (MinigameManager.Instance != null)
                MinigameManager.Instance.HideMinigame();

            // Fire quest events!
            if (onLessonComplete != null)
                onLessonComplete.Invoke();
        }
    }
}
