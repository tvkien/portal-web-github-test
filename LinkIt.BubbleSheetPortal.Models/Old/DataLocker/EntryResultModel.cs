using System;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    [Serializable]
    public class EntryResultModel
    {
        public int? VirtualTestId { get; set; }
        public int? ClassId { get; set; }
        public string StudentsIdSelectedString { get; set; }        
        public DateFormatModel DateFormatModel { get; set; }

        public string VirtualTestName { get; set; }
        public string ClassName { get; set; }

        public string VirtualtestFileKey { get; set; }
        public string RubricDescription { get; set; }

        public int? DistrictId { get; set; }
        public int? SchoolId { get; set; }
        public int? TeacherId { get; set; }
        public int? GradeId { get; set; }
        public int? SubjectId { get; set; }
        public int? BankId { get; set; }
        public string ResultDate { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string AllowChangeResultDate { get; set; }
        public int? CurrentUserDistrictId { get; set; }
        public bool HasTestResult { get; set; }
        public bool? IsOldUI { get; set; }
    }
}
