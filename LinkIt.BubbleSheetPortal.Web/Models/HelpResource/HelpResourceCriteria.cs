namespace LinkIt.BubbleSheetPortal.Web.Models.HelpResource
{
    public class HelpResourceCriteria : DataTableRequest
    {
        public bool PageLoad { get; set; }
        public string SelectedCategories { get; set; }
        public string SearchText { get; set; }
    }
}