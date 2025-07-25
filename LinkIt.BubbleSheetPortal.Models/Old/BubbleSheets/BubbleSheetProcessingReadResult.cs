using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetProcessingReadResult
    {
        public int ReadResultId { get; set; }
        public string OutputFile { get; set; }
        public string ListQuestion { get; set; }
        public int UserId { get; set; }
        public int DistrictId { get; set; }
        public string Barcode1 { get; set; }
        public string Barcode2 { get; set; }
        public string InputPath { get; set; }
        public string InputFileName { get; set; }
        public decimal Dpi { get; set; }
        public int PageNumber { get; set; }
        public string FileDisposition { get; set; }
        public string UrlSafeOutputFile { get; set; }
        public int ProcessingTime { get; set; }
        public DateTime CompletedTime { get; set; }
        public string RosterPosition { get; set; }
        public int QuestionCount { get; set; }
        public bool IsRoster { get; set; }
        public Guid SheetPageId { get; set; }
        public string RawResult { get; set; }
        public int? TestType { get; set; }
        public int? ACTPageIndex { get; set; }
        public int? SecondValidationStatus { get; set; }
    }
}
