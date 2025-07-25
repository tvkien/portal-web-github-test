using System;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class DelegateTeacher : ValidatableEntity<DelegateTeacher>
    {
        public int DelegateTeacherID { get; set; }
        public int OriginalTeacherID { get; set; }
        public int NewTeacherID { get; set; }
        public int ClassID { get; set; }
        public int UserID { get; set; }
        public bool IsActive { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}