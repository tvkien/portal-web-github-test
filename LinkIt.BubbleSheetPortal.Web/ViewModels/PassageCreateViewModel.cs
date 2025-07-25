namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PassageCreateViewModel
    {
        public PassageCreateViewModel()
        {
            FromPassageEditor = false;
            FromItemSetEditor = false;
            FromItemEditor = false;
            FromVirtualTestEditor = false;
            FromTestEditor = false;
        }
        public bool FromPassageEditor { get; set; }
        public bool FromItemSetEditor { get; set; }
        public bool FromItemEditor { get; set; }
        public bool FromVirtualTestEditor { get; set; }
        public bool FromTestEditor { get; set; }
        public int? QtiItemGroupId { get; set; }
    }
    
}
