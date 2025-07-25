using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class SendGridViewModel
    {
        public string From { get; set; }
        public string Alias { get; set; }
        public string To { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }

        public string HtmlFooter { get; set; }
        public string PlainTextFooter { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}