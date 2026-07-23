using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Luminang.UI.Announcements
{
    public class AnnouncementDetailsManager : MonoBehaviour
    {
        [Header("UI References")]
        public Image IconImage;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DateTimeText;
        public TextMeshProUGUI DetailsText;
        public GameObject ArchiveButton;
        public GameObject DeleteButton;

        [Header("Icons configuration")]
        public Sprite SystemIcon;
        public Sprite UpdateIcon;
        public Sprite MaintenanceIcon;

        private AnnouncementModel _currentData;
        private AnnouncementManager _mainManager;

        private void Awake()
        {
            if (_currentData == null)
            {
                ClearDetails();
            }
        }

        private void Start()
        {
            _mainManager = FindFirstObjectByType<AnnouncementManager>();
        }

        public void ShowDetails(AnnouncementModel data)
        {
            Debug.Log($"[AnnouncementDetailsManager] ShowDetails called for: {data?.Title}. ActiveSelf: {gameObject.activeSelf}");
            gameObject.SetActive(true);
            _currentData = data;

            if (TitleText != null) TitleText.text = data.Title;
            if (DetailsText != null) DetailsText.text = data.Details;
            
            // Format: July 2, 2026 • 8:30 AM
            if (DateTimeText != null) 
            {
                DateTimeText.text = data.ParsedDate.ToString("MMMM d, yyyy \u2022 h:mm tt");
            }

            if (IconImage != null)
            {
                IconImage.gameObject.SetActive(true);
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

            // Hide Archive button if it's already archived
            if (ArchiveButton != null)
            {
                ArchiveButton.SetActive(data.State != AnnouncementState.Archived);
            }

            if (DeleteButton != null)
            {
                DeleteButton.SetActive(true);
            }
        }

        public void ClearDetails()
        {
            Debug.Log("[AnnouncementDetailsManager] ClearDetails called. StackTrace: " + System.Environment.StackTrace);
            _currentData = null;
            if (TitleText != null) TitleText.text = "";
            if (DetailsText != null) DetailsText.text = "";
            if (DateTimeText != null) DateTimeText.text = "";
            if (IconImage != null) IconImage.gameObject.SetActive(false);
            if (ArchiveButton != null) ArchiveButton.SetActive(false);
            if (DeleteButton != null) DeleteButton.SetActive(false);
        }

        public void OnClickArchive()
        {
            if (_currentData == null || _mainManager == null) return;

            if (GenericModal.Instance != null)
            {
                GenericModal.Instance.ShowConfirm(
                    "Archive this announcement?",
                    "Yes",
                    () => {
                        _mainManager.ArchiveAnnouncement(_currentData.Id);
                        ClearDetails();
                    },
                    "No"
                );
            }
            else
            {
                Debug.LogWarning("[AnnouncementDetailsManager] GenericModal.Instance not found! Archiving immediately.");
                _mainManager.ArchiveAnnouncement(_currentData.Id);
                ClearDetails();
            }
        }

        public void OnClickDelete()
        {
            if (_currentData == null || _mainManager == null) return;

            if (GenericModal.Instance != null)
            {
                GenericModal.Instance.ShowConfirm(
                    "Delete this announcement? This cannot be undone.",
                    "Yes",
                    () => {
                        _mainManager.DeleteAnnouncement(_currentData.Id);
                        ClearDetails();
                    },
                    "No"
                );
            }
            else
            {
                Debug.LogWarning("[AnnouncementDetailsManager] GenericModal.Instance not found! Deleting immediately.");
                _mainManager.DeleteAnnouncement(_currentData.Id);
                ClearDetails();
            }
        }
    }
}
