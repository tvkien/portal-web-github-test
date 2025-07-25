using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestFeedback
    {
       public int TestFeedbackID { get; set; }
       public int QtiOnlineTestSessionID { get; set; }
       public int? TestResultID { get; set; }
       public string Feedback { get; set; }
       public int UserID { get; set; }
       public DateTime UpdatedDate { get; set; }

    }
}

