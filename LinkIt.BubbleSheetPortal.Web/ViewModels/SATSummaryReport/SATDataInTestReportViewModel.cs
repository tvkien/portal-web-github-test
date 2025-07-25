using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATSummaryReport
{
    public class SATDataInTestReportViewModel
    {
        public List<SATSummarySubScore> SubScores { get; set; }

        //public decimal EnglishScore { get; set; }
        //public decimal MathScore { get; set; }
        //public decimal ReadingScore { get; set; }
        //public decimal ScienceScore { get; set; }
        //public decimal WritingScore { get; set; }
        //public decimal EWScore { get; set; }

        public decimal CompositeScore { get; set; }
        public int TotalStudents { get; set; }

        public void CalculateCompositeScore()
        {            
            if (SubScores.All(en=>en.Score == -1))
            {
                CompositeScore = -1;
            }
            else
            {
                CompositeScore = SubScores.Sum(en=>en.Score==-1?0:en.Score)/SubScores.Count;
            }
        }

        public string DisplayName { get; set; }
        public string SortName { get; set; }
        
        /// <summary>
        /// If report type is class/student, this is StudentID.
        /// If report type is teacher/school, this is ClassID.
        /// If report type is district, this is SchoolID
        /// </summary>
        public int ID { get; set; }

        public SATSummaryReportType ReportType { get; set; }
    }
}