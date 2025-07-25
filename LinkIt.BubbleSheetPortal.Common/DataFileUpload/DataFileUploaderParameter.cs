using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload
{
    public class DataFileUploaderParameter
    {
        public int CurrentUserId { get; set; }
        public string ZipFileName { get; set; }
        public int QtiGroupId { get; set; }
        public string ExtractedFoler { get; set; }
        public string ItemSetPath { get; set; }
        public bool UploadS3 { get; set; }
        public string AUVirtualTestBucketName { get; set; }
        public string S3TestMedia { get; set; }
        public string AUVirtualTestFolder { get; set; }
        public string S3Domain { get; set; }
        public bool UploadTo3pItem { get; set; }
        public int QTI3pSourceId { get; set; }
        public string TimeStamp { get; set; }
        public int DataFileUploadLogId { get; set; }
        public string S3Subdomain { get; set; }
        public List<GradeMapping> GradeMappingList { get; set; }
    }

    public class GradeMapping
    {
        public int GradeId { get; set; }
        public string GradeLevel { get; set; }
    }
}
