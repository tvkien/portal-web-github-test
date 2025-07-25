using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLockerPreferencesSetting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class DataLockerPreferencesViewModel
    {
        public bool IsAdmin { get { return CurrentRoleId == (int)Permissions.DistrictAdmin || CurrentRoleId == (int)Permissions.NetworkAdmin; } }      
        public bool IsPublisher { get { return CurrentRoleId == (int)Permissions.Publisher; } }
        public int CurrentDistrictId { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public List<int> ListDistricIds { get; set; }
        public int CurrentRoleId { get; set; }

        public string StrIds
        {
            get
            {
                var ids = string.Empty;
                if (this.ListDistricIds == null || !this.ListDistricIds.Any())
                {
                    return ids;
                }
                ids = this.ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
                return ids.TrimEnd(new[] { ',' });
            }
        }

        public bool IsSchoolAdmin { get { return CurrentRoleId == (int)Permissions.SchoolAdmin; } }

    }

    public class DataLockerPreferencesSettingMap
    {
        public int DistrictId { get; set; }
        public int SettingLevelID { get; set; }
        public string SettingLevel { get; set; }
        public DataLockerPreferenceLocalize Tooltips { get; set; }
        public DataLockerPreferencesSettingModal DataLockerPreferencesSettingModal { get; set; }
    }
}
