using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Luminang.UI.Announcements
{
    [RequireComponent(typeof(Image))]
    public class AnnouncementTabButton : MonoBehaviour, IPointerClickHandler
    {
        public AnnouncementTabGroup TabGroup;
        
        [Header("UI References")]
        public Image BackgroundImage;
        public TextMeshProUGUI TabText;

        [Header("Sprites")]
        public Sprite NormalSprite;
        public Sprite ActiveSprite;

        [Header("Colors")]
        public Color NormalTextColor = Color.black;
        public Color ActiveTextColor = Color.white;

        [Header("Tab Identity")]
        public string TabName;
        private int _currentCount = 0;

        private void Awake()
        {
            if (BackgroundImage == null) BackgroundImage = GetComponent<Image>();
            
            // Register with the tab group
            if (TabGroup != null)
            {
                TabGroup.Subscribe(this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (TabGroup != null)
            {
                TabGroup.OnTabSelected(this);
            }
        }

        public void Select()
        {
            if (BackgroundImage != null && ActiveSprite != null)
                BackgroundImage.sprite = ActiveSprite;
                
            if (TabText != null)
                TabText.color = ActiveTextColor;
        }

        public void Deselect()
        {
            if (BackgroundImage != null && NormalSprite != null)
                BackgroundImage.sprite = NormalSprite;
                
            if (TabText != null)
                TabText.color = NormalTextColor;
        }

        public void UpdateCount(int count)
        {
            _currentCount = count;
            if (TabText != null)
            {
                TabText.text = $"{TabName} ({_currentCount})";
            }
        }
    }
}
