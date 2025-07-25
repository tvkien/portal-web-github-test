using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestResultTransferModel
    {
        public string TestResultIDs { get; set; }
        public string StudentIDs { get; set; }
        public int UserId { get; set; }
        public int OldClassId { get; set; }
        public int DistrictId { get; set; }
        public int TotalResultSelected { get; set; }
    }
}
