using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Retake
{
    public class LoadAssignmentRetakeResponse
    {
        public int TotalRecord { get; set; }
        public List<RetakeTestAssignResultViewModel> RetakeTestAssignResults { get; set; }
    }
}
