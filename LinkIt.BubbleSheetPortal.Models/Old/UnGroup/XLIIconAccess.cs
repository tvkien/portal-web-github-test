using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class XLIIconAccess
    {
        private string iconCode;

        public bool Restrict { get; set; }
        public bool? AllModules { get; set; }
        public bool? Expires { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        
        public int? DistrictId { get; set; }
        public int XliAreaId { get; set; }

        public int? SchoolDistrictId { get; set; }
        public bool? DistrictSchoolRestrict { get; set; }
        public int? SchoolId { get; set; }
        public DateTime? AreaSchoolStartDate { get; set; }
        public DateTime? AreaSchoolEndDate { get; set; }
        public bool? AreaSchoolExprires { get; set; }
        
        public string IconCode
        {
            get { return iconCode; }
            set { iconCode = value.ConvertNullToEmptyString(); }
        }

        public bool? AllRoles { get; set; }
        public int? RoleID { get; set; }
    }
}
