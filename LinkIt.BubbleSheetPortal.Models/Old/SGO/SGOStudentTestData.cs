using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOStudentTestData
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public int GradeOrder { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public int? BankAuthorId { get; set; }
        public int VirtualTestId { get; set; }
        public string VirtualTestName { get; set; }
        public int TestResultId { get; set; }
        public int StudentId { get; set; }
        public int? AchievementLevelSettingId { get; set; }
        public string AchievementLevelSettingName { get; set; }
        public int? DataSetCategoryID { get; set; }
        public string DataSetCategoryName { get; set; }
        public int? VirtualTestSourceId { get; set; }

        public int? VirtualTestType { get; set; }
        public DateTime? ResultDate { get; set; }
        public int DataSetOriginID { get; set; }
    }
}
