using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SchoolTeacherList
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string userName = string.Empty;
        private string classID = string.Empty;
        private string className = string.Empty;

        public int UserID { set; get; }
        public int SchoolID { get; set; }
        public bool? Active { get; set; }
        public int RoleId { get; set; }

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

        public string UserName
        {
            get { return userName; } 
            set { userName = value.ConvertNullToEmptyString(); }
        }

        public string ClassID
        {
            get { return classID; }
            set { classID = value.ConvertNullToEmptyString(); }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }
    }
}