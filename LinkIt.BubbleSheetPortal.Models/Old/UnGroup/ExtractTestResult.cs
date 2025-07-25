using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExtractTestResult
    {
        private string _testNameCustom = string.Empty;
        private string _schoolName = string.Empty;
        private string _teacherCustom = string.Empty;
        private string _classNameCustom = string.Empty;
        private string _studentCustom = string.Empty;
        private string _testName = string.Empty;
        private string _gradeName = string.Empty;
        private string _subjectName = string.Empty;
        private string _bankName = string.Empty;
        private string _className = string.Empty;

        private string _studentCodeCustom = string.Empty;

        public int TestResultId { get; set; }

        public string TestNameCustom
        {
            get { return _testNameCustom; }
            set { _testNameCustom = value.ConvertNullToEmptyString(); }
        }
        public string ClassNameCustom
        {
            get { return _classNameCustom; }
            set { _classNameCustom = value.ConvertNullToEmptyString(); }
        }
        public string StudentCustom
        {
            get { return _studentCustom; }
            set { _studentCustom = value.ConvertNullToEmptyString(); }
        }

        public string StudentCodeCustom
        {
            get { return _studentCodeCustom; }
            set { _studentCodeCustom = value.ConvertNullToEmptyString(); }
        }
        public int StudentId { get; set; }
        public string TeacherCustom
        {
            get { return _teacherCustom; }
            set { _teacherCustom = value.ConvertNullToEmptyString(); }
        }
        public string SchoolName
        {
            get { return _schoolName; }
            set { _schoolName = value.ConvertNullToEmptyString(); }
        }
        public int DistrictId { get; set; }
        public string TestName
        {
            get { return _testName; }
            set { _testName = value.ConvertNullToEmptyString(); }
        }
        public DateTime ResultDate { get; set; }
        public string GradeName
        {
            get { return _gradeName; }
            set { _gradeName = value.ConvertNullToEmptyString(); }
        }
        public string SubjectName
        {
            get { return _subjectName; }
            set { _subjectName = value.ConvertNullToEmptyString(); }
        }
        public string BankName
        {
            get { return _bankName; }
            set { _bankName = value.ConvertNullToEmptyString(); }
        }
        public string ClassName
        {
            get { return _className; }
            set { _className = value.ConvertNullToEmptyString(); }
        }
        public int? StudentDistrictId { get; set; }

        public int SchoolId { get; set; }

        public int UserId { get; set; }
        public int? ClassId { get; set; }
        public int TotalRows { get; set; }
    }
}

