using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesReportData
    {
        public List<AblesTestResultData> TestResults { get; set; } 
        public string TimeStamp { get; set; }
        public string CheckSum { get; set; }
    }
}