using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntryInputScoreModel
    {
        public ResultEntryCustomScore CustomScore { get; set; }
        public List<ResultEntryCustomSubScore> CustomSubScores { get; set; }

        public string StudentTestResultScores { get; set; }
        public string StudentTestResultSubScores { get; set; }
        public string ActualTestResultScoresJson { get; set; }
        public bool CheckPublishForm { get; set; }
        public QTITestClassAssignmentData QTITestClassAssignmentData { get; set; }

        public List<string> EntryResultDates { get; set; }
        public List<StudentResultDate> StudentResultDates { get; set; }
        public Preferences Preferences { get; set; }

        public List<PerformanceBandSettingScoreModel> PerformanceBandSettingScores { get; set; }
        public List<PerformanceBandSettingScoreModel> PerformanceBandSettingSubScores { get; set; }
    }

    public class StudentResultDate
    {
        public int StudentId { get; set; }
        public List<string> ResultDates { get; set; }
    }
}
