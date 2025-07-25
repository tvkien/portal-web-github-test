using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.AuthorizeItemLib
{
    public class AuthorizeUser
    {
        public int DistrictID { get; set; }
        public int RoleID { get; set; }
        public int UserID { get; set; }
        public string[] LibraryType { get; set; }
    }
}
