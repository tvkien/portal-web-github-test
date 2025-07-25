using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetDetailsStudentListViewModel
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Graded { get; set; }
        public string PointsEarned { get; set; }
        public int Id { get; set; }
        public BubbleSheetFinalStatus BubbleSheetFinalStatus { get; set; }
        public int BubbleSheetId { get; set; }
        public string ArtifactFileName { get; set; }
    }
}
