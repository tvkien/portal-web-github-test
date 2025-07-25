using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ViewMessageViewModel
    {
        public int ReplyMessageType { get; set; } // 2: Reply message; 3: Acknowledge message
        
        public List<MessageInViewMessageViewModel> Messages { get; set; }
        public string Subject { get; set; }
        public int IsAcknowledgeRequired { get; set; }
        public int IsRepliedRequired { get; set; }
        public int MessageType { get; set; }
        public string Body { get; set; }

        public int MessageRef { get; set; }
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int StudentId { get; set; }

        public bool IsViewSubThreadMessage { get; set; }
        public bool IsDisable { get; set; }

        public string SendActionType { get; set; }
        public bool SendActionResult { get; set; }

        public bool HasUnreadMessageInThread { get; set; }

        // Detect this main thread has multi recipients or not to display 'Reply' or' Reply to All' button
        public bool HasMultiRecipients
        {
            get
            {
                if(Messages == null || Messages.Count == 0)
                    return false;
                else
                {
                    if(Messages[0].Recipients != null && Messages[0].Recipients.Contains(","))
                        return true;
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}