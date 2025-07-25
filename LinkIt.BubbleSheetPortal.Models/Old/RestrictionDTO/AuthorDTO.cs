using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Models.RestrictionDTO
{
    public class AuthorDTO
    {
        public int BankId { get; set; }
        public int UserId { get; set; }
        public int DistrictId { get; set; }
        public int SchoolId { get; set; }
    }
}