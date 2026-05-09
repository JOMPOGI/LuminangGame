using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the visual display of the Dialogue System.
/// Supports Next/Prev navigation, typewriter skip, and button press animations.
/// </summary>
public class DialogueUIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject dialoguePanel;
    public Transform choicesContainer;

    [Header("Text Elements")]
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Prefabs")]
    public GameObject choiceButtonPrefab;

    [Header("Optional UI")]
    public GameObject movementUI;
    public GameObject choicesGroup;

    [Header("Navigation Buttons")]
    [Tooltip("The 'Next >>' button. Assign in Inspector.")]
    public Button nextButton;
    [Tooltip("The '<< Prev' button. Assign in Inspector.")]
    public Button prevButton;

    [Header("Panel Pop-in Animation")]
    public float panelPopDuration = 0.35f;
    public AnimationCurve panelPopCurve = new AnimationCurve(
        new Keyframe(0f,    0f,   0f, 3f),
        new Keyframe(0.65f, 1.08f, 0f, 0f),
        new Keyframe(1f,    1f,   0f, 0f)
    );

    [Header("Choices Curtain Drop Animation")]
    public float curtainDropDuration = 0.5f;
    public float curtainDelay        = 0.1f;

    [Header("Typewriter Effect")]
    [Tooltip("Seconds per character. Set to 0 to show instantly.")]
    public float typingSpeed = 0.02f;

    [Header("Button Press Animation")]
    [Tooltip("How much the button squishes on press (0.85 = 15% smaller).")]
    public float buttonPressScale    = 0.85f;
    public float buttonAnimDuration  = 0.12f;

    // ── Private State ────────────────────────────────────────────────
    private List<GameObject>              _activeChoiceButtons = new List<GameObject>();
    private Coroutine                     _showSequenceCoroutine;
    private bool                          _isTyping  = false;
    private bool                          _skipTyping = false;
    private string                        _fullText  = "";
    private System.Action<DialogueChoice> _onChoiceSelected;
    private List<DialogueChoice>          _currentChoices = new List<DialogueChoice>();

    void Awake()
    {
        HideDialogue();

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);

        if (prevButton != null)
            prevButton.onClick.AddListener(OnPrevClicked);
    }

    // ─────────────────────────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Displays a dialogue node. Called by DialogueManager.
    /// </summary>
    public void DisplayNode(DialogueNode node, System.Action<DialogueChoice> onChoiceSelected)
    {
        _onChoiceSelected = onChoiceSelected;
        _currentChoices   = node.choices;
        _fullText         = node.dialogueText;
        _skipTyping       = false;

        if (speakerNameText != null)
            speakerNameText.text = string.IsNullOrEmpty(node.speakerName) ? "" : node.speakerName;

        // Clear text immediately so no placeholder shows during pop-in
        if (dialogueText != null) dialogueText.text = "";

        ClearChoices();

        // Spawn choice buttons ONLY if there are multiple choices branching out
        if (node.choices != null && node.choices.Count > 1)
        {
            foreach (var choice in node.choices)
            {
                GameObject obj = Instantiate(choiceButtonPrefab, choicesContainer);
                obj.SetActive(true);
                _activeChoiceButtons.Add(obj);

                var btnText = obj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = choice.choiceText;

                var btn = obj.GetComponent<Button>();
                if (btn != null)
                {
                    DialogueChoice cached = choice;
                    btn.onClick.AddListener(() =>
                    {
                        StartCoroutine(ButtonPressAnim(btn.transform));
                        _onChoiceSelected?.Invoke(cached);
                    });
                }
            }
        }

        // Show/hide Next button depending on choice count
        // If multiple choices exist, player must pick one — Next is hidden
        bool hasMultipleChoices = node.choices.Count > 1;
        if (nextButton != null)
            nextButton.gameObject.SetActive(!hasMultipleChoices);

        ShowDialogue(true);
    }

    /// <summary>
    /// Called by DialogueManager to update whether the Prev button is interactable.
    /// </summary>
    public void SetNavigation(bool canGoBack)
    {
        // Hide completely on first node, show when there is history
        if (prevButton != null)
            prevButton.gameObject.SetActive(canGoBack);
    }

    public void ShowDialogue(bool show)
    {
        if (show)
        {
            if (_showSequenceCoroutine != null) StopCoroutine(_showSequenceCoroutine);
            bool isAlreadyOpen = dialoguePanel.activeSelf;
            _showSequenceCoroutine = StartCoroutine(ShowSequence(isAlreadyOpen));
        }
        else
        {
            if (_showSequenceCoroutine != null) StopCoroutine(_showSequenceCoroutine);
            _showSequenceCoroutine = null;
            _isTyping   = false;
            _skipTyping = false;

            dialoguePanel.SetActive(false);
            if (choicesGroup != null)
            {
                choicesGroup.SetActive(false);
                choicesGroup.transform.localScale = new Vector3(1, 0, 1);
            }
        }

        if (movementUI != null) movementUI.SetActive(!show);
    }

    public void HideDialogue()
    {
        ShowDialogue(false);
        ClearChoices();
    }

    /// <summary>
    /// Hides only the choices area and dialogue panel visuals WITHOUT
    /// showing the movement UI. Used during wrong-answer animations
    /// where the player is still considered to be "in dialogue".
    /// </summary>
    public void HideChoicesOnly()
    {
        if (_showSequenceCoroutine != null) StopCoroutine(_showSequenceCoroutine);
        _isTyping   = false;
        _skipTyping = false;

        if (choicesGroup != null)
        {
            choicesGroup.SetActive(false);
            choicesGroup.transform.localScale = new Vector3(1, 0, 1);
        }

        // NOTE: We intentionally do NOT call movementUI.SetActive(true) here.
        // The player is still in dialogue — UI should stay hidden.
        ClearChoices();
    }

    // ─────────────────────────────────────────────────────────────────
    // Button Handlers
    // ─────────────────────────────────────────────────────────────────

    private void OnNextClicked()
    {
        StartCoroutine(ButtonPressAnim(nextButton.transform));

        if (_isTyping)
        {
            // First click: skip the typewriter, show full text immediately
            _skipTyping = true;
        }
        else
        {
            // Second click (or first if typing was instant):
            // Auto-advance if there is 0 or 1 choice
            if (_currentChoices.Count == 0)
            {
                _onChoiceSelected?.Invoke(null); // Ends dialogue
            }
            else if (_currentChoices.Count == 1)
            {
                _onChoiceSelected?.Invoke(_currentChoices[0]);
            }
            // If multiple choices exist, Next is hidden so this won't fire
        }
    }

    private void OnPrevClicked()
    {
        StartCoroutine(ButtonPressAnim(prevButton.transform));
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.GoToPreviousNode();
    }

    // ─────────────────────────────────────────────────────────────────
    // Coroutines
    // ─────────────────────────────────────────────────────────────────

    private IEnumerator ShowSequence(bool isAlreadyOpen)
    {
        if (choicesGroup != null) choicesGroup.SetActive(false);
        dialoguePanel.SetActive(true);

        if (!isAlreadyOpen)
            yield return StartCoroutine(PopInPanel());
        else
        {
            dialoguePanel.transform.localScale = Vector3.one;
            var cg = dialoguePanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }

        if (typingSpeed > 0)
            yield return StartCoroutine(TypeText(_fullText));
        else
            if (dialogueText != null) dialogueText.text = _fullText;

        // Only show the choices area when there are actual choices to pick from
        if (_activeChoiceButtons.Count > 0 && choicesGroup != null)
        {
            choicesGroup.SetActive(true);
            if (curtainDelay > 0) yield return new WaitForSeconds(curtainDelay);
            yield return StartCoroutine(CurtainDrop());
        }
    }

    private IEnumerator PopInPanel()
    {
        var cg = dialoguePanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = dialoguePanel.AddComponent<CanvasGroup>();

        Vector3 startScale = new Vector3(0.7f, 0.7f, 1f);
        dialoguePanel.transform.localScale = startScale;
        cg.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < panelPopDuration)
        {
            elapsed += Time.deltaTime;
            float t      = Mathf.Clamp01(elapsed / panelPopDuration);
            float curved = panelPopCurve.Evaluate(t);
            dialoguePanel.transform.localScale = Vector3.LerpUnclamped(startScale, Vector3.one, curved);
            cg.alpha = Mathf.Clamp01(t / 0.5f);
            yield return null;
        }

        dialoguePanel.transform.localScale = Vector3.one;
        cg.alpha = 1f;
    }

    private IEnumerator TypeText(string sentence)
    {
        _isTyping   = true;
        _skipTyping = false;

        if (dialogueText != null)
        {
            dialogueText.text = "";
            foreach (char c in sentence.ToCharArray())
            {
                if (_skipTyping)
                {
                    // Skip pressed — jump to full text immediately
                    dialogueText.text = sentence;
                    break;
                }
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        _isTyping   = false;
        _skipTyping = false;
    }

    private IEnumerator CurtainDrop()
    {
        Vector3 start = new Vector3(1f, 0f, 1f);
        Vector3 end   = Vector3.one;
        choicesGroup.transform.localScale = start;

        float elapsed = 0f;
        while (elapsed < curtainDropDuration)
        {
            elapsed += Time.deltaTime;
            float t     = Mathf.Clamp01(elapsed / curtainDropDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            choicesGroup.transform.localScale = Vector3.Lerp(start, end, eased);
            yield return null;
        }
        choicesGroup.transform.localScale = end;
    }

    private IEnumerator ButtonPressAnim(Transform btn)
    {
        if (btn == null) yield break;

        Vector3 original  = Vector3.one;
        Vector3 squish    = Vector3.one * buttonPressScale;
        float   half      = buttonAnimDuration / 2f;
        float   elapsed   = 0f;

        // Squish down
        while (elapsed < half)
        {
            if (btn == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            btn.localScale = Vector3.Lerp(original, squish, elapsed / half);
            yield return null;
        }

        elapsed = 0f;

        // Bounce back
        while (elapsed < half)
        {
            if (btn == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            btn.localScale = Vector3.Lerp(squish, original, elapsed / half);
            yield return null;
        }

        if (btn != null) btn.localScale = original;
    }


    private void ClearChoices()
    {
        foreach (var btn in _activeChoiceButtons)
            if (btn != null) Destroy(btn);
        _activeChoiceButtons.Clear();
    }
}
