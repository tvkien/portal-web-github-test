using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.Requests
{
    public class RequestEmailNotification
    {
        private string emailContent = string.Empty;

        public int Id { get; set; }
        public int RequestId { get; set; }

        public string EmailContent
        {
            get { return emailContent; }
            set { emailContent = value.ConvertNullToEmptyString(); }
        }
    }
}