using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassSchoolStudent
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;

        public int ClassID { get; set; }
        public int SchoolID { get; set; }
        public int StudentID { get; set; }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }
    }
}