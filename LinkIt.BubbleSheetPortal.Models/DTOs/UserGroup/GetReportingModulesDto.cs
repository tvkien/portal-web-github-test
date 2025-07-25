using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup
{
    public class GetReportingModulesDto
    {
        public int Order { get; set; }
        public int ModuleID { get; set; }
        public string ModuleCode { get; set; }
    }
}
