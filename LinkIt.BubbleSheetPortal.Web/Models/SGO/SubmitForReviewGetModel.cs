using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.Models.SGO
{
    public class SubmitForReviewGetModel
    {
        public int SGOID { get; set; }
        public List<UserModel> DistrictAdmins { get; set; } 
    }
}