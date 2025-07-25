using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentInGroupWithParents : ValidatableEntity<StudentInGroupWithParents>
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string middleName = string.Empty;

        public int ReportGroupStudentID { get; set; }
        public int StudentID { get; set; }
        public int ReportGroupID { get; set; }
        private string code = string.Empty;
        private string gender = string.Empty;

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value.ConvertNullToEmptyString(); }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }

        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }
        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }
        public string Gender
        {
            get { return gender; }
            set { gender = value.ConvertNullToEmptyString(); }
        }
        
        //Xml
        public string Parents { get; set; }
    }
}