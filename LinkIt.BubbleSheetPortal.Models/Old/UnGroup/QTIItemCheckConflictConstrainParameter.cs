using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIItemCheckConflictConstrainParameter
    {
        public int QtiItemId { get; set; }
        public string XmlContent { get; set; }
        public bool IsChangeAnswerChoice { get; set; }
        public bool? IsVirtualTestHasRetakeRequest { get; set; }
    }
}
