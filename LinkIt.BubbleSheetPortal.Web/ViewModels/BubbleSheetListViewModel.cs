using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetListViewModel
    {
        public string Ticket { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }        
        public string Bank { get; set; }
        public string Test { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreateBy { get; set; }
        public string Class { get; set; }
        public string School { get; set; }
        public string PrintingGroup { get; set; }
        public int PrintingGroupID { get; set; }
        public int GradedCount { get; set; }
        public int SheetsMissing { get; set; }         
    }
}