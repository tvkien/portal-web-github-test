using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.GenesisGradeBook
{
    public class DisplayTestResultDistrictCustomV2
    {

        public int TestResultID { get; set; }

        private string virtualTestName = string.Empty;
        public string VirtualTestName
        {
            get { return virtualTestName; }
            set { virtualTestName = value.ConvertNullToEmptyString(); }
        }

        private string studentName = string.Empty;
        public string StudentName
        {
            get { return studentName; }
            set { studentName = value.ConvertNullToEmptyString(); }
        }

        private string classTermName = string.Empty;
        public string ClassTermName
        {
            get { return classTermName; }
            set { classTermName = value.ConvertNullToEmptyString(); }
        }

        public DateTime? ResultDate { get; set; }

        private string categoryName = string.Empty;
        public string CategoryName
        {
            get { return categoryName; }
            set { categoryName = value.ConvertNullToEmptyString(); }
        }

        private string gradeName = string.Empty;
        public string GradeName
        {
            get { return gradeName; }
            set { gradeName = value.ConvertNullToEmptyString(); }
        }

        private string subjectName = string.Empty;
        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value.ConvertNullToEmptyString(); }
        }
        public int TotalRecords { get; set; }
        public int TotalStudents { get; set; }
        public int TotalVirtualTests { get; set; }
    }
}
