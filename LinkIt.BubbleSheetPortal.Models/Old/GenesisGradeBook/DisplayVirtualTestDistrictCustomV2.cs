using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.GenesisGradeBook
{
    public class DisplayVirtualTestDistrictCustomV2
    {

        public int VirtualTestID { get; set; }

        private string virtualTestName = string.Empty;
        public string VirtualTestName
        {
            get { return virtualTestName; }
            set { virtualTestName = value.ConvertNullToEmptyString(); }
        }

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

        public int ResultCount { get; set; }

        private string testResultIDList = string.Empty;
        public string TestResultIDList
        {
            get { return testResultIDList; }
            set { testResultIDList = value.ConvertNullToEmptyString(); }
        }

        private string studentNameList = string.Empty;
        public string StudentNameList
        {
            get { return studentNameList; }
            set { studentNameList = value.ConvertNullToEmptyString(); }
        }

        private string studentIDList = string.Empty;
        public string StudentIDList
        {
            get { return studentIDList; }
            set { studentIDList = value.ConvertNullToEmptyString(); }
        }
    }
}
