using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class RetagTestResult
    {
        public string ListTestResultIds { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public bool Gradebook { get; set; }
        public bool StudentRecord { get; set; }
        public bool CleverApi { get; set; }
        public bool IsExportRawScore { get; set; }
    }
}
