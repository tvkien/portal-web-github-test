using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetReviewDetails
    {
        private string ticket = string.Empty;
        private string studentName = string.Empty;
        private string className = string.Empty;
        private string teacherName = string.Empty;
        private string schoolName = string.Empty;
        private string fileDisposition = string.Empty;
        private string inputFileName = string.Empty;
        private string uploadedBy = string.Empty;
        private string outputFileName = string.Empty;

        public short RosterPosition { get; set; }
        public int? BubbleSheetFileId { get; set; }
        public int BubbleSheetId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int TeacherId { get; set; }
        public int SchoolId { get; set; }
        public int? ResultCount { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? UploadedDate { get; set; }
        public bool IsManualEntry { get; set; }
        public int? VirtualTestId { get; set; }
        public DateTime? ResultDate { get; set; }

        public bool HasBubbleSheetFile
        {
            get { return UploadedDate.HasValue || BubbleSheetFileId.HasValue; }
        }

        public bool HasCreatedBubbleSheetFile
        {
            get { return FileDisposition.Equals("Created"); }
        }

        public string Ticket
        {
            get { return ticket; }
            set { ticket = value.ConvertNullToEmptyString(); }
        }

        public string StudentName
        {
            get { return studentName; }
            set { studentName = value.ConvertNullToEmptyString(); }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }

        public string TeacherName
        {
            get { return teacherName; }
            set { teacherName = value.ConvertNullToEmptyString(); }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string FileDisposition
        {
            get { return fileDisposition; }
            set { fileDisposition = value.ConvertNullToEmptyString(); }
        }

        public string InputFileName
        {
            get { return inputFileName; }
            set { inputFileName = value.ConvertNullToEmptyString(); }
        }

        public string OutputFileName
        {
            get { return outputFileName; }
            set { outputFileName = value.ConvertNullToEmptyString(); }
        }

        public string UploadedBy
        {
            get { return uploadedBy; }
            set { uploadedBy = value.ConvertNullToEmptyString(); }
        }
    }
}