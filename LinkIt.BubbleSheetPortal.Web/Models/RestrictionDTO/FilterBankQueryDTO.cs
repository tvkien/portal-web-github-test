using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.RestrictionDTO
{
    public class FilterBankQueryDTO
    {
        public string ModuleCode { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public List<ListItem> Banks { get; set; }
    }
}