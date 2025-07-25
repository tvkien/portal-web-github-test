using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetListItem : ValidatableEntity<BubbleSheetListItem>
    {
        private string ticket = string.Empty;
        private string subjectName = string.Empty;
        private string gradeName = string.Empty;
        private string bankName = string.Empty;
        private string testName = string.Empty;
        private string className = string.Empty;
        private string teacherName = string.Empty;
        private string schoolName = string.Empty;
        private string userName = string.Empty;
        private string groupName = string.Empty;
        private string groupTeacherName = string.Empty;
        private string classIds = string.Empty;

        public int GradedCount { get; set; }
        public int TotalSheets { get; set; }
        public int CreatedByUserId { get; set; }
        public int UserId { get; set; }
        public int? DistrictId { get; set; }
        public int? SchoolId { get; set; }
        public int ClassId { get; set; }
        public bool IsArchived { get; set; }
        public bool IsManualEntry { get; set; }
        public DateTime DateCreated { get; set; } 
        public DateTime? DateUpdated { get; set; }
        public int? PrintingGroupJobID { get; set; }
        public int BankID { get; set; }

        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value.ConvertNullToEmptyString(); }
        }

        public string GroupName
        {
            get { return groupName; }
            set { groupName = value.ConvertNullToEmptyString(); }
        }

        public string Ticket
        {
            get { return ticket; }
            set { ticket = value.ConvertNullToEmptyString(); }
        }

        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value.ConvertNullToEmptyString(); }
        }

        public string GradeName
        {
            get { return gradeName; }
            set { gradeName = value.ConvertNullToEmptyString(); }
        }

        public string BankName
        {
            get { return bankName; }
            set { bankName = value.ConvertNullToEmptyString(); }
        }

        public string TestName
        {
            get { return testName; }
            set { testName = value.ConvertNullToEmptyString(); }
        }

        public string TeacherName
        {
            get { return teacherName; }
            set { teacherName = value.ConvertNullToEmptyString(); }
        }

        public string GroupTeacherName
        {
            get { return groupTeacherName; }
            set { groupTeacherName = value.ConvertNullToEmptyString(); }
        }

        public string ClassIds
        {
            get { return classIds; }
            set { classIds = value.ConvertNullToEmptyString(); }
        }

        public int SheetsMissing
        {
            get { return TotalSheets - GradedCount; }
        }

        public int UnmappedCount { get; set; }

        public int? VirtualTestSubTypeID { get; set; }

        public int? Fini { get; set; }

        public int? Review { get; set; }

        public int? Ungraded { get; set; }
    }
}