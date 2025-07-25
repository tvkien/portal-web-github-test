namespace LinkIt.BubbleSheetPortal.Models.DTOs.Survey
{
    public class SurveyCloneInformationDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string S3Domain { get; set; }
        public string AUVirtualTestBucketName { get; set; }
        public string AUVirtualTestFolder { get; set; }
    }
}
