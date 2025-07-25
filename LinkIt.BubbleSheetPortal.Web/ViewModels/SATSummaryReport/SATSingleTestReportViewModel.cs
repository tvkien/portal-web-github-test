using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATSummaryReport
{
    public class SATSingleTestReportViewModel
    {
        public List<SATDataInTestReportViewModel> ListDataInTest { get; set; }
        public SATDataInTestReportViewModel AverageScores { get; set; }

        public void CalculateAverageScores()
        {
            if (ListDataInTest.Count == 0)
            {
                AverageScores = new SATDataInTestReportViewModel
                                    {
                                        DisplayName = "Average",
                                        CompositeScore = -1,
                                        SubScores = new List<SATSummarySubScore>(),
                                        TotalStudents = 0,
                                        ReportType = ReportType
                                    };
            }
            else
            {
                var allSubScores = new List<SATSummarySubScore>();
                foreach (var dataInTest in ListDataInTest)
                {
                    allSubScores.AddRange(dataInTest.SubScores);
                }
                var allSectionNames = allSubScores.Select(en => en.SectionName).Distinct();

                AverageScores = new SATDataInTestReportViewModel
                                {
                                    DisplayName = "Average",
                                    SubScores = allSectionNames.Select(en =>
                                        new SATSummarySubScore
                                        {
                                            SectionName = en,
                                            Score = allSubScores.Any(x => x.SectionName == en && x.Score >= 0)
                                                ? allSubScores.Where(x => x.SectionName == en && x.Score >= 0)
                                                .Average(x => x.Score)
                                                : 0
                                        }).ToList(),
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
        public SATSummaryReportType ReportType { get; set; }
        public int Index { get; set; }
    }
}