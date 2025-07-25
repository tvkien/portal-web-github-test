using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.RetakeAssignment
{
    public class GetTestPreferencePartialRetakeModel
    {
        public int VirtualTestID { get; set; }
        public string StudentIDs { get; set; }
        public string GUID { get; set; }
    }
}
