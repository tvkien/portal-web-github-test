using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentOfParent : ValidatableEntity<StudentOfParent>
    {
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;

        public int StudentID { get; set; }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value.ConvertNullToEmptyString(); }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value.ConvertNullToEmptyString(); }
        }
        
    }
}