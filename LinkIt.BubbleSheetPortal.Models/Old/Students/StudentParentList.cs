using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentParentList
    {
        public int StudentID { get; set; }

        private string firstName = string.Empty;
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        private string lastName = string.Empty;
        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }

        private string gender = string.Empty;
        public string Gender
        {
            get { return gender; }
            set { gender = value.ConvertNullToEmptyString(); }
        }

        private string school = string.Empty;
        public string School
        {
            get { return school; }
            set { school = value.ConvertNullToEmptyString(); }
        }

        private string gradeName = string.Empty;
        public string GradeName
        {
            get { return gradeName; }
            set { gradeName = value.ConvertNullToEmptyString(); }
        }

        public int GradeOrder { get; set; }

        public int ParentCount { get; set; }
        public int ClassCount { get; set; }

        private string firstClassName = string.Empty;
        public string FirstClassName
        {
            get { return firstClassName; }
            set { firstClassName = value.ConvertNullToEmptyString(); }
        }

        private string firstParentName = string.Empty;
        public string FirstParentName
        {
            get { return firstParentName; }
            set { firstParentName = value.ConvertNullToEmptyString(); }
        }
    }
}
