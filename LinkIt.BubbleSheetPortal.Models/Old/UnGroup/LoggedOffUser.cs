using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class LoggedOffUser
    {
        public int LoggedOffUserID { get; set; }
        public string RedirectURL { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserID { get; set; }
    }
}
