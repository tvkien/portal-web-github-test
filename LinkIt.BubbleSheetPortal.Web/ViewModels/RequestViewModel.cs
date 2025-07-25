using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RequestViewModel
    {
        public int Status { get; set; }
        public string DistrictName { get; set; }
        public string FileName { get; set; }
        public string RosterType { get; set; }
        public DateTime DateUploaded { get; set; }
        public bool IsDeleted { get; set; }
        public bool CanSubmit { get; set; }
        public bool IsSubmitted { get; set; }
        public bool HasBeenMoved { get; set; }
        public bool HasEmailContent { get; set; }
        public int Id { get; set; }
        public int Mode { get; set; }
        public int DataRequestTypeId { get; set; }
    }
}
