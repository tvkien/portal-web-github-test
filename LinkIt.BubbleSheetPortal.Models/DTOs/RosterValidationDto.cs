using LinkIt.BubbleSheetPortal.Models.Old.XpsQueue;
using LinkIt.BubbleSheetPortal.Models.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class RosterValidationDto
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public XpsQueue XpsQueue { get; set; }
        public Request Request { get; set; }
    }
}
