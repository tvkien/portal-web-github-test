namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BankPublishedDistrictViewModel
    {
        public int BankDistrictId { get; set; }
        public string Name { get; set; }
        public int BankId { get; set; }
        public int DistrictId { get; set; }

        public bool IsRightDeleteDistrictBank{ set; get; }
    }
}