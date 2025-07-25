using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSReportHeaderViewModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                {
                    return string.Format("{0} {1}", FirstName, LastName);
                }
                else
                {
                    return string.Format("{0}{1}", FirstName, LastName);
                }
            }
        }
    }
}