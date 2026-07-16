using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the Ilokano and Cebuano language card buttons in the LeaderboardGroup.
/// The active card restores its original designed colors with a smooth fade.
/// The inactive card smoothly fades to gray.
/// Attach this script to the LanguageCardGroup object.
/// </summary>
public class LanguageCardManager : MonoBehaviour
{
    [System.Serializable]
    public class LanguageCard
    {
        public string languageName;
        public Button cardButton;
        [Tooltip("All Image components inside this card to tint (background, icons, etc.)")]
        public List<Image> images = new List<Image>();
        [Tooltip("All TMP text components inside this card to tint")]
        public List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

        // Original colors saved at runtime — do NOT assign these in Inspector
        [HideInInspector] public List<Color> originalImageColors = new List<Color>();
        [HideInInspector] public List<Color> originalTextColors  = new List<Color>();
    }

    [Header("Language Cards")]
    public LanguageCard ilokanoCard;
    public LanguageCard cebuanoCard;

    [Header("Inactive Color")]
    [Tooltip("Tint applied to the inactive/unselected card. Active card always uses its original designed colors.")]
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("Transition")]
    [Tooltip("How long the fade between active and inactive takes in seconds.")]
    public float fadeDuration = 0.25f;

    [Header("Callbacks")]
    [Tooltip("Drag CategoryListManager here so it updates its button colors when a card is selected.")]
    public CategoryListManager categoryListManager;
    [Tooltip("Drag WordsListManager here so the word list updates when a card is selected.")]
    public WordsListManager wordsListManager;

    private string _activeLanguage = "Ilokano"; // default
    private Coroutine _iloFade;
    private Coroutine _cebFade;

    private void Start()
    {
        // Save original colors BEFORE any tinting
        SaveOriginalColors(ilokanoCard);
        SaveOriginalColors(cebuanoCard);

        // Wire up button clicks
        if (ilokanoCard?.cardButton != null)
            ilokanoCard.cardButton.onClick.AddListener(() => SelectLanguage("Ilokano"));

        if (cebuanoCard?.cardButton != null)
            cebuanoCard.cardButton.onClick.AddListener(() => SelectLanguage("Cebuano"));

        // Apply default selection visuals instantly (no fade on startup)
        ApplyCardColorInstant(ilokanoCard, true);
        ApplyCardColorInstant(cebuanoCard, false);
    }

    public void SelectLanguage(string languageName)
    {
        _activeLanguage = languageName;

        // Smoothly fade both cards to their new states
        FadeCard(ref _iloFade, ilokanoCard, _activeLanguage == "Ilokano");
        FadeCard(ref _cebFade, cebuanoCard, _activeLanguage == "Cebuano");

        // Notify CategoryListManager to update its button colours
        if (categoryListManager != null)
            categoryListManager.SetActiveLanguage(languageName);

        // Notify WordsListManager to refresh the words list
        if (wordsListManager != null)
            wordsListManager.SetLanguage(languageName);
    }

    private void FadeCard(ref Coroutine handle, LanguageCard card, bool isActive)
    {
        if (handle != null) StopCoroutine(handle);
        handle = StartCoroutine(FadeCardRoutine(card, isActive));
    }

    private IEnumerator FadeCardRoutine(LanguageCard card, bool isActive)
    {
        if (card == null) yield break;

        // Capture starting colors
        var startImgColors = new List<Color>();
        var startTxtColors = new List<Color>();
        for (int i = 0; i < card.images.Count; i++)
            startImgColors.Add(card.images[i] != null ? card.images[i].color : Color.white);
        for (int i = 0; i < card.texts.Count; i++)
            startTxtColors.Add(card.texts[i] != null ? card.texts[i].color : Color.white);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);

            for (int i = 0; i < card.images.Count; i++)
            {
                if (card.images[i] == null) continue;
                Color target = isActive ? card.originalImageColors[i] : inactiveColor;
                card.images[i].color = Color.Lerp(startImgColors[i], target, t);
            }

            for (int i = 0; i < card.texts.Count; i++)
            {
                if (card.texts[i] == null) continue;
                Color target = isActive ? card.originalTextColors[i] : inactiveColor;
                card.texts[i].color = Color.Lerp(startTxtColors[i], target, t);
            }

            yield return null;
        }

        // Snap to final values
        ApplyCardColorInstant(card, isActive);
    }

    private void SaveOriginalColors(LanguageCard card)
    {
        if (card == null) return;

        card.originalImageColors.Clear();
        foreach (var img in card.images)
            card.originalImageColors.Add(img != null ? img.color : Color.white);

        card.originalTextColors.Clear();
        foreach (var txt in card.texts)
            card.originalTextColors.Add(txt != null ? txt.color : Color.white);
    }

    private void ApplyCardColorInstant(LanguageCard card, bool isActive)
    {
        if (card == null) return;

        for (int i = 0; i < card.images.Count; i++)
        {
            if (card.images[i] == null) continue;
            card.images[i].color = isActive ? card.originalImageColors[i] : inactiveColor;
        }

        for (int i = 0; i < card.texts.Count; i++)
        {
            if (card.texts[i] == null) continue;
            card.texts[i].color = isActive ? card.originalTextColors[i] : inactiveColor;
        }
    }
}
