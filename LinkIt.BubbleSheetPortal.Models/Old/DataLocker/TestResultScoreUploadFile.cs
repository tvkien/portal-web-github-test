using System;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class TestResultScoreUploadFile
    {
        public int TestResultScoreUploadFileID { get; set; }
        public int? TestResultScoreID { get; set; }
        public int? TestResultSubScoreID { get; set; }
        public string FileName { get; set; }
        public bool IsUrl { get; set; }
        public DateTime UploadDate { get; set; }
        public string Tag { get; set; }
        public Guid? DocumentGuid { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class TestResultScoreUploadFileMapModel
    {
        public int TestResultScoreUploadFileID { get; set; }

        public int? TestResultScoreID { get; set; }

        public string FileName { get; set; }

        public bool IsUrl { get; set; }

        public DateTime UploadDate { get; set; }

        public int StudentID { get; set; }

        public int ClassID { get; set; }

        public int VirtualTestID { get; set; }

        public string Tag { get; set; }

        public Guid? DocumentGuid { get; set; }

        public int? CreatedBy { get; set; }
        public int LineIndex { get; set; }
    }


    public class TestResultSubScoreUploadFileMapModel
    {
        public int TestResultScoreUploadFileID { get; set; }

        public int? TestResultSubScoreID { get; set; }

        public string FileName { get; set; }

        public bool IsUrl { get; set; }

        public DateTime UploadDate { get; set; }

        public int StudentID { get; set; }

        public int ClassID { get; set; }

        public int VirtualTestID { get; set; }

        public string SubScoreName { get; set; }

        public string Tag { get; set; }

        public Guid? DocumentGuid { get; set; }

        public int? CreatedBy { get; set; }
        public int LineIndex { get; set; }
    }

}
