namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetReviewDetailsViewModel
    {
        public string Ticket { get; set; }
        public bool CanAccess { get; set; }
        public int ClassId { get; set; }
        public bool HasGenericSheet { get; set; }
        public bool IsMultipage { get; set; }
        public string TestName { get; set; }
    }
}