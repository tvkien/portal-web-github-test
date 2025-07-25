using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassList
    {
        private string className = string.Empty;
        private string termName = string.Empty;
        private string primaryTeacher = string.Empty;
        private string schoolName = string.Empty;

        public int ClassId { get; set; }
        public bool IsLocked { get; set; }
        public int? SchoolID { get; set; }
        public int? UserId { get; set; }

        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }

        public string TermName
        {
            get { return termName; }
            set { termName = value.ConvertNullToEmptyString(); }
        }

        public string PrimaryTeacher
        {
            get { return primaryTeacher; }
            set { primaryTeacher = value.ConvertNullToEmptyString(); }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public DateTime? TermStartDate { get; set; }
        public DateTime? TermEndDate { get; set; }
        public string Students { get; set; }
        public string Teachers { get; set; }
        public string ModifiedBy { get; set; }
        public string ClassType { get; set; }
        public string Subjects { get; set; }
    }
}
