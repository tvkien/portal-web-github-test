namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOStudent
    {
        public int SGOStudentID { get; set; }
        public int SGOID { get; set; }
        public int ClassID { get; set; }
        public int StudentID { get; set; }
        public int? SGOGroupID { get; set; }        

        public int? Type { get; set; }
        public int? ArchievedTarget { get; set; }
    }
}