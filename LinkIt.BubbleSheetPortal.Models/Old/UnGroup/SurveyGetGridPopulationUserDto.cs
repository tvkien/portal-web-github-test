using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SurveyGetGridPopulationUserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string SchoolName { get; set; }
        public int RoleId { get; set; }
        public int TotalRecords { get; set; }
    }
}
