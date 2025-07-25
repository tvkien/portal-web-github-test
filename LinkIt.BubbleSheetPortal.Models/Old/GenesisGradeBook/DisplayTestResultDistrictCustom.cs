using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.GenesisGradeBook
{
    public class DisplayTestResultDistrictCustom
    {

        public int TestResultId { get; set; }

        private string testName = string.Empty;
        public string TestName
        {
            get { return testName; }
            set { testName = value.ConvertNullToEmptyString(); }
        }

        private string schoolName = string.Empty;
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        private string teacherNameCustom = string.Empty;
        public string TeacherCustom
        {
            get { return teacherNameCustom; }
            set { teacherNameCustom = value.ConvertNullToEmptyString(); }
        }

        private string classNameCustom = string.Empty;
        public string ClassNameCustom
        {
            get { return classNameCustom; }
            set { classNameCustom = value.ConvertNullToEmptyString(); }
        }

        private string studentCustom = string.Empty;
        public string StudentCustom
        {
            get { return studentCustom; }
            set { studentCustom = value.ConvertNullToEmptyString(); }
        }

        public bool? IsExported { get; set; }

        public DateTime? ResultDate { get; set; }
    }
}
