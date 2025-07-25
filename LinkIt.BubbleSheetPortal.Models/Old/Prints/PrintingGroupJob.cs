using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PrintingGroupJob
    {
        public int PrintingGroupJobID { get; set; } 
        public int GroupID { get; set; }
        public int CreatedUserID { get; set; }
        public DateTime DateCreated { get; set; }
    }
}