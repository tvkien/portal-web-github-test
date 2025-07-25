using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class MessageInbox
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string Sender { get; set; }
        public string Recipients { get; set; }
        public string Subject { get; set; }
        public string Boby { get; set; }
        public int StudentId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int MessageNoUnread { get; set; }
        public int Acknow { get; set; }
        public int Replies { get; set; }
        public int MessageNoInThread { get; set; }
        public int MessageType { get; set; }
    }
}