using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AssignSameTestViewModel
    {
        public DateTime AssignmentDate { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public DateTime? ResultDate { get; set; }
        public int StudentId { get; set; }
        public int QTITestClassAssignmentID { get; set; }
    }
}
