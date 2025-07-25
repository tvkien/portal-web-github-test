using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ExtractUserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }        
        public string FirstName{ get; set; }
        public string LastName { get; set; }        
        public string SchoolName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int SchoolId { get; set; }
    }
}