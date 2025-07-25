using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntryPrintModel
    {
        public int VirtualtestId { get; set; }
        public int ClassId { get; set; }
        public string StudentsIdSelectedString { get; set; }
        public bool AllColumn { get; set; }
        public List<string> OverrallScoreNameList { get; set; }
        public List<SubScorePart> SubScorePartList { get; set; }
        public string Layout { get; set; }
        public string ScoreDescription { get; set; }
        public string IncludeRubricDescription { get; set; }
        public DateTime EntryResultDate { get; set; }
        public string StudentTestResultScores { get; set; }
        public string StudentTestResultSubscores { get; set; }
        public int TemplateId { get; set; }
    }

    public class SubScorePart
    {
        public string Name { get; set; }
        public List<string> SubScoreNameList { get; set; }
    }
}
