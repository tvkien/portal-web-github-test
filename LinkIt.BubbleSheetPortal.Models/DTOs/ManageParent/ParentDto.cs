using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class ParentDto
    {
        public int ParentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public int DistrictID { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int UserID { get; set; }

    }
}
