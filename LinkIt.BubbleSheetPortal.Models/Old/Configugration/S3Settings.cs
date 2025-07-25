using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Models.Configugration
{
    public class S3Settings
    {
        public string ChytenReportBucket { get; set; }
        public string ChytenReportFolder {  get { return ConfigurationManager.AppSettings["ChytenReportFolder"]; } }

        public string SGOBucketName { get; set; }
        public string SGOFolder { get { return ConfigurationManager.AppSettings["SGOFolder"]; } }

        public string LessonBucketName { get; set; }
        public string LessonFolder { get { return ConfigurationManager.AppSettings["LessonFolder"]; } }

        public string GuideBucketName { get; set; }
        public string GuideFolder { get { return ConfigurationManager.AppSettings["GuideFolder"]; } }

        public string RubricBucketName { get; set; }
        public string RubricFolder { get { return ConfigurationManager.AppSettings["RubricFolder"]; } }

        public string AnswerKeySampleFileBucketName { get; set; }
        public string AnswerKeySampleFileFolderName { get { return ConfigurationManager.AppSettings["AnswerKeySampleFileFolderName"]; } }

        public string ACTReportBucket { get; set; }
        public string ACTReportFolder { get { return ConfigurationManager.AppSettings["ACTReportFolder"]; } }
        public string BubbleSheetBucketName { get; set; }
        public string BubbleSheetFolder { get { return ConfigurationManager.AppSettings["BubbleSheetFolder"]; } }

        public string AUVirtualTestBucketName { get; set; }
        public string AUVirtualTestFolder {
            get { return ConfigurationManager.AppSettings["AUVirtualTestFolder"]; }
        }
        public string AUVirtualTestROFolder { get { return ConfigurationManager.AppSettings["AUVirtualTestROFolder"]; } }
        public string KnowsysStateImageFolder { get { return ConfigurationManager.AppSettings["KnowsysStateImageFolder"]; } }

        public string S3ItemBucketName { get; set; }
        public string ACTReportChartFolder { get { return ConfigurationManager.AppSettings["ACTReportChartFolder"]; }}

        public string HelpResourceBucket { get; set; }
        public string HelpReourceFolder { get { return ConfigurationManager.AppSettings["HelpReourceFolder"]; } }
        public string NavigatorBucket { get; set; }
        public string NavigatorFolder { get { return ConfigurationManager.AppSettings["NavigatorFolder"]; } }
        public string ExportClassTestAssBucket { get; set; }
        public string S3Domain { get; set; }
        public string S3CSSKey { get; set; }
        public string SlideShowKey { get; set; }
        public string CssBucketName { get; set; }
        public string AblesReportBucket { get; set; }
        public string ReportPrintingBucketName { get; set; }
        public string ReportPrintingFolder { get { return ConfigurationManager.AppSettings["ReportPrintingFolder"]; } }
        private string _tldsBucket;
        private string _dtlBucket;

        public string TLDSBucket
        {
            get
            {
                return _tldsBucket;
            }
            set { _tldsBucket = value; }
        }
        public string DTLBucket
        {
            get
            {
                return _dtlBucket;
            }
            set { _dtlBucket = value; }
        }
        public string S3AssessmentArtifactBucketName { get; set; }
        public string S3AssessmentArtifactFolderName { get; set; }
    }
}
