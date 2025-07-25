namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentRoster
    {
        public int BubbleSheetId { get; set; }
        public string Ticket { get; set; }
        public int RosterPosition { get; set; }
        public int? StudentId { get; set; }
        public int ClassId { get; set; }
    }
}