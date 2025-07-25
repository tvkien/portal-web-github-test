using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SecuritySettingsMap
    {
        public SecuritySettingPreferenceModel SecuritySettingModel { get; set; }
        public int DistrictId { get; set; }
        public string SettingLevel { get; set; }
        public int SettingLevelID { get; set; }
    }
}
