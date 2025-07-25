using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntrySaveScoreModel
    {
        public int VirtualTestCustomScoreId { get; set; }
        public DTLStudentAndTestResultScore TestResultScore { get; set; }
        public List<DTLStudentAndTestResultSubScore> TestResultSubScores { get; set; }
    }
}
