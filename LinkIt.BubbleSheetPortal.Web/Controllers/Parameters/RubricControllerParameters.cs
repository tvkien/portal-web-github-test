using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class RubricControllerParameters
    {
        public ListRubricService ListRubricServices { get; set; }
        public UserSchoolService UserSchoolServices { get; set; }
        public UserBankService UserBankServices { get; set; }
        public VirtualTestFileService VirtualTestFileServices { get; set; }
    }
}