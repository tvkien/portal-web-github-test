using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.SharingGroup
{
    public class SharingGroupResponseDto
    {
        public List<SharingGroupDto> Data { get; set; } = new List<SharingGroupDto>();
        public int TotalRecord { get; set; }
    }
}
