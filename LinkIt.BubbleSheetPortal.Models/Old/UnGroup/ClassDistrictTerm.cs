using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassDistrictTerm
    {
        private string className = string.Empty;
        private string schoolName = string.Empty;
        private string termName = string.Empty;
        private string teacherFirstName = string.Empty;
        private string teacherLastName = string.Empty;
        private string teacherUserName = string.Empty;

        public int ClassId { get; set; }
        public int? SchoolId { get; set; }
        public int? DistrictTermId { get; set; }
        public int? DistrictId { get; set; }
        public int? UserId { get; set; }

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

        public string TeacherFirstName
        {
            get { return teacherFirstName; } 
            set { teacherFirstName = value.ConvertNullToEmptyString(); }
        }

        public string TeacherLastName
        {
            get { return teacherLastName; }
            set { teacherLastName = value.ConvertNullToEmptyString(); }
        }

        public string TeacherUserName
        {
            get { return teacherUserName; }
            set { teacherUserName = value.ConvertNullToEmptyString(); }
        }
    }
}