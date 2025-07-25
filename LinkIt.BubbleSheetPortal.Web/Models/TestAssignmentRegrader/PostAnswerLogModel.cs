using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class PostAnswerLogModel
    {
        public string DumpCol { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string Answer { get; set; }
        public string AnswerImage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimestampString
        {
            get
            {
                return Timestamp.ToString("MM/dd/yyyy hh:mm tt");
            }
        }
    }
   
    public class RecoverPostAnswerLogModel
    {
        public int QtiOnlineTestSessionID { get; set; }

        [AllowHtml]
        public string AnswerText { get; set; }

        public int AnswerId { get; set; }

        public int? AnswerSubId { get; set; }
    }
}

