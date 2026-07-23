using System;
using System.Collections.Generic;

namespace Luminang.UI.Announcements
{
    public enum AnnouncementType
    {
        System,
        Update,
        Maintenance
    }

    public enum AnnouncementState
    {
        Unread,
        Read,
        Archived
    }

    [Serializable]
    public class AnnouncementModel
    {
        public string Id;
        public AnnouncementType Type;
        public string Title;
        public string Details;
        public string DateString; // ISO 8601 string from JSON/Supabase
        public AnnouncementState State;

        public DateTime ParsedDate
        {
            get
            {
                if (DateTime.TryParse(DateString, out DateTime result))
                    return result;
                return DateTime.MinValue;
            }
        }
    }

    [Serializable]
    public class AnnouncementDataList
    {
        public List<AnnouncementModel> announcements;
    }
}
