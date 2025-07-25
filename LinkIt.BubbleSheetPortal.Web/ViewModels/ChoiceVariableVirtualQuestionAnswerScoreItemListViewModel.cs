using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel
    {
        public string Answer { get; set; }
        public string QtiItemScore { get; set; }
        public int TestScore { get; set; }
        public int VirtualQuestionAnswerScoreId { get; set; }
        
    }
}
