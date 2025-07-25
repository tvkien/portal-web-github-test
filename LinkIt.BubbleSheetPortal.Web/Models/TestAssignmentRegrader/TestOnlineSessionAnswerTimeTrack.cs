using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class TestOnlineSessionAnswerTimeTrack
    {
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public string StartTimeUTC { get; set; }
        public string EndTimeUTC { get; set; }
    }
}