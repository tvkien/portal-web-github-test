using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class DisplayTestResultDistrict
    {
        private string testName = string.Empty;
        private string schoolName = string.Empty;
        private string teacherNameCustom = string.Empty;
        private string classNameCustom = string.Empty;
        private string studentCustom = string.Empty;

        public int TestResultId { get; set; }
        public int VirtualTestId { get; set; }
        public int AuthorUserId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public int DistrictId { get; set; }
        public DateTime? ResultDate { get; set; }

        public string TestName
        {
            get { return testName; }
            set { testName = value.ConvertNullToEmptyString(); }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string ClassNameCustom
        {
            get { return classNameCustom; }
            set { classNameCustom = value.ConvertNullToEmptyString(); }
        }

        public string StudentCustom
        {
            get { return studentCustom; }
            set { studentCustom = value.ConvertNullToEmptyString(); }
        }

        public string TeacherCustom
        {
            get { return teacherNameCustom; }
            set { teacherNameCustom = value.ConvertNullToEmptyString(); }  
        }
    }
}