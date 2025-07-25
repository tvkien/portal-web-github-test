using LinkIt.BubbleSheetPortal.Models.Enum;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole
{
    public class PublishUserInformationDto
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string SchoolName { get; set; }
        public string LICode { get; set; }
    }

}
