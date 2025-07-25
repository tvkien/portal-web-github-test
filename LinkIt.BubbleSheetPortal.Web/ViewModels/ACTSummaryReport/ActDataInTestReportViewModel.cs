namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTSummaryReport
{
    public class ActDataInTestReportViewModel
    {
        public decimal EnglishScore { get; set; }
        public decimal MathScore { get; set; }
        public decimal ReadingScore { get; set; }
        public decimal ScienceScore { get; set; }
        public decimal WritingScore { get; set; }
        public decimal EWScore { get; set; }
        public decimal CompositeScore { get; set; }
        public int TotalStudents { get; set; }

        //public decimal CompositeScore
        //{
        //    get { return (EnglishScore + MathScore + ReadingScore + ScienceScore + WritingScore + EWScore)/6; }
        //}

        public void CalculateCompositeScore()
        {
            if (EnglishScore == -1
                && MathScore == -1
                && ReadingScore == -1
                && ScienceScore == -1
                && WritingScore == -1
                && EWScore == -1)
            {
                CompositeScore = -1;
            }
            else
            {
                CompositeScore = ((EnglishScore == -1 ? 0 : EnglishScore)
                                  + (MathScore == -1 ? 0 : MathScore)
                                  + (ReadingScore == -1 ? 0 : ReadingScore)
                                  + (ScienceScore == -1 ? 0 : ScienceScore)
                                  + (WritingScore == -1 ? 0 : WritingScore)
                                  + (EWScore == -1 ? 0 : EWScore))/6;
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

        public ActSummaryReportType ReportType { get; set; }
    }
}