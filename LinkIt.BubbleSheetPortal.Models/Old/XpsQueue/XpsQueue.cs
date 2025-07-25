using Envoc.Core.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.XpsQueue
{
    public class XpsQueue : ValidatableEntity<XpsQueue>
    {
        public int XpsQueueID { get; set; }
        public int XpsDistrictUploadID { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int? XpsUpLoadTypeID { get; set; }
        public int? XpsQueueStatusID { get; set; }
        public int? XpsQueueResultID { get; set; }
        public DateTime? SchedStart { get; set; }
        public int? ProcessId { get; set; }
        public bool IsValidation { get; set; }
    }
}
