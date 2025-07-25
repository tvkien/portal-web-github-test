using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UserManage
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
        public string GroupName { get; set; }
        public string UserStatusName
        {
            get {
                if (UserStatusId.HasValue && UserStatusId.Value == (int)UserStatus.Active)
                {
                    return "Active";
                }
                else if (UserStatusId.HasValue && UserStatusId.Value == (int)UserStatus.Suspended)
                {
                    return "Suspended";
                }
                else
                {
                    return "Inactive";
                }                
            }
        }
    }
}
