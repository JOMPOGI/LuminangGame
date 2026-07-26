using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script for the LearningsList prefab inside LearningsScrollView.
///
/// Attach to the LearningsList prefab root.
/// Children expected:
///   IconBullet   (Image)            ← bullet icon
///   LessonDescri (TextMeshProUGUI)  ← the learning text
/// </summary>
public class LearningItemRow : MonoBehaviour
{
    [Tooltip("The IconBullet Image child.")]
    public Image iconBullet;

    [Tooltip("The LessonDescri TMP text child.")]
    public TextMeshProUGUI lessonDescriptionText;

    public void Setup(string learningText, Sprite bulletSprite)
    {
        if (lessonDescriptionText != null)
            lessonDescriptionText.text = learningText;

        if (iconBullet != null && bulletSprite != null)
            iconBullet.sprite = bulletSprite;
    }
}
