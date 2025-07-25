namespace LinkIt.BubbleSheetPortal.Models.Old.UnGroup
{
    public class GetBanksByUserIDFilter
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int DistrictID { get; set; }
        public int BankID { get; set; }
        public bool ShowArchived { get; set; }
        public bool HideTeacherBanks { get; set; }
        public bool HideOtherPeopleBanks { get; set; }
        public bool HideBankOnlyForm { get; set; }
        public bool IsSurvey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = int.MaxValue;
        public string GeneralSearch { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }
}
