using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ParentListViewModel
    {
        public int UserID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }        
        public string Phone { get; set; }
        public string MessageNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}