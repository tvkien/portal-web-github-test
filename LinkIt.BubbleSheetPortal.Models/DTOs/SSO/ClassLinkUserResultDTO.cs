using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO
{
    public class ClassLinkUserResultDTO
    {
        public ClassLinkUserResultDTO()
        {
            Users = new List<User>();
            Students = new List<Student>();
            Parents = new List<ParentDto>();
        }

        public List<User> Users { get; set; }

        public List<Student> Students { get; set; }

        public List<ParentDto> Parents { get; set; }
    }
}
