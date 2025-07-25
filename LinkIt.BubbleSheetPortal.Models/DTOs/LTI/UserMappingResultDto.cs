
namespace LinkIt.BubbleSheetPortal.Models.DTOs.LTI
{
    public class UserMappingResultDto
    {
        public string UserName { get; set; }
        public bool IsFirstStudentLogonSSO { get; set; } = false;
        public int RoleId { get; set; }
    }
}
