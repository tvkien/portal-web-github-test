using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class VirtualSectionQuestionItemListViewModel
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public int QtiItemId { get; set; }
        public int QuestionOrder { get; set; }
        public int SectionOrder { get; set; }
        
    }
}
