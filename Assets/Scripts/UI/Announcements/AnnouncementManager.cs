using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Luminang.UI.Announcements
{
    public class AnnouncementManager : MonoBehaviour
    {
        [Header("UI References")]
        public Transform ContentContainer; // The parent inside the ScrollView
        public GameObject AnnouncementItemPrefab; // The prefab with AnnouncementItemUI script
        public AnnouncementTabGroup TabGroup; // Reference to update tab counts
        public AnnouncementDetailsManager DetailsManager; // Reference to details panel

        private List<AnnouncementModel> _mockDatabase = new List<AnnouncementModel>();
        private List<GameObject> _instantiatedItems = new List<GameObject>();
        
        private string _currentSelectedTab = "Inbox";
        private bool _dataLoaded = false;
        private string _selectedAnnouncementId;

        public void SetSelectedAnnouncement(string id)
        {
            _selectedAnnouncementId = id;
            foreach (var item in _instantiatedItems)
            {
                var ui = item.GetComponent<AnnouncementItemUI>();
                if (ui != null && ui.GetCurrentData() != null)
                {
                    ui.SetSelected(ui.GetCurrentData().Id == _selectedAnnouncementId);
                }
            }
        }

        private void EnsureDataLoaded()
        {
            if (!_dataLoaded)
            {
                LoadMockData();
                _dataLoaded = true;
            }
        }

        private void LoadMockData()
        {
            string path = Application.dataPath + "/Demo Data/AnnouncementDemoData.json";
            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                AnnouncementDataList dataList = JsonUtility.FromJson<AnnouncementDataList>(json);
                if (dataList != null && dataList.announcements != null)
                {
                    _mockDatabase = dataList.announcements;
                }
            }
            else
            {
                Debug.LogError("Could not find AnnouncementDemoData.json at " + path);
                _mockDatabase = new List<AnnouncementModel>();
            }
        }

        public void ArchiveAnnouncement(string id)
        {
            EnsureDataLoaded();
            var announcement = _mockDatabase.FirstOrDefault(a => a.Id == id);
            if (announcement != null)
            {
                announcement.State = AnnouncementState.Archived;
                RefreshCounts();
                PopulateList();
            }
        }

        public void DeleteAnnouncement(string id)
        {
            EnsureDataLoaded();
            var announcement = _mockDatabase.FirstOrDefault(a => a.Id == id);
            if (announcement != null)
            {
                _mockDatabase.Remove(announcement);
                RefreshCounts();
                PopulateList();
            }
        }



        public void OnTabChanged(string tabName)
        {
            EnsureDataLoaded();
            _currentSelectedTab = tabName;
            PopulateList();
        }

        public void RefreshCounts()
        {
            EnsureDataLoaded();
            if (TabGroup == null || TabGroup.TabButtons == null) return;

            int inboxCount = _mockDatabase.Count(a => a.State != AnnouncementState.Archived);
            int unreadCount = _mockDatabase.Count(a => a.State == AnnouncementState.Unread);
            int archivedCount = _mockDatabase.Count(a => a.State == AnnouncementState.Archived);

            foreach (var tab in TabGroup.TabButtons)
            {
                if (tab.TabName.ToLower() == "inbox")
                    tab.UpdateCount(inboxCount);
                else if (tab.TabName.ToLower() == "unread")
                    tab.UpdateCount(unreadCount);
                else if (tab.TabName.ToLower() == "archived")
                    tab.UpdateCount(archivedCount);
            }
        }

        private void PopulateList()
        {
            EnsureDataLoaded();
            RefreshCounts();

            // Clear existing UI items
            foreach (var item in _instantiatedItems)
            {
                Destroy(item);
            }
            _instantiatedItems.Clear();

            // Filter data based on selected tab
            List<AnnouncementModel> filteredList = new List<AnnouncementModel>();

            switch (_currentSelectedTab.ToLower())
            {
                case "inbox":
                    // Show everything not archived
                    filteredList = _mockDatabase.Where(a => a.State != AnnouncementState.Archived).ToList();
                    break;
                case "unread":
                    filteredList = _mockDatabase.Where(a => a.State == AnnouncementState.Unread).ToList();
                    break;
                case "archived":
                    filteredList = _mockDatabase.Where(a => a.State == AnnouncementState.Archived).ToList();
                    break;
                default:
                    filteredList = _mockDatabase;
                    break;
            }

            // Sort by Date (newest first)
            filteredList = filteredList.OrderByDescending(a => a.ParsedDate).ToList();

            // Resolve selection ID: default to first if current ID is empty or not in the filtered list
            if (filteredList.Count > 0)
            {
                if (string.IsNullOrEmpty(_selectedAnnouncementId) || !filteredList.Any(a => a.Id == _selectedAnnouncementId))
                {
                    _selectedAnnouncementId = filteredList[0].Id;
                }
            }
            else
            {
                _selectedAnnouncementId = null;
            }

            // Instantiate items
            foreach (var data in filteredList)
            {
                GameObject newObj = Instantiate(AnnouncementItemPrefab, ContentContainer);
                AnnouncementItemUI uiComponent = newObj.GetComponent<AnnouncementItemUI>();
                if (uiComponent != null)
                {
                    uiComponent.Setup(data);
                    uiComponent.SetSelected(data.Id == _selectedAnnouncementId);
                }
                _instantiatedItems.Add(newObj);
            }

            // Auto-select the active item to populate the right panel
            if (DetailsManager == null)
            {
                DetailsManager = FindFirstObjectByType<AnnouncementDetailsManager>();
                if (DetailsManager == null)
                {
                    var allManagers = Resources.FindObjectsOfTypeAll<AnnouncementDetailsManager>();
                    if (allManagers != null && allManagers.Length > 0)
                    {
                        foreach (var mgr in allManagers)
                        {
                            if (mgr.gameObject.scene.name != null)
                            {
                                DetailsManager = mgr;
                                break;
                            }
                        }
                    }
                }
            }

            Debug.Log($"[AnnouncementManager] PopulateList resolved DetailsManager: {DetailsManager != null}. SelectedId: {_selectedAnnouncementId}");

            if (DetailsManager != null)
            {
                if (!string.IsNullOrEmpty(_selectedAnnouncementId))
                {
                    var selectedData = filteredList.FirstOrDefault(a => a.Id == _selectedAnnouncementId);
                    Debug.Log($"[AnnouncementManager] SelectedData found: {selectedData != null} (Title: {selectedData?.Title})");
                    if (selectedData != null)
                    {
                        DetailsManager.ShowDetails(selectedData);
                    }
                }
                else
                {
                    DetailsManager.ClearDetails();
                }
            }
        }
    }
}
