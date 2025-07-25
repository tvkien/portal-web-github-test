using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassWithDetail
    {
        public int ClassID { get; set; }

        private string className = string.Empty;
        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }

        private string course = string.Empty;
        public string Course
        {
            get { return course; }
            set { course = value.ConvertNullToEmptyString(); }
        }

        private string section = string.Empty;
        public string Section
        {
            get { return section; }
            set { section = value.ConvertNullToEmptyString(); }
        }

        private string termName = string.Empty;
        public string TermName
        {
            get { return termName; }
            set { termName = value.ConvertNullToEmptyString(); }
        }

        private string teacherFirstName = string.Empty;
        public string TeacherFirstName
        {
            get { return teacherFirstName; }
            set { teacherFirstName = value.ConvertNullToEmptyString(); }
        }

        private string teacherLastName = string.Empty;
        public string TeacherLastName
        {
            get { return teacherLastName; }
            set { teacherLastName = value.ConvertNullToEmptyString(); }
        }
    }
}
