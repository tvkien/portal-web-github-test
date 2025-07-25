using System;

namespace LinkIt.BubbleSheetPortal.Web.Models.HelpResource
{
    public class HelpResourceRow
    {
        public int ID { get; set; }
        public string FileType { get; set; }
        public int HelpResourceTypeID { get; set; }
        public string Category { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string UpdatedDateStr
        {
            get { return UpdatedDate.HasValue ? string.Format("{0:MM/dd/yy}", UpdatedDate.Value) : string.Empty; }
        }
        public DateTime? UpdatedDate { get; set; }
        public string HelpResourceFilePath { get; set; }
        public string HelpResourceLink { get; set; }
        public int? HelpResourceFileTypeID { get; set; }
        public string HelpResourceTypeDisplayText { get; set; }
        public int HelpResourceID { get; set; }
        public string HelpResourceTypeIcon { get; set; }
    }
}