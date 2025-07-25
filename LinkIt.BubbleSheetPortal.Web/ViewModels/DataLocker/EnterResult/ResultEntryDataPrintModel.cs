using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntryDataPrintModel
    {
        private string _testTitle;
        private string _className;
        public string TestTitle
        {
            get { return _testTitle ?? (_testTitle = string.Empty); }
            set { _testTitle = value; }
        }

        public string RubricDescription { get; set; }
        public string ClassName
        {
            get { return _className ?? (_className = string.Empty); }
            set { _className = value; }
        }
        public ResultEntryCustomScore CustomScore { get; set; }
        public List<ResultEntryCustomSubScore> CustomSubScores { get; set; }

        public List<DTLStudentAndTestResultScore> StudentTestResultScores { get; set; }
        public List<DTLStudentAndTestResultSubScore> StudentTestResultSubScores { get; set; }
        public bool AllColumn { get; set; }
        public List<string> OverrallScoreNameList { get; set; }
        public List<SubScorePart> SubScorePartList { get; set; }
        public string Layout { get; set; }
        public string ScoreDescription { get; set; }
        public string IncludeRubricDescription { get; set; }
        public string DateFormatPrint { get; set; }

        public string IncludeCoverPage { get; set; }
        public List<string> Css { get; set; }
        public List<string> JS { get; set; }

        public string IncludePageNumbers { get; set; }
        public string StartCountingOnCover { get; set; }
        public string ColumnCount { get; set; }
        public string S3Domain { get; set; }
        public string UpLoadBucketName { get; set; }
        public string AUVirtualTestFolder { get; set; }
        public string AUVirtualTestROFolder { get; set; }
    }
}
