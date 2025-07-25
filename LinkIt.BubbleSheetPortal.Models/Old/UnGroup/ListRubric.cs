using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ListRubric
    {
        private string _subjectName = string.Empty;
        public string SubjectName
        {
            get { return _subjectName; }
            set { _subjectName = value.ConvertNullToEmptyString(); }
        }

        public int SubjectId { get; set; }
        public int GradeId { get; set; }

        private string _gradeName = string.Empty;
        public string GradeName
        {
            get { return _gradeName; }
            set { _gradeName = value.ConvertNullToEmptyString(); }
        }

        private string _bankName = string.Empty;
        public string BankName
        {
            get { return _bankName; }
            set { _bankName = value.ConvertNullToEmptyString(); }
        }

        private string _author = string.Empty;
        public string Author
        {
            get { return _author; }
            set { _author = value.ConvertNullToEmptyString(); }
        }

        public int BankId { get; set; }

        private string _fileName = string.Empty;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value.ConvertNullToEmptyString(); }
        }

        public int VirtualTestFileId { get; set; }

        private string _fileKey = string.Empty;
        public string FileKey
        {
            get { return _fileKey; }
            set { _fileKey = value.ConvertNullToEmptyString(); }
        }

        private string _testName = string.Empty;
        public string TestName
        {
            get { return _testName; }
            set { _testName = value.ConvertNullToEmptyString(); }
        }

        public int AuthorUserId { get; set; }
        public int DistrictId { get; set; }
        public int TestId { get; set; }
        public int BankShareDistrictID { get; set; }
    }
}
