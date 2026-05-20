using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Luminang.UI.Minigames
{
    public class MatchingCard : MonoBehaviour
    {
        public enum CardSide { Left, Right }

        [Header("Settings")]
        public CardSide side;
        public string pairID;
        public string wordContent; // The text to be spoken for this card

        [Header("UI References")]
        public Image cardImage;
        public TextMeshProUGUI cardText;
        public RectTransform connectionPoint;
        public GameObject checkIcon;
        public GameObject crossIcon;

        [Header("State Colors")]
        public Color normalColor = Color.white;
        public Color selectedColor = new Color(0.9f, 0.9f, 0.7f); // Subtle highlight
        public Color matchedColor = new Color(0.8f, 1f, 0.8f);

        private Image mainImage;
        private MatchingGameManager manager;
        public bool IsMatched { get; private set; }

        private void Awake()
        {
            mainImage = GetComponent<Image>();
            manager = GetComponentInParent<MatchingGameManager>();
            
            if (checkIcon) checkIcon.SetActive(false);
            if (crossIcon) crossIcon.SetActive(false);
        }

        public void Setup(string id, string content, Sprite sprite = null)
        {
            pairID = id;
            wordContent = content;
            
            if (cardText) cardText.text = content;
            if (cardImage && sprite != null) cardImage.sprite = sprite;
            
            IsMatched = false;
        }

        public void OnClick()
        {
            if (IsMatched) return;
            manager.OnCardClicked(this);
        }

        public void SetSelected(bool selected)
        {
            if (IsMatched) return;
            if (mainImage) mainImage.color = selected ? selectedColor : normalColor;
        }

        public void SetMatched(bool success)
        {
            IsMatched = success;
            if (success)
            {
                if (mainImage) mainImage.color = matchedColor;
                if (checkIcon) checkIcon.SetActive(true);
            }
            else
            {
                // Play fail animation or show cross briefly
                StartCoroutine(ShowFailFeedback());
            }
        }

        private System.Collections.IEnumerator ShowFailFeedback()
        {
            if (crossIcon) crossIcon.SetActive(true);
            yield return new WaitForSeconds(1f);
            if (crossIcon) crossIcon.SetActive(false);
        }
    }
}
