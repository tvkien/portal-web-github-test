using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class MessageInDetail
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int StudentId { get; set; }
        public int MessageType { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int IsRead { get; set; }
        public int IsAcknowledgeRequired { get; set; }
        public int ReplyEnabled { get; set; }
        public string Recipients { get; set; }
    }
}