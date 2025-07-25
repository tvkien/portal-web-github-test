using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Message : ValidatableEntity<Message>
    {
        private string subject = string.Empty;
        private string body = string.Empty;

        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int IsAcknowlegdeRequired { get; set; }
        public int ReplyEnabled { get; set; }
        public int MessageType { get; set; }
        public int MessageRef { get; set; }
        public int IsDeleted { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string Body
        {
            get { return body; }
            set { body = value; }
        }
    }
}