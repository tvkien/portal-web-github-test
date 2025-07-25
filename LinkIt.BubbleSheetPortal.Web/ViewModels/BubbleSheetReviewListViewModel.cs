using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetReviewListViewModel
    {
        public string Ticket { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Bank { get; set; }
        public string Test { get; set; }
        public string Class { get; set; }
        public string Teacher { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string GradedCount { get; set; }
        public string Fini { get; set; }
        public string Review { get; set; }
        public string Ungraded { get; set; }
        public int ClassId { get; set; }
        public bool IsArchived { get; set; }
        public bool IsDownloadable { get; set; }
        public int UnmappedSheetCount { get; set; }
        public int VirtualTestSubTypeID { get; set; }
        public string GroupTeacher { get; set; }
        public string ClassIds { get; set; }
        public int BankID { get; set; }
    }
}