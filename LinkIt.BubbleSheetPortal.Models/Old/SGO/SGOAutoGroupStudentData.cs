namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOAutoGroupStudentData
    {
        public int SGOStudentID { get; set; }
        public int StudentID { get; set; }
        public decimal AveragePercentScore { get; set; }
        public decimal MinPercentage { get; set; }
        public decimal MaxPercentage { get; set; }
        public int SGOGroupID { get; set; }
    }
}