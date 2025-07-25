using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ParentDetail
    {
        public int UserID { get; set; }

        private string firstName = string.Empty;
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        private string phone = string.Empty;
        public string Phone
        {
            get { return phone; }
            set { phone = value.ConvertNullToEmptyString(); }
        }

        private string lastName = string.Empty;
        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }

        private string messageNumber = string.Empty;
        public string MessageNumber
        {
            get { return messageNumber; }
            set { messageNumber = value.ConvertNullToEmptyString(); }
        }

        private string emailAddress = string.Empty;
        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value.ConvertNullToEmptyString(); }
        }
    }
}
