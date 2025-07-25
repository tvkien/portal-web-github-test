using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ExtractTestViewModel
    {
        public int VirtualTestId { get; set; }        

        public string TestName { get; set; }
        public string BankName { get; set; }       

        public string Subject { get; set; }
        public string Grade { get; set; }
    }
}
