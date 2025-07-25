namespace LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets
{
    public class GenericSheetViewModel
    {
        public string Ticket { get; set; }
        public int? ClassId { get; set; }
        public bool HasNoFilesUploaded { get; set; }

        public int VirtualTestSubTypeId { get; set; }
        public string TestName { get; set; }
    }
}
