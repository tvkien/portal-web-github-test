namespace LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences
{
    public class GetPreferencesParams
    {
        public int CurrrentLevelId { get; set; }
        public int EnterpriseId { get; set; } = 0;
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public int VirtualTestId { get; set; }
        public int TestAssignmentId { get; set; }
        public bool IsSurvey { get; set; }
        public bool IsFromOnlineTestingPreferences { get; set; } = false;
        public int StateId { get; set; }
        public int? SchoolId { get; set; }
    }
}
