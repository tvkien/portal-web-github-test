using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AutoFocusGroupConfig
    {
        public int AutoFocusGroupConfigID { get; set; }
        public int DistrictID { get; set; }
        public string AutoGroupType { get; set; }
        public string JSONConfig { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public DateTime? LastRunDate { get; set; }
        public bool Run { get; set; }
    }

    public class AutoFocusGroupJsonConfig
    {
        public string PrimaryTeacher { get; set; }
        public string DistrictTermID { get; set; }
        public string SchoolID { get; set; }
        public List<string> IncludedSchools { get; set; }
    }
}
