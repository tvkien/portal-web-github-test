using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class MessageInboxViewModel
    {
        public int MessageId { get; set; }
        public string Sender { get; set; }
        public string Recipients { get; set; }
        public string BriefInfo { get; set; }        
        public string Acknow { get; set; }
        public string Replies { get; set; }
        public string CreatedDateTimeString { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int MessageNoUnread { get; set; }
        public int StudentId { get; set; }
        public string CreatedDateTimeFullString { get; set; }
        public int MessageNoInThread { get; set; }
        public int SenderId { get; set; }
    }
}