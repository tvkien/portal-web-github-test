using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestResult
    {
        private string trData = string.Empty;
        private string uin = string.Empty;

        public int TestResultId { get; set; }
        public int? QTIOnlineTestSessionID { get; set; }
        public int VirtualTestId { get; set; }
        public int StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int SchoolId { get; set; }
        public DateTime ResultDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? TermId { get; set; }
        public int ClassId { get; set; }
        public int GradedById { get; set; }
        public int ScoreType { get; set; }
        public decimal ScoreValue { get; set; }
        public int SubmitType { get; set; }
        public int DistrictTermId { get; set; }
        public int UserId { get; set; }
        public int OriginalUserId { get; set; }
        public int LegacyBatchId { get; set; }
        public int BubbleSheetId { get; set; }
        public int? DistrictID { get; set; }

        public string TRData
        {
            get { return trData; }
            set { trData = value.ConvertNullToEmptyString(); }
        }

        public string UIN
        {
            get { return uin; }
            set { uin = value.ConvertNullToEmptyString(); }
        }
        public int LineIndex { get; set; }
    }
}
