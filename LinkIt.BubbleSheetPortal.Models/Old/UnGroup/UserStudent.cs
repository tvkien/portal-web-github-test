using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UserStudent
    {
        private string className = string.Empty;
        private string teacherName = string.Empty;
        private string termName = string.Empty;
        private string schoolName = string.Empty;

        public int UserID { get; set; }
        public int SchoolID { get; set; }
        public int ClassID { get; set; }
        public int? TeacherID { get; set; }
        public int? StudentID { get; set; }
        public int TotalCount { get; set; }
        public string ModifiedBy { get; set; }

        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string TermName
        {
            get { return termName; }
            set { termName = value.ConvertNullToEmptyString(); }
        }

        public string TeacherName
        {
            get { return teacherName; }
            set { teacherName = value.ConvertNullToEmptyString(); }
        }

    }
}
