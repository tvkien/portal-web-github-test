using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.Isolating
{
    public class IsolatingAnswerTimeTrackDTO
    {
        public int QTIOnlineTestSessionID { get; set; }
        public int VirtualQuestionID { get; set; }
        public DateTime? StartTimeUTC { get; set; }
        public DateTime? EndTimeUTC { get; set; }
    }
}