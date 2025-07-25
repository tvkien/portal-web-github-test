using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class ParentStudentEmailModel
    {
        public List<UserRegistrationCodeDto> ListUserRegistrationCode { get; set; }
        public string HTTPProtocal { get; set; }
    }
}
