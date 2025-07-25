using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetClassViewViewModel
    {
        public BubbleSheetClassViewViewModel()
        {
            BubbleSheetStudentDatas = new List<BubbleSheetClassViewStudentViewModel>();
        }
        public string Ticket { get; set; }
        public int ClassId { get; set; }
        public string TestName { get; set; }
        public List<BubbleSheetClassViewStudentViewModel> BubbleSheetStudentDatas { get; set; }
    }

    public class BubbleSheetClassViewSaveDataViewModel
    {
        public BubbleSheetClassViewStudentViewModel BubbleSheetStudentData { get; set; }
        public List<BubbleSheetClassViewAnswer> BubbleSheetAnswers { get; set; }
    }

    public class BubbleSheetClassViewStudentViewModel
    {
        public BubbleSheetClassViewStudentViewModel()
        {
            BubbleSheetAnswers = new List<BubbleSheetClassViewAnswer>();
        }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Graded { get; set; }
        public string PointsEarned { get; set; }
        public int StudentId { get; set; }
        public int BubbleSheetId { get; set; }
        public bool IsChanged { get; set; } //answerletter is difference from original
        public int RosterPosition { get; set; }
        public string ArtifactFileName { get; set; }
        public BubbleSheetFileViewModel BubbleSheetFileViewModel { get; set; }
        public List<BubbleSheetClassViewAnswer> BubbleSheetAnswers { get; set; }
    }
}
