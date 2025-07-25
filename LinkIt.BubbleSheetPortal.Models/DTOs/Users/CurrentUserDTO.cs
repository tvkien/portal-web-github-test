using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Users
{
    public class CurrentUserDTO
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public List<int> ListDistrictId { get; set; }
        public int Id { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int RoleId { get; set; }
        public int? DistrictGroupId { get; set; }
        public int? SchoolId { get; set; }
        public bool? Active { get; set; }
    }
}
