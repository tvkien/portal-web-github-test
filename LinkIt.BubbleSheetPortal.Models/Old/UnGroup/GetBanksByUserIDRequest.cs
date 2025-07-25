namespace LinkIt.BubbleSheetPortal.Models.Old.UnGroup
{
    public class GetBanksByUserIDRequest : GenericDataTableRequest
    {
        public int CurrentBankId { get; set; }

        public bool IsCopy { get; set; }
    }
}
