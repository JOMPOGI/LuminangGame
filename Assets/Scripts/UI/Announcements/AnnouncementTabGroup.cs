using System.Collections.Generic;
using UnityEngine;

namespace Luminang.UI.Announcements
{
    public class AnnouncementTabGroup : MonoBehaviour
    {
        public List<AnnouncementTabButton> TabButtons = new List<AnnouncementTabButton>();
        public AnnouncementTabButton DefaultTab;
        
        private AnnouncementTabButton _selectedTab;
        private AnnouncementManager _manager;

        private void OnEnable()
        {
            if (_manager == null)
            {
                _manager = FindFirstObjectByType<AnnouncementManager>();
            }
            
            if (DefaultTab != null)
            {
                OnTabSelected(DefaultTab);
            }
            else if (TabButtons != null && TabButtons.Count > 0)
            {
                OnTabSelected(TabButtons[0]);
            }
        }

        public void Subscribe(AnnouncementTabButton button)
        {
            if (TabButtons == null)
            {
                TabButtons = new List<AnnouncementTabButton>();
            }

            if (!TabButtons.Contains(button))
            {
                TabButtons.Add(button);
            }
        }

        public void OnTabSelected(AnnouncementTabButton button)
        {
            _selectedTab = button;
            ResetTabs();
            button.Select();

            if (_manager != null)
            {
                _manager.OnTabChanged(button.TabName);
            }
        }

        private void ResetTabs()
        {
            foreach (AnnouncementTabButton button in TabButtons)
            {
                if (_selectedTab != null && button == _selectedTab) continue;
                button.Deselect();
            }
        }
    }
}
