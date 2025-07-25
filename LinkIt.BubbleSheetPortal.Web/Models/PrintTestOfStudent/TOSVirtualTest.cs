using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader;

namespace LinkIt.BubbleSheetPortal.Web.Models.PrintTestOfStudent
{
    public class TOSVirtualTest
    {
        public List<TOSVirtualSection> VirtualSections { get; set; }
        public List<TestOnlineSessionAnwer> Answers { get; set; }
        public VirtualTestData VirtualTest { get; set; }
        public decimal? ScoreRaw { get; set; }
        public string TestName { get; set; }
        public string StudentName { get; set; }
        public int? UserDistrictId { get; set; }
        public List<string> JavaScripts { get; set; } 
        public List<string> Css { get; set; }
        public int TestFeedbackId { get; set; }
        public string TestFeedback { get; set; }
        public int? TotalPointsEarned { get; set; }
        public int? TotalPointsPossible { get; set; }
        public decimal? Percent { get; set; }
        public bool PrintGuidance { get; set; }
        public bool UseS3Content { get; set; }

        public bool TeacherFeedback { get; set; }
        public bool TheCoverPage { get; set; }
        public bool TheCorrectAnswer { get; set; }
        public bool Passages { get; set; }
        public bool GuidanceAndRationale { get; set; }
        public bool TheQuestionContent { get; set; }

        public int NumberOfColumn { get; set; }
        public bool ShowQuestionPrefix { get; set; }
        public bool ShowBorderAroundQuestions { get; set; }

        public bool ExcludeTestScore { get; set; }

    }
}