using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One answer tile in the TiptipInlineQuiz panel.
/// Handles correct/wrong visual feedback with color flash and scale animation.
/// </summary>
[RequireComponent(typeof(Button))]
public class TiptipQuizChoice : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────
    // Inspector
    // ─────────────────────────────────────────────────────────

    [Header("References")]
    public TextMeshProUGUI labelText;
    public Image background;

    [Header("Colors")]
    public Color normalColor    = new Color(0.95f, 0.88f, 0.70f, 1f);  // parchment
    public Color correctColor   = new Color(0.33f, 0.80f, 0.33f, 1f);  // green
    public Color wrongColor     = new Color(0.85f, 0.25f, 0.20f, 1f);  // red
    public Color disabledColor  = new Color(0.60f, 0.60f, 0.60f, 0.5f); // grey

    [Header("Animation")]
    [Tooltip("Duration of the color flash and scale pulse (seconds).")]
    public float animDuration = 0.3f;

    // ─────────────────────────────────────────────────────────
    // State
    // ─────────────────────────────────────────────────────────

    public bool IsCorrect { get; private set; }
    private string _phrase;
    private Button _button;
    private Action<TiptipQuizChoice> _onSelected;

    // ─────────────────────────────────────────────────────────
    // Init
    // ─────────────────────────────────────────────────────────

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        if (background != null) background.color = normalColor;
    }

    /// <summary>
    /// Called by TiptipInlineQuiz to assign this tile's phrase, correctness, and callback.
    /// </summary>
    public void Setup(string phrase, bool isCorrect, Action<TiptipQuizChoice> onSelected)
    {
        _phrase     = phrase;
        IsCorrect   = isCorrect;
        _onSelected = onSelected;

        if (labelText != null) labelText.text = phrase;
        if (background != null) background.color = normalColor;
        _button.interactable = true;
        transform.localScale = Vector3.one;
    }

    public void SetInteractable(bool value)
    {
        _button.interactable = value;
        if (!value && background != null)
        {
            // Don't grey out if we're already showing correct/wrong color
        }
    }

    // ─────────────────────────────────────────────────────────
    // Feedback Animations
    // ─────────────────────────────────────────────────────────

    /// <summary>Flash green + scale up briefly to show the correct answer.</summary>
    public void PlayCorrect()
    {
        StopAllCoroutines();
        StartCoroutine(FlashAndPulse(correctColor, 1.08f));
    }

    /// <summary>Flash red + shake to show a wrong selection.</summary>
    public void PlayWrong()
    {
        StopAllCoroutines();
        StartCoroutine(FlashAndShake(wrongColor));
    }

    private IEnumerator FlashAndPulse(Color targetColor, float maxScale)
    {
        float elapsed = 0f;
        Color startColor = background != null ? background.color : normalColor;
        Vector3 startScale = transform.localScale;
        Vector3 peakScale  = Vector3.one * maxScale;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animDuration;
            if (background != null) background.color = Color.Lerp(startColor, targetColor, t);
            transform.localScale = Vector3.Lerp(startScale, peakScale, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }

        if (background != null) background.color = targetColor;
        transform.localScale = Vector3.one;
    }

    private IEnumerator FlashAndShake(Color targetColor)
    {
        if (background != null) background.color = targetColor;

        // Shake horizontally
        Vector3 origin = transform.localPosition;
        float shakeAmount = 8f;
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float x = Mathf.Sin(elapsed * 60f) * shakeAmount * (1f - elapsed / animDuration);
            transform.localPosition = origin + new Vector3(x, 0f, 0f);
            yield return null;
        }
        transform.localPosition = origin;
    }

    // ─────────────────────────────────────────────────────────
    // Click
    // ─────────────────────────────────────────────────────────

    private void OnClick()
    {
        _onSelected?.Invoke(this);
    }
}
