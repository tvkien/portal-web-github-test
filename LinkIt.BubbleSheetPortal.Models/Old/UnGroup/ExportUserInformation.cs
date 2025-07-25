using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExportUserInformation
    {
        public int SchoolID { get; set; }

        public string SchoolName { get; set; }

        public int UserID { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string NameFirst { get; set; }

        public string NameLast { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

    }
}
