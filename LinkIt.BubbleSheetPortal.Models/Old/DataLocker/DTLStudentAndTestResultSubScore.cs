using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class DTLStudentAndTestResultSubScore
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public string AltCode { get; set; }
        public int? TestResultID { get; set; }
        public int? VirtualTestID { get; set; }
        public DateTime? ResultDate { get; set; }
        public int? ClassID { get; set; }
        public int? TestResultScoreID { get; set; }
        public int? TestResultScoreSubID { get; set; }
        public string Name { get; set; }
        public decimal? ScorePercent { get; set; }
        public decimal? ScorePercentage { get; set; }
        public decimal? ScoreRaw { get; set; }
        public decimal? ScoreScaled { get; set; }
        public decimal? ScoreCustomN_1 { get; set; }
        public decimal? ScoreCustomN_2 { get; set; }
        public decimal? ScoreCustomN_3 { get; set; }
        public decimal? ScoreCustomN_4 { get; set; }
        public string ScoreCustomA_1 { get; set; }
        public string ScoreCustomA_2 { get; set; }
        public string ScoreCustomA_3 { get; set; }
        public string ScoreCustomA_4 { get; set; }
        public List<TestResultScoreArtifact> Artifacts { get; set; }

        public List<TestResultSubScoreNote> Notes { get; set; }

        public bool HasOtherScore { get; set; }
        public bool IsAutoSave { get; set; }
        public List<Dictionary<string, string>> Colors { get; set; }
    }
}
