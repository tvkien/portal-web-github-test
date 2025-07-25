using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTSummaryReport
{
    public class ActSingleTestReportViewModel
    {
        public List<ActDataInTestReportViewModel> ListDataInTest { get; set; }
        public ActDataInTestReportViewModel AverageScores { get; set; }

        public void CalculateAverageScores()
        {
            if (ListDataInTest.Count == 0)
            {
                AverageScores = new ActDataInTestReportViewModel
                                    {
                                        DisplayName = "Average",
                                        EnglishScore = -1,
                                        EWScore = -1,
                                        MathScore = -1,
                                        ReadingScore = -1,
                                        ScienceScore = -1,
                                        WritingScore = -1,
                                        CompositeScore = -1,
                                        TotalStudents = 0,
                                        ReportType = ReportType
                                    };
            }
            else
            {
                AverageScores = new ActDataInTestReportViewModel
                                    {
                                        DisplayName = "Average",
                                        EnglishScore =
                                            ListDataInTest.Any(x => x.EnglishScore >= 0)
                                                ? ListDataInTest.Where(x => x.EnglishScore >= 0).Average(
                                                    x => x.EnglishScore)
                                                : -1,
                                        EWScore =
                                            ListDataInTest.Any(x => x.EWScore >= 0)
                                                ? ListDataInTest.Where(x => x.EWScore >= 0).Average(x => x.EWScore)
                                                : -1,
                                        MathScore =
                                            ListDataInTest.Any(x => x.MathScore >= 0)
                                                ? ListDataInTest.Where(x => x.MathScore >= 0).Average(x => x.MathScore)
                                                : -1,
                                        ReadingScore =
                                            ListDataInTest.Any(x => x.ReadingScore >= 0)
                                                ? ListDataInTest.Where(x => x.ReadingScore >= 0).Average(
                                                    x => x.ReadingScore)
                                                : -1,
                                        ScienceScore =
                                            ListDataInTest.Any(x => x.ScienceScore >= 0)
                                                ? ListDataInTest.Where(x => x.ScienceScore >= 0).Average(
                                                    x => x.ScienceScore)
                                                : -1,
                                        WritingScore =
                                            ListDataInTest.Any(x => x.WritingScore >= 0)
                                                ? ListDataInTest.Where(x => x.WritingScore >= 0).Average(
                                                    x => x.WritingScore)
                                                : -1,
                                        CompositeScore =
                                            ListDataInTest.Any(x => x.CompositeScore >= 0)
                                                ? ListDataInTest.Where(x => x.CompositeScore >= 0).Average(
                                                    x => x.CompositeScore)
                                                : -1,
                                        TotalStudents =
                                            ListDataInTest.Any()
                                                ? (int) Math.Ceiling(ListDataInTest.Average(x => x.TotalStudents))
                                                : 0,
                                        ReportType = ReportType
                                    };
            }
        }
        
        public string TestName { get; set; }
        public ActSummaryReportType ReportType { get; set; }
        public int Index { get; set; }
        public int VirtualTestSubTypeId { get; set; }
    }
}