using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SpecializedTestResult
    {
        private string bankName = string.Empty;
        private string schoolName = string.Empty;
        private string teacherNameCustom = string.Empty;
        private string classNameCustom = string.Empty;
        private string studentCustom = string.Empty;

        public int BankId { get; set; }
        public int StudentId { get; set; }
        public string StudentCode { get; set; }

        public string BankName
        {
            get { return bankName; }
            set { bankName = value.ConvertNullToEmptyString(); }
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