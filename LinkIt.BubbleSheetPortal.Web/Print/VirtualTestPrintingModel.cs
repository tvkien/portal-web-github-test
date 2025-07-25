using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class VirtualTestPrintingModel
    {
        public VirtualTestPrintingModel()
        {
            S3Domain = string.Empty;
            UpLoadBucketName = string.Empty;
            AUVirtualTestFolder = string.Empty;
        }
        public List<VirtualSectionModel> Sections { get; set; }

        private string _testTitle;

        public string TestTitle
        {
            get { return _testTitle ?? (_testTitle = string.Empty); }
            set { _testTitle = value; }
        }
        public string TeacherName { get; set; }
        public string IncludeCoverPage { get; set; }
        public string ClassName { get; set; }
        public string TestInstruction { get; set; }
        public List<string> Css { get; set; }
        public List<string> JS { get; set; }

        private List<KeyValuePair<string, string>> preferenceObjects;

        public List<KeyValuePair<string, string>> PreferenceObjects
        {
            get
            {
                if (preferenceObjects == null) preferenceObjects = new List<KeyValuePair<string, string>>();
                return preferenceObjects;
            }
            set
            {
                preferenceObjects = value;
            }
        }

        public string IncludePageNumbers { get; set; }
        public string StartCountingOnCover { get; set; }
        public string ColumnCount { get; set; }
        public string ShowSectionHeadings { get; set; }
        public string ShowQuestionBorders { get; set; }
        public string ExtendedTextAreaShowLines { get; set; }
        public string AnswerLabelFormat { get; set; }
        public string DrawReferenceBackground { get; set; }
        public string ExtendedTextAreaAnswerOnSeparateSheet { get; set; }
        public string QuestionPrefix { get; set; }

        public int? ExtendedTextAreaNumberOfLines { get; set; }
        public string IncludeStandards { get; set; }
        public string IncludeTags { get; set; }
        public string TestItemMediaPath { get; set; }
        public int UserDistrictId { get; set; }
        //public bool UseS3Content { get; set; }
        public string S3Domain { get; set; }
        public string UpLoadBucketName { get; set; }
        public string AUVirtualTestFolder { get; set; }
        public bool IncludeRationale { get; set; }
        public bool IncludeGuidance { get; set; }
        public string AUVirtualTestROFolder { get; set; }
        public bool IsCustomItemNaming { get; set; }

        public bool? IsNumberQuestions { get; set; }
    }
}
