using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesTestResultData
    {
        public int ReportType { get; set; }
        public string SchoolId { get; set; }
        public string StudentClass { get; set; } // className
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public int ASDStatus { get; set; }
        public string TestingPeriod { get; set; }
        public int TestId { get; set; }
        public List<AblesResponsesData> Responses { get; set; }
        public int Score { get; set; }
        public int Completed { get; set; }
        public string Stamp { get; set; }
        public int TestIndex { get; set; }
    }
}
