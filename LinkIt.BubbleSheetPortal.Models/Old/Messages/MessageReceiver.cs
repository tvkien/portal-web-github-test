using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class MessageReceiver : ValidatableEntity<MessageReceiver>
    {
        public int MessageReceiverId { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int StudentId { get; set; }
        public int IsRead { get; set; }
        public int IsDeleted { get; set; }
    }
}