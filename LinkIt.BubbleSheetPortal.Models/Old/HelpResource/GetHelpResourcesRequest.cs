namespace LinkIt.BubbleSheetPortal.Models.HelpResource
{
    public class GetHelpResourcesRequest
    {
        public string SearchText { get; set; }
        public string HelpResourceCaterogyIDs { get; set; }
        public int RoleId { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public int StartRow { get; set; }
        public int PageSize { get; set; }
    }
}
