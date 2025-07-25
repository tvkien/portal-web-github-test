using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExportTestResultTemplate
    {

        public string TestName { get; set; }

        public int SchoolID{ get; set; }

        public string SchoolName{ get; set; }

        public string DistrictTermName{ get; set; }

        public string ClassName{ get; set; }

        public int UserID{ get; set; }

        public string Username{ get; set; }

        public string FirstName{ get; set; }

        public string LastName{ get; set; }

        public string Email{ get; set; }

        public int StudentID{ get; set; }

        public decimal ScoreRaw{ get; set; }

        public System.DateTime ResultDate{ get; set; }

        public string Code{ get; set; }

        public string StatusName{ get; set; }

        public string TimeSettingValue{ get; set; }
		
    }
}
