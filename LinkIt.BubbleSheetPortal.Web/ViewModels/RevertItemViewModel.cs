using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RevertItemViewModel
    {
        public int VirtualQuestionId { get; set; }
        public int QtiItemId { get; set; }
        public int QtiItemHistoryId { get; set; }
    }
}
