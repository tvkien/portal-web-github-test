using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentParent : ValidatableEntity<StudentParent>
    {
        public int StudentParentID { get; set; }
        public int StudentID { get; set; }
        public int ParentID { get; set; }
        public string Relationship { get; set; }
        public bool StudentDataAccess { get; set; }
        
    }
}
