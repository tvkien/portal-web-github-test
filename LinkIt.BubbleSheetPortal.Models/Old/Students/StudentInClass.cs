using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentInClass : ValidatableEntity<StudentInClass>
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string middleName = string.Empty;
        private string gender = string.Empty;
        private string race = string.Empty;
        private string code = string.Empty;
        private string gradeName = string.Empty;

        public int ID { get; set; }
        public int StudentID { get; set; }
        public bool? Active { get; set; }
        public int ClassID { get; set; }

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

        public string Gender
        {
            get { return gender; }
            set { gender = value.ConvertNullToEmptyString(); }
        }
        
        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }

        public string Race
        {
            get { return race; }
            set { race = value.ConvertNullToEmptyString(); }
        }

        public string GradeName
        {
            get { return gradeName; }
            set { gradeName = value.ConvertNullToEmptyString(); }
        }

        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }
    }
}