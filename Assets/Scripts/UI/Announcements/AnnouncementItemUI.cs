using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Luminang.UI.Announcements
{
    public class AnnouncementItemUI : MonoBehaviour
    {
        [Header("UI References")]
        public Image IconImage;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DateText;
        public TextMeshProUGUI DetailsText;
        public Image StatusIndicator; // E.g., a dot showing if it's unread
        public Button RowButton;

        [Header("Icons configuration")]
        public Sprite SystemIcon;
        public Sprite UpdateIcon;
        public Sprite MaintenanceIcon;

        private AnnouncementModel _currentData;
        private Color _originalNormalColor;
        private Color _originalHighlightedColor;
        private bool _colorsCached = false;

        private void CacheColors()
        {
            if (!_colorsCached && RowButton != null)
            {
                _originalNormalColor = RowButton.colors.normalColor;
                _originalHighlightedColor = RowButton.colors.highlightedColor;
                _colorsCached = true;
            }
        }

        public void Setup(AnnouncementModel data)
        {
            _currentData = data;
            CacheColors();

            if (TitleText != null) TitleText.text = data.Title;
            if (DetailsText != null) DetailsText.text = data.Details;
            if (DateText != null) DateText.text = data.ParsedDate.ToString("MMM dd, yyyy");

            if (IconImage != null)
            {
                switch (data.Type)
                {
                    case AnnouncementType.System:
                        IconImage.sprite = SystemIcon;
                        break;
                    case AnnouncementType.Update:
                        IconImage.sprite = UpdateIcon;
                        break;
                    case AnnouncementType.Maintenance:
                        IconImage.sprite = MaintenanceIcon;
                        break;
                }
            }

            // Example status indicator (hide if archived or read, show if unread)
            if (StatusIndicator != null)
            {
                StatusIndicator.gameObject.SetActive(data.State == AnnouncementState.Unread);
            }

            if (RowButton != null)
            {
                ColorBlock cb = RowButton.colors;
                cb.normalColor = data.State == AnnouncementState.Unread ? _originalHighlightedColor : _originalNormalColor;
                RowButton.colors = cb;
            }
        }

        public AnnouncementModel GetCurrentData() => _currentData;

        public void SetSelected(bool selected)
        {
            CacheColors();
            if (RowButton != null)
            {
                ColorBlock cb = RowButton.colors;
                cb.normalColor = selected ? cb.selectedColor : (_currentData.State == AnnouncementState.Unread ? _originalHighlightedColor : _originalNormalColor);
                RowButton.colors = cb;
            }
        }

        // Call this from a UI button on the item itself when clicked
        public void OnClickRead()
        {
            if (_currentData == null) return;
            
            // Tell the manager to set this as selected first
            AnnouncementManager manager = FindFirstObjectByType<AnnouncementManager>();
            if (manager != null)
            {
                manager.SetSelectedAnnouncement(_currentData.Id);
            }

            if (_currentData.State == AnnouncementState.Unread)
            {
                _currentData.State = AnnouncementState.Read;
                
                // Refresh local UI
                if (StatusIndicator != null)
                {
                    StatusIndicator.gameObject.SetActive(false);
                }

                if (RowButton != null)
                {
                    ColorBlock cb = RowButton.colors;
                    cb.normalColor = _originalNormalColor;
                    RowButton.colors = cb;
                }
                
                // Tell the manager to update counts
                if (manager != null)
                {
                    manager.RefreshCounts();
                }
            }

            // Show details on the right panel
            AnnouncementDetailsManager detailsManager = FindFirstObjectByType<AnnouncementDetailsManager>();
            if (detailsManager != null)
            {
                detailsManager.ShowDetails(_currentData);
            }
        }
    }
}
