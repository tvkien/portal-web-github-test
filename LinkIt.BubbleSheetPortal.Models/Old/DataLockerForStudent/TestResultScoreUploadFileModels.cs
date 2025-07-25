using System;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class TestResultScoreUploadFileModels
    {
        public int? TestResultScoreID { get; set; }
        public int? TestResultSubScoreID { get; set; }
        public string ArtifactName { get; set; }
        public int IsUrl { get; set; }
        public DateTime UploadedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Tag { get; set; }
        public int FileNumber { get; set; }
        public int TestResultID { get; set; }
        public string MetaData { get; set; }
        public int? Order { get; set; }
    }
}
