using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.SharingGroup
{
    public class UserAvailableAddSharingGroupResponseDto
    {
        public List<UserInSharingGroupDto> Data { get; set; } = new List<UserInSharingGroupDto>();
        public int TotalRecord { get; set; }
    }
}
