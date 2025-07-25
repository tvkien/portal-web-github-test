using LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class EntryResultAllStudentModel
    {
        public List<StudentInVirtualTestClass> Students { get; set; }
        public List<string> Tabs { get; set; }
        public List<TestResultScoreAllArtifact> Artifacts { get; set; }
        public ResultEntryCustomScore CustomScore { get; set; }
        public ResultEntryCustomSubScore CustomSubScore { get; set; }
    }
}
