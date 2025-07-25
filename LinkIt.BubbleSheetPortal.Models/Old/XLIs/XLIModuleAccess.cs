using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class XLIModuleAccess
    {   
        private string areaCode;
        private string moduleCode;

        public bool ModuleAllRoles { get; set; }
        public bool ModuleRestrict { get; set; }
        public int XliModuleId { get; set; }
        public int? DMRoleId { get; set; }
        public int? SMRoleId { get; set; }
        public int? DSMDistrictId { get; set; }
        public bool? DSMRestrict { get; set; }
        public bool? SMAllRole { get; set; }
        public int? SchoolId { get; set; }
        public DateTime? SMStartDate { get; set; }
        public DateTime? SMEndDate { get; set; }
        public bool? SMExprires { get; set; }
        public bool? DMAllRoles { get; set; }
        public DateTime? DMEndDate { get; set; }
        public DateTime? DMStartDate { get; set; }
        public bool? DMExpires { get; set; }
        public int? DMDistrictId { get; set; }

        public string AreaCode
        {
            get { return areaCode; }
            set { areaCode = value.ConvertNullToEmptyString(); }
        }
        public string ModuleCode
        {
            get { return moduleCode; }
            set { moduleCode = value.ConvertNullToEmptyString(); }
        }
    }
}
