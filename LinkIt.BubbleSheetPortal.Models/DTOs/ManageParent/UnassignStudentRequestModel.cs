using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class UnassignStudentRequestModel
    {
        public int StudentId { get; set; }
        public int ParentUserId { get; set; }
    }
}
