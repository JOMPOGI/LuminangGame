using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class CategoryListManager : MonoBehaviour
{
    [Header("List Settings")]
    public Transform contentParent;
    
    [Header("Data")]
    public TextAsset chaptersJsonData;

    [Header("Prefabs")]
    public GameObject chapterHeaderPrefab;
    public GameObject lessonRowPrefab;

    [Header("Language Specific Colours")]
    public Color ilokanoSelectedBgColor = new Color(0.2f, 0.5f, 1f);
    public Color cebuanoSelectedBgColor = new Color(1f, 0.8f, 0.2f);
    public Color normalBgColor = Color.clear;

    [Header("Chapter Sprites (one per chapter, in order)")]
    [Tooltip("Sprites for the background behind each chapter number. Leave empty to use prefab default.")]
    public Sprite[] chapterNumberBgSprites;
    [Tooltip("Icon sprites for each chapter header. Leave empty to use prefab default.")]
    public Sprite[] chapterIconSprites;
    [Tooltip("Background color for each chapter header row. Leave array empty to use prefab default.")]
    public Color[] chapterHeaderColors;

    [Header("Lesson Row Sprites (one per chapter, in order)")]
    [Tooltip("Sprites for the background behind the lesson number.")]
    public Sprite[] lessonNumberBgSprites;
    [Tooltip("Sprites for the background behind the checkmark when the lesson is COMPLETED.")]
    public Sprite[] lessonCompletedBgSprites;
    [Tooltip("Sprite for the background behind the checkmark when the lesson is INCOMPLETE (same for all).")]
    public Sprite lessonIncompleteBgSprite;
    [Tooltip("Color tint when lesson is COMPLETED.")]
    public Color lessonCompletedBgColor = Color.white;
    [Tooltip("Color tint when lesson is INCOMPLETE.")]
    public Color lessonIncompleteBgColor = Color.white;

    [Header("Callbacks")]
    public UnityEngine.Events.UnityEvent<string> onCategorySelected;

    private string _selectedCategory = "Greetings"; // Default to first lesson category

    private enum Language { Ilokano, Cebuano }
    private Language _activeLanguage = Language.Ilokano;

    [Header("Expand Animation")]
    [Tooltip("How fast the chapter rows slide open/close (seconds).")]
    public float expandDuration = 0.2f;
    [Tooltip("The height (in pixels) of each lesson row. Set this to match your LessonRowPrefab's height.")]
    public float rowHeight = 60f;

    [System.Serializable]
    private class ChapterInfo
    {
        public string title = default;
        public List<LessonInfo> lessons = default;
        public bool isExpanded = default;
    }

    [System.Serializable]
    private class LessonInfo
    {
        public string title = default;
        public string categoryName = default;
        public bool isCompleted = default;
    }

    [System.Serializable]
    private class ChaptersDataWrapper
    {
        public List<ChapterInfo> ilokanoChapters;
        public List<ChapterInfo> cebuanoChapters;
    }

    private ChaptersDataWrapper _chaptersData;
    private List<ChapterInfo> _chapters = new List<ChapterInfo>();

    // Tracks spawned lesson row GameObjects per chapter so we can animate them
    private Dictionary<int, List<GameObject>> _chapterLessonRows = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, Coroutine> _chapterAnimCoroutines = new Dictionary<int, Coroutine>();

    private void Awake()
    {
        InitializeChapters();
    }

    private void Start()
    {
        BuildCategoryList();
        StartCoroutine(ForceLayoutRebuild());
    }

    private void InitializeChapters()
    {
        if (chaptersJsonData != null)
        {
            _chaptersData = JsonUtility.FromJson<ChaptersDataWrapper>(chaptersJsonData.text);
        }
        
        if (_chaptersData == null)
        {
            Debug.LogError("Failed to parse Chapters JSON data.");
            _chaptersData = new ChaptersDataWrapper();
            _chaptersData.ilokanoChapters = new List<ChapterInfo>();
            _chaptersData.cebuanoChapters = new List<ChapterInfo>();
        }

        LoadActiveLanguageChapters();
    }

    private void LoadActiveLanguageChapters()
    {
        if (_chaptersData == null) return;
        
        _chapters = _activeLanguage == Language.Ilokano ? _chaptersData.ilokanoChapters : _chaptersData.cebuanoChapters;
        if (_chapters == null) _chapters = new List<ChapterInfo>();
    }

    public void ToggleChapter(int chapterIndex)
    {
        int listIndex = chapterIndex - 1;
        if (listIndex < 0 || listIndex >= _chapters.Count) return;

        _chapters[listIndex].isExpanded = !_chapters[listIndex].isExpanded;
        bool expand = _chapters[listIndex].isExpanded;

        // Update the header chevron
        ChapterHeaderUI header = GetChapterHeader(chapterIndex);
        if (header != null) header.UpdateChevron(expand);

        if (!_chapterLessonRows.ContainsKey(chapterIndex)) return;

        List<GameObject> rows = _chapterLessonRows[chapterIndex];

        // Stop any running anim for this chapter
        if (_chapterAnimCoroutines.ContainsKey(chapterIndex) && _chapterAnimCoroutines[chapterIndex] != null)
            StopCoroutine(_chapterAnimCoroutines[chapterIndex]);

        _chapterAnimCoroutines[chapterIndex] = StartCoroutine(AnimateRows(rows, expand));
    }

    private ChapterHeaderUI GetChapterHeader(int chapterIndex)
    {
        // Headers are the ChapterHeaderUI children of contentParent
        int headerCount = 0;
        foreach (Transform child in contentParent)
        {
            ChapterHeaderUI h = child.GetComponent<ChapterHeaderUI>();
            if (h != null)
            {
                headerCount++;
                if (headerCount == chapterIndex) return h;
            }
        }
        return null;
    }

    private IEnumerator AnimateRows(List<GameObject> rows, bool expand)
    {
        // Get natural height from first row
        float targetHeight = expand ? GetNaturalRowHeight(rows) : 0f;
        float startHeight = expand ? 0f : GetNaturalRowHeight(rows);

        // Make rows visible before animating open
        if (expand)
        {
            foreach (var row in rows)
            {
                row.SetActive(true);
                var le = GetOrAddLayoutElement(row);
                le.preferredHeight = 0f;
                le.minHeight = 0f;
            }
        }

        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / expandDuration);
            float h = Mathf.Lerp(startHeight, targetHeight, t);

            foreach (var row in rows)
            {
                var le = GetOrAddLayoutElement(row);
                le.preferredHeight = h;
                le.minHeight = 0f;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
            yield return null;
        }

        // Final state
        foreach (var row in rows)
        {
            if (!expand)
            {
                row.SetActive(false);
                var le = row.GetComponent<LayoutElement>();
                if (le != null) { le.preferredHeight = -1f; le.minHeight = -1f; }
            }
            else
            {
                // Keep the height locked at rowHeight so the layout doesn't snap back
                var le = GetOrAddLayoutElement(row);
                le.preferredHeight = rowHeight;
                le.minHeight = 0f;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
    }

    private float GetNaturalRowHeight(List<GameObject> rows)
    {
        return rowHeight;
    }

    private LayoutElement GetOrAddLayoutElement(GameObject go)
    {
        LayoutElement le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();
        return le;
    }

    private IEnumerator ForceLayoutRebuild()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
    }

    public void BuildCategoryList()
    {
        if (contentParent == null || chapterHeaderPrefab == null || lessonRowPrefab == null) return;

        // Clear everything
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        _chapterLessonRows.Clear();
        _chapterAnimCoroutines.Clear();

        Color selectedBg = GetSelectedBgColor();
        int chapterNum = 1;

        foreach (var chapter in _chapters)
        {
            // Spawn Chapter Header
            GameObject headerObj = Instantiate(chapterHeaderPrefab, contentParent, false);
            ChapterHeaderUI headerUI = headerObj.GetComponent<ChapterHeaderUI>();
            if (headerUI != null)
            {
                int completedCount = chapter.lessons.FindAll(l => l.isCompleted).Count;
                string progressStr = $"{completedCount}/{chapter.lessons.Count}";

                // Get per-chapter sprites (index = chapterNum - 1)
                int spriteIdx = chapterNum - 1;
                Sprite numBg = (chapterNumberBgSprites != null && spriteIdx < chapterNumberBgSprites.Length)
                    ? chapterNumberBgSprites[spriteIdx] : null;
                Sprite icon = (chapterIconSprites != null && spriteIdx < chapterIconSprites.Length)
                    ? chapterIconSprites[spriteIdx] : null;
                Color? headerColor = (chapterHeaderColors != null && spriteIdx < chapterHeaderColors.Length)
                    ? chapterHeaderColors[spriteIdx] : (Color?)null;

                headerUI.Setup(chapterNum, chapter.title, progressStr, chapter.isExpanded, this, numBg, icon, headerColor);
            }

            // Always spawn ALL lesson rows (even for collapsed chapters)
            // Collapsed rows start hidden via SetActive(false)
            List<GameObject> lessonRows = new List<GameObject>();
            int lessonNum = 1;
            foreach (var lesson in chapter.lessons)
            {
                GameObject lessonObj = Instantiate(lessonRowPrefab, contentParent, false);
                LessonRowUI lessonUI = lessonObj.GetComponent<LessonRowUI>();
                if (lessonUI != null)
                {
                    bool isSelected = (lesson.categoryName == _selectedCategory);
                    lessonUI.selectedBgColor = selectedBg;
                    lessonUI.normalBgColor = normalBgColor;

                    int spriteIdx = chapterNum - 1;
                    Sprite rowNumBg = (lessonNumberBgSprites != null && spriteIdx < lessonNumberBgSprites.Length)
                        ? lessonNumberBgSprites[spriteIdx] : null;
                    
                    Sprite rowCheckBg = lesson.isCompleted 
                        ? ((lessonCompletedBgSprites != null && spriteIdx < lessonCompletedBgSprites.Length) ? lessonCompletedBgSprites[spriteIdx] : null)
                        : lessonIncompleteBgSprite;
                    
                    Color rowCheckColor = lesson.isCompleted ? lessonCompletedBgColor : lessonIncompleteBgColor;

                    lessonUI.Setup(lessonNum, lesson.title, lesson.categoryName, lesson.isCompleted, isSelected, SelectCategory, rowNumBg, rowCheckBg, rowCheckColor);
                }

                // Collapsed chapters start hidden
                lessonObj.SetActive(chapter.isExpanded);
                lessonRows.Add(lessonObj);
                lessonNum++;
            }

            _chapterLessonRows[chapterNum] = lessonRows;
            chapterNum++;
        }
    }

    public void SelectCategory(string categoryName)
    {
        _selectedCategory = categoryName;
        onCategorySelected?.Invoke(_selectedCategory);
        RefreshLessonSelectionVisuals();
    }

    // Call this from the language card buttons (Ilokano / Cebuano)
    public void SetActiveLanguage(string languageName)
    {
        if (languageName.Equals("Ilokano", System.StringComparison.OrdinalIgnoreCase))
            _activeLanguage = Language.Ilokano;
        else if (languageName.Equals("Cebuano", System.StringComparison.OrdinalIgnoreCase))
            _activeLanguage = Language.Cebuano;
        else
            _activeLanguage = Language.Ilokano;

        LoadActiveLanguageChapters();
        BuildCategoryList();
        StartCoroutine(ForceLayoutRebuild());
    }

    private Color GetSelectedBgColor()
    {
        return _activeLanguage == Language.Ilokano ? ilokanoSelectedBgColor : cebuanoSelectedBgColor;
    }

    private void RefreshLessonSelectionVisuals()
    {
        Color selectedBg = GetSelectedBgColor();
        foreach (Transform child in contentParent)
        {
            LessonRowUI lessonUI = child.GetComponent<LessonRowUI>();
            if (lessonUI != null)
            {
                lessonUI.selectedBgColor = selectedBg;
                lessonUI.SetSelected(lessonUI.CategoryName == _selectedCategory);
            }
        }
    }
}
