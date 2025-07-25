using LinkIt.BubbleSheetPortal.Models.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class GetChildrenListRequestModel : GenericDataTableRequestBase
    {
        public int ParentUserId { get; set; }
        public string studentIdsThatBeAddedOnCommit { get; set; }
    }
}
