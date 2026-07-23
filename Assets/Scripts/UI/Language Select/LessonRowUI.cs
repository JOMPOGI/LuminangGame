using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class LessonRowUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public TextMeshProUGUI lessonNumberText;
    public TextMeshProUGUI lessonTitleText;
    public GameObject checkmarkBadge;
    public Image backgroundImage;
    public Image lessonNumberBgImage;
    public Image checkUncheckBgImage;
    public GameObject checkIconObject;

    [Header("Colors")]
    public Color selectedBgColor = new Color(0.85f, 0.75f, 0.55f, 1f);
    public Color normalBgColor = Color.clear;
    public Color selectedTextColor = new Color(0.55f, 0.25f, 0.05f, 1f);
    public Color normalTextColor = new Color(0.18f, 0.12f, 0.06f, 1f);

    private string _categoryName;
    public string CategoryName => _categoryName;
    private Action<string> _onSelect;

    public void Setup(int lessonNumber, string title, string categoryName, bool isCompleted, bool isSelected, Action<string> onSelect,
                      Sprite numberBgSprite = null, Sprite checkBgSprite = null, Color? checkBgColor = null)
    {
        _categoryName = categoryName;
        _onSelect = onSelect;

        if (lessonNumberText != null) lessonNumberText.text = lessonNumber.ToString();
        if (lessonTitleText != null) lessonTitleText.text = title;
        
        if (lessonNumberBgImage != null && numberBgSprite != null)
            lessonNumberBgImage.sprite = numberBgSprite;

        if (checkUncheckBgImage != null)
        {
            if (checkBgSprite != null) checkUncheckBgImage.sprite = checkBgSprite;
            if (checkBgColor.HasValue) checkUncheckBgImage.color = checkBgColor.Value;
        }

        // The overall badge/background should always stay visible
        if (checkmarkBadge != null) checkmarkBadge.SetActive(true);

        // ONLY the check mark icon itself toggles based on completion
        if (checkIconObject != null) checkIconObject.SetActive(isCompleted);

        SetSelected(isSelected, instant: true);
    }

    public void SetSelected(bool isSelected, bool instant = false)
    {
        Color targetBg = isSelected ? selectedBgColor : normalBgColor;
        Color targetText = isSelected ? selectedTextColor : normalTextColor;
        FontStyles targetStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;

        if (lessonTitleText != null) lessonTitleText.fontStyle = targetStyle;

        // Apply instantly if flagged OR if the GameObject is inactive (can't run coroutines)
        if (instant || !gameObject.activeInHierarchy)
        {
            if (backgroundImage != null) backgroundImage.color = targetBg;
            if (lessonTitleText != null) lessonTitleText.color = targetText;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(LerpColors(targetBg, targetText));
        }
    }

    private System.Collections.IEnumerator LerpColors(Color targetBg, Color targetText)
    {
        float duration = 0.2f;
        float elapsed = 0f;

        Color startBg = backgroundImage != null ? backgroundImage.color : targetBg;
        Color startText = lessonTitleText != null ? lessonTitleText.color : targetText;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (backgroundImage != null) backgroundImage.color = Color.Lerp(startBg, targetBg, t);
            if (lessonTitleText != null) lessonTitleText.color = Color.Lerp(startText, targetText, t);

            yield return null;
        }

        if (backgroundImage != null) backgroundImage.color = targetBg;
        if (lessonTitleText != null) lessonTitleText.color = targetText;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onSelect?.Invoke(_categoryName);
    }
}
