using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.SharingGroup
{
    public class UserInSharingGroupDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? UserStatusId { get; set; }
        public string DistrictName { get; set; }
        public string StateName { get; set; }
        public string RoleName { get; set; }
        public string SchoolList { get; set; }
        public List<string> SchoolIdList { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int RoleId { get; set; }
        public string SharingGroupName { get; set; }
        public string UserStatusName { get; set; }
    }
}
