using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetGroupUserResponse
    {
        public int TotalRecord { get; set; }
        public List<UserManage> Data { get; set; }
    }
}