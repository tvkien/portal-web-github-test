using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Retake
{
    public class LoadAssignmentRetakeRequest : PaggingInfo
    {
        public string RetakeRequestGuid { get; set; }
        public string GeneralSearch { get; set; }

        public LoadAssignmentRetakeRequest()
        {
            SortColumn = "StudentName";
            SortDirection = "ASC";
            PageSize = 25;
            StartRow = 0;
        }
    }
}
