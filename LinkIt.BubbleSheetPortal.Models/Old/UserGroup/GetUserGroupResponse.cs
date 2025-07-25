using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetUserGroupResponse
    {
        public int TotalRecord { get; set; }

        public List<XLIGroup> Data { get; set; }
    }
}
