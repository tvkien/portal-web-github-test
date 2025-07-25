using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    [Serializable]
    public class BubbleSheetGroupData : ValidatableEntity<BubbleSheetGroupData>
    {
        private List<string> studentIdList = new List<string>();
        private List<string> classNameList = new List<string>();
        private List<string> teacherNameList = new List<string>();
        private List<string> schoolNameList = new List<string>();
        private List<string> districtNameList = new List<string>();
        private List<string> studentFullNameList = new List<string>();
        private List<int> districtTermIdList = new List<int>();
        private List<int> schoolIdList = new List<int>();
        private List<int> classIdList = new List<int>();
        private List<int?> userIdList = new List<int?>();

        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int BankId { get; set; }
        public int TestId { get; set; }                
        public int SheetStyleId { get; set; }
        public int BubbleSizeId { get; set; }
        public int SelectSheetStyle { get; set; }        
        public int GroupId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public int BubbleFormat { get; set; }
        public string SubjectName { get; set; }
        public string TestName { get; set; }
        public int StateId { get; set; }
        public int PrintingGroupJobID { get; set; }
        public bool IsLargeClass { get; set; }
        public bool IsIncludeExtraPages { get; set; }
        public int NumberOfGraphExtraPages { get; set; }
        public int NumberOfPlainExtraPages { get; set; }
        public int NumberOfLinedExtraPages { get; set; }

        public bool IsManualEntry
        {
            get { return SheetStyleId == (int)SheetStyle.ManualEntry; }
        }

        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }

        public List<string> ClassNameList
        {
            get { return classNameList; }
            set { classNameList = value ?? new List<string>(); }
        }

        public List<string> TeacherNameList
        {
            get { return teacherNameList; }
            set { teacherNameList = value ?? new List<string>(); }
        }

        public List<string> SchoolNameList
        {
            get { return schoolNameList; }
            set { schoolNameList = value ?? new List<string>(); }
        }

        public List<string> StudentFullNameList
        {
            get { return studentFullNameList; }
            set { studentFullNameList = value ?? new List<string>(); }
        }

        public List<string> DistrictNameList
        {
            get { return districtNameList; }
            set { districtNameList = value ?? new List<string>(); }
        }

        public List<int> DistrictTermIdList
        {
            get { return districtTermIdList; }
            set { districtTermIdList = value ?? new List<int>(); }
        }

        public List<int> SchoolIdList
        {
            get { return schoolIdList; }
            set { schoolIdList = value ?? new List<int>(); }
        }

        public List<int> ClassIdList
        {
            get { return classIdList; }
            set { classIdList = value ?? new List<int>(); }
        }
        
        public List<int?> UserIdList
        {
            get { return userIdList; }
            set { userIdList = value ?? new List<int?>(); }
        }

        public bool IsIncludeEssayPage { get; set; }
        public bool IsIncludeShading { get; set; }

        public bool IsGenericBubbleSheet { get; set; }
        public int NumberOfGenericSheet { get; set; }
        public bool PrintStudentIDs { get; set; }
        // 0: None; 1: Automatic; 2: By Last Item (question); 3: By Last Section
        public int PaginationOption { get; set; }
        public string PaginationQuestionIds { get; set; }
        public string PaginationSectionIds { get; set; }

        public int TimezoneOffset { get; set; }
    }
}