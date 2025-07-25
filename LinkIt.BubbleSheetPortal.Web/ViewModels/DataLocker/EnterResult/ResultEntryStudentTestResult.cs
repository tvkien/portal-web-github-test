using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntryStudentTestResult
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }        
        public ResultEntryTestResultScore TestResultScore { get; set; }
        public List<ResultEntryTestResultSubScore> TestResultSubScores { get; set; }
    }
}
