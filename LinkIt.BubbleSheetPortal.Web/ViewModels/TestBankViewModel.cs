using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestBankViewModel
    {
        public string BankName { get; set; }
        public string Subject { get; set; }
        public int GradeOrder { get; set; }
        public int BankID { get; set; }
        public string Grade { get; set; }
        public bool Archived { get; set; }
    }
}