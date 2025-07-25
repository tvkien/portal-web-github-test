using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class MessageInViewMessageViewModel
    {
        public int MessageId { get; set; }
        
        public string Sender { get; set; }
        public string CreatedDateTimeString { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public int IsAcknowledgeRequired { get; set; }
        public int ReplyEnabled { get; set; }
        public int IsRead { get; set; }
        public int StudentId { get; set; }
        public int SenderId { get; set; }

        public string Recipients { get; set; }
    }
}