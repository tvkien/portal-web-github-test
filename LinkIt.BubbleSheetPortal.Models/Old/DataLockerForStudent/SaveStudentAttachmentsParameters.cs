using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class SaveStudentAttachmentsParameters
    {
        public SaveStudentAttachmentsParameters()
        {
            DeletedItems = new List<StudentAttachmentItem>();
            AddedItems = new List<StudentAttachmentItem>();
        }
        public int? TestResultScoreID { get; set; }
        public int? TestResultSubScoreID { get; set; }
        public List<StudentAttachmentItem> DeletedItems { get; set; }
        public List<StudentAttachmentItem> AddedItems { get; set; }
    }

    public class StudentAttachmentItem
    {
        public int TestResultScoreUploadFileID { get; set; }
        public Guid? DocumentGuid { get; set; }
        public string FileName { get; set; }
        public bool? IsUrl { get; set; }
        public string Name { get; set; }
    }
}
