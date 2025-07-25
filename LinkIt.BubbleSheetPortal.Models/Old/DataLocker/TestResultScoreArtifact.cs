using System;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class TestResultScoreArtifact
    {
        public int TestResultScoreUploadFileID { get; set; }
        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsLink { get; set; }

        public DateTime UploadDate { get; set; }

        public string TagValue { get; set; }

        public Guid? DocumentGuid { get; set; }

        public int? CreatedBy { get; set; }

    }
}
