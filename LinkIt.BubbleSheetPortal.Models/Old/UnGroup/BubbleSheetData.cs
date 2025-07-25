using System.Collections.Generic;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetData : ValidatableEntity<BubbleSheetData>
    {
        private string className = string.Empty;
        private string teacherName = string.Empty;
        private string schoolName = string.Empty;
        private string subjectName = string.Empty;
        private string testName = string.Empty;
        private string districtName = string.Empty;

        private List<string> studentIdList = new List<string>();

        public int SchoolId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int BankId { get; set; }
        public int TestId { get; set; }
        public int ClassId { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int BubbleSizeId { get; set; }
        public int SheetStyleId { get; set; }
        public int DistrictTermId { get; set; }
        public int UserId { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public int BubbleFormat { get; set; }
        public bool IsGenericBubbleSheet { get; set; }
        public bool IsIncludeEssayPage { get; set; }
        public bool IsIncludeShading { get; set; }
        public int NumberOfGenericSheet { get; set; }
        public bool PrintStudentIDs { get; set; }
        public bool IsIncludeExtraPages { get; set; }
        public int NumberOfGraphExtraPages { get; set; }
        public int NumberOfPlainExtraPages { get; set; }
        public int NumberOfLinedExtraPages { get; set; }
        public int NumberOfPrimaryExtraPages { get; set; }

        public bool IsManualEntry
        {
            get { return SheetStyleId == (int)SheetStyle.ManualEntry; }
        }

        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }
        
        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }

        public string TeacherName
        {
            get { return teacherName; } 
            set { teacherName = value.ConvertNullToEmptyString(); }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value.ConvertNullToEmptyString(); }
        }

        public string TestName
        {
            get { return testName; }
            set { testName = value.ConvertNullToEmptyString(); }
        }

        public string DistrictName
        {
            get { return districtName; }
            set { districtName = value.ConvertNullToEmptyString(); }
        }

        public int TestExtract { get; set; }

        // 0: None; 1: Automatic; 2: By Last Item (question); 3: By Last Section
        public int PaginationOption { get; set; }
        public string PaginationQuestionIds { get; set; }
        public string PaginationSectionIds { get; set; }

        public int TimezoneOffset { get; set; }

        public string BubbleSheetPreference { get; set; }

        public bool IsGridStype { get; set; }

        public bool IsPrintExtraPageOnly { get; set; }
    }
}
