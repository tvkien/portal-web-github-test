using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO
{
    [Serializable()]
    public class ClassLinkProfileDTO
    {
        public string LoginId { get; set; }
        public string Role { get; set; }
        public string SourcedId { get; set; }
        public string SourceId => SourcedId;
        public int TenantId { get; set; }
        public int StateId { get; set; }
        public string Email { get; set; }
        public bool IsStudent => Role.IsStudentRole();
    }
}
