using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class APILogDetail : APILog
    {
        public string Account { get; set; }
        public string AccountType { get; set; }
        public string DistrictCustom { get; set; }
        public string UserCustom { get; set; }
    }
}