using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.AssessmentItem
{
    public class CreateVirtualTestModel
    {
        public int? QTIItemGroupID { get; set; }
        public int? StateID { get; set; }
        public int? DistrictID { get; set; }
        public int? DatasetCategoryId { get; set; }
        public string SubjectIDs { get; set; }
        public int? BankID { get; set; }
        public string BankName { get; set; }
        public string VirtualTestName { get; set; }
        public bool IsExistingBank { get; set; }
    }
}
