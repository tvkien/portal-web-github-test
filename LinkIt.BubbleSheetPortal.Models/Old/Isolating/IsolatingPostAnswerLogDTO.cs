using System;

namespace LinkIt.BubbleSheetPortal.Models.Isolating
{
    public class IsolatingPostAnswerLogDTO
    {
        public string DumpCol { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string Answer { get; set; }
        public string AnswerImage { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
