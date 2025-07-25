using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExtractLocalTestResultsQueue : ValidatableEntity<ExtractLocalTestResultsQueue>
    {
        public int ExtractLocalTestResultsQueueId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int UserTimeZoneOffset { get; set; }
        public string ExportTemplates { get; set; }
        public string ListIDsInput { get; set; }
        public int ExtractType { get; set; }
        public string ListSchoolIds { get; set; }
        public string ListClassIds { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public DateTime? ProcessingDate { get; set; }
        public int? ProcessingTime { get; set; }
        public DateTime? EndProcessingDate { get; set; }
        public string BaseHostURL { get; set; }
        public string UrlDownload { get; set; }
        public int? ExtractTestResultParamID { get; set; }
    }
}