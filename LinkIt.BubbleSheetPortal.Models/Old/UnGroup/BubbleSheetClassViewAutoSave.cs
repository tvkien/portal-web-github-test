using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetClassViewAutoSave
    {
        public int BubbleSheetClassViewAutoSaveId { get; set; }
        public string Ticket { get; set; }
        public int ClassId { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ActualData { get; set; } //data from db
        public string Data { get; set; }
    }
}