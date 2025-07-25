using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class TestResultDetails
    {
        public int TestResultID { get; set; }
        public DateTime ResultDate { get; set; }
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int DistrictTermID { get; set; }
        public string DistrictTermName { get; set; }
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int VirtualTestID { get; set; }
        public string TestName { get; set; }

        public int? TestResultScoreID { get; set; }
        public decimal? ScorePercent { get; set; }
        public decimal? ScorePercentage { get; set; }
        public decimal? ScoreRaw { get; set; }
        public decimal? ScoreLexile { get; set; }
        public decimal? ScoreIndex { get; set; }
        public decimal? ScoreScaled { get; set; }
        public bool? UsePercent { get; set; }
        public bool? UsePercentage { get; set; }
        public bool? UseRaw { get; set; }
        public bool? UseScaled { get; set; }
        public bool? UseLexile { get; set; }
        public bool? UseIndex { get; set; }
        public int? PointsPossible { get; set; }
        public int? AchievementLevel { get; set; }
        public bool? UseGradeLevelEquiv { get; set; }
        public decimal? ScoreGradeLevelEquiv { get; set; }
        public string Name { get; set; }
        public bool? MetStandard { get; set; }
        public string AchieveLevelName { get; set; }
        public decimal? ScoreCustomN_1 { get; set; }
        public decimal? ScoreCustomN_2 { get; set; }
        public decimal? ScoreCustomN_3 { get; set; }
        public decimal? ScoreCustomN_4 { get; set; }
        public string ScoreCustomA_1 { get; set; }
        public string ScoreCustomA_2 { get; set; }
        public string ScoreCustomA_3 { get; set; }
        public string ScoreCustomA_4 { get; set; }

        public int? TestResultSubScoreID { get; set; }
        public decimal? ScorePercentSub { get; set; }
        public decimal? ScorePercentageSub { get; set; }
        public decimal? ScoreRawSub { get; set; }
        public decimal? ScoreScaledSub { get; set; }
        public decimal? ScoreLexileSub { get; set; }
        public bool? UsePercentSub { get; set; }
        public bool? UsePercentageSub { get; set; }
        public bool? UseRawSub { get; set; }
        public bool? UseScaledSub { get; set; }
        public bool? UseLexileSub { get; set; }
        public int? PointsPossibleSub { get; set; }
        public int? AchievementLevelSub { get; set; }
        public bool? UseGradeLevelEquivSub { get; set; }
        public decimal? ScoreGradeLevelEquivSub { get; set; }
        public string NameSub { get; set; }
        public bool? MetStandardSub { get; set; }
        public decimal? ScoreCustomN_1Sub { get; set; }
        public decimal? ScoreCustomN_2Sub { get; set; }
        public decimal? ScoreCustomN_3Sub { get; set; }
        public decimal? ScoreCustomN_4Sub { get; set; }
        public string ScoreCustomA_1Sub { get; set; }
        public string ScoreCustomA_2Sub { get; set; }
        public string ScoreCustomA_3Sub { get; set; }
        public string ScoreCustomA_4Sub { get; set; }

        public int? TestResultProgramID { get; set; }
        public int? ProgramID { get; set; }
        public string ProgramName { get; set; }

        public int? AnswerID { get; set; }
        public int? VirtualQuestionID { get; set; }
        public int? QuestionOrder { get; set; }
        public int? QTIItemID { get; set; }
        public string CorrectAnswer { get; set; }
        public int? PointsEarned { get; set; }
        public int? AnswerPointsPossible { get; set; }
        public bool? WasAnswered { get; set; }
        public string AnswerLetter { get; set; }
        public string AnswerText { get; set; }

        public int? AnswerSubID { get; set; }
        public int? VirtualQuestionSubID { get; set; }
        public int? PointsEarnedSub { get; set; }
        public int? AnswerPointsPossibleSub { get; set; }
        public string AnswerLetterSub { get; set; }
        public string AnswerTextSub { get; set; }
    }
}