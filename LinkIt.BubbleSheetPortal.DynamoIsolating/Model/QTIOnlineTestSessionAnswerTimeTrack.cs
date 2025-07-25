using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Model
{
    public class QTIOnlineTestSessionAnswerTimeTrack
    {
        public string QTIOnlineTestSessionID_VirtualQuestionID { get; set; }
        public DateTime? StartTimeUTC { get; set; }
        public DateTime? EndTimeUTC { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
    }
}
